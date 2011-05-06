using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Media;

/*
 * This class will collect all of the necessary information to set up your network
 */

namespace KineticControl
{
    public class Network
    {
        private int _localPort;
        private int _destnPort = 0;
        private IPAddress _localIPAddress;
        private IPEndPoint _destEndPoint;
        private NetworkInterface _bindedNetworkCard;
        private List<NetworkInterface> _networkCardList;
        readonly IPEndPoint _ipEndPoint = new IPEndPoint(IPAddress.Any, 55350);
        private readonly Socket _socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

        private List<PDS> _pdss = new List<PDS>();
    
   
        public Network()
        {
            _pdss.Add(new PDS60ca(this, new IPEndPoint(IPAddress.Parse( "169.254.49.154" ),6038 )));
            _pdss.Add(new PDS60ca(this, new IPEndPoint(IPAddress.Parse( "169.254.49.160" ),6038 )));
            _pdss.Add(new PDS150e(this, new IPEndPoint(IPAddress.Parse( "169.254.49.149" ), 6038)));
        }
        
/*           
 * Retrieve a list of all the Network Interfaces on your system
 */

        public List<NetworkInterface> RetrieveNetworkCards()
        {
            NetworkInterface[] adapters = NetworkInterface.GetAllNetworkInterfaces();
            return _networkCardList = new List<NetworkInterface>(adapters);
        }

        public void SetInterface(String networkInterface)
        {
            foreach(NetworkInterface inter in NetworkCardList)
            {
                if(inter.Name.Equals(networkInterface))
                {
                    NetworkCard = inter;
                    return;
                }
            }
        }

/*
 * Retrieve your local IP Address for your default network interface
 */

        public IPAddress InitializeLocalIP()
        {
            _localIPAddress =_bindedNetworkCard.GetIPProperties().UnicastAddresses[0].Address;
                       
            return _localIPAddress;
        }


/*
 * Initialize your the local port to cmmunicate over
 */

        public int InitializeLocalPort()
        {
            TcpListener listener = new TcpListener(IPAddress.Parse("127.0.0.1"), 0);
            listener.Start();
             _localPort = ((IPEndPoint)listener.LocalEndpoint).Port;
            listener.Stop();
            
            return _localPort;
        }

        private void HandleCallback(IAsyncResult result)
        {
            Socket socket = (Socket)result.AsyncState;
            EndPoint endPoint = _ipEndPoint;
            socket.EndReceiveFrom(result, ref endPoint);
            lock (this)
            {
                _destEndPoint = (IPEndPoint) endPoint;
                PDS60ca newPDS = new PDS60ca(this, _destEndPoint);
                if (!_pdss.Contains(newPDS))
                    _pdss.Add(newPDS);
                Console.WriteLine("Found Device at endpoint: " + _destEndPoint);
            }
        }

/*
 * Broadcast to the power supplies
 */
        public void BroadCast()
        {
            IPEndPoint broadCastIp = new IPEndPoint(IPAddress.Parse("169.254.255.255"), 6038);

            _socket.Bind(_ipEndPoint);
            _socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, true);
              
            byte[] data1 = HexStrings.DataOne;
            byte[] data2 = HexStrings.DataTwo;
            byte[] data3 = HexStrings.DataThree;
            byte[] data4 = HexStrings.DataFour;

            EndPoint endPoint = _ipEndPoint;
            
            _socket.BeginReceiveFrom(new byte[500], 0, 500, SocketFlags.None, ref endPoint,
                                           HandleCallback, _socket); 
            
            while (_destEndPoint == null)
            {
                for (int i = 0; i < 5; i++)
                {
                    _socket.SendTo(data3, SocketFlags.None, broadCastIp);
                    Thread.Sleep(250);
                }
                _socket.SendTo(data4, SocketFlags.None, broadCastIp);
                Thread.Sleep(1000);
            }
        }

        public void SendUpdate(ColorData colorData)
        {
            _socket.SendTo(colorData.Bytes, SocketFlags.None, _destEndPoint);
        }

        public void SendUpdate(IPEndPoint endPoint, ColorData colorData)
        {
            _socket.SendTo(colorData.Bytes, SocketFlags.None, endPoint);
        }

        public void SendUpdate(Color[] colors, int network)
        {
            int length = (PDS60ca.IntialHex.Length + HexStrings.AddressOff.Length) / 2;
            byte[] colorData = new byte[length];
            byte[] initial;

            if (network == 1)
            {
                initial = PDS60ca.IntialHex;
            }
            else
            {
                initial = PDS60ca.IntialHex2;
            }

            colorData.Initialize();
            
            initial.CopyTo(colorData, 0);
            for (int i = 0; i < colors.Length; i++)
            {
                colorData[initial.Length + i*3] = colors[i].R;
                colorData[initial.Length + i * 3 + 1] = colors[i].G;
                colorData[initial.Length + i * 3 + 2] = colors[i].B;
            }

//            if (network == 2)
//            {
//                _socket.SendTo(DecodeString(PDS60ca.byteStringThree), SocketFlags.None, _destEndPoint);
//            }
//            else
//            {
//                _socket.SendTo(DecodeString(PDS60ca.byteStringOne), SocketFlags.None, _destEndPoint);
//            }
            Thread.Sleep(20);  
            _socket.SendTo(colorData, SocketFlags.None, _destEndPoint);
            Thread.Sleep(20);
        }

        
        public void FindPowerSupplies(int timeOutInMs = int.MaxValue)
        {
            DateTime start = DateTime.Now;
            IPEndPoint broadCastIp = new IPEndPoint(IPAddress.Parse("255.255.255.255"), 6038);

            _socket.Bind(_ipEndPoint);
            _socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, true);

            byte[] data1 = HexStrings.PsSearch;
           
            EndPoint endPoint = _ipEndPoint;

            _socket.BeginReceiveFrom(new byte[500], 0, 500, SocketFlags.None, ref endPoint,
                                           HandleCallback, _socket);
            int ms = 0;
            do
            {
                for (int i = 0; i < 5; i++)
                {
                    ms = (int) Math.Min(250, (DateTime.Now - start).TotalMilliseconds + timeOutInMs);
                    _socket.SendTo(data1, SocketFlags.None, broadCastIp);
                    Thread.Sleep(ms);
                }
                // _socket.SendTo(data2, SocketFlags.None, broadCastIp);
                if (_destEndPoint == null)
                    Thread.Sleep(3000);
            } while (_destEndPoint == null && ms > 0);
        }

     

/*
 * Decode
 */

        

/*
 * Getters and setters for the network
 */

        public List<NetworkInterface> NetworkCardList
        {
            get { return _networkCardList ?? RetrieveNetworkCards(); }
            set { this._networkCardList = value; }
        }

        public int LocalPort
        {
            get { return _localPort; }
            set { this._localPort = value; }
        }

        public int DestnPort
        {
            get { return _destnPort; }
            set { this._destnPort = value; }
        }

        public IPAddress LocalIPAddress
        {
            get { return _localIPAddress; }
            set { this._localIPAddress = value; }
        }

        public NetworkInterface NetworkCard
        {
            get { return _bindedNetworkCard; }
            set { this._bindedNetworkCard = value; }
        }

        public IPEndPoint ipEndpoint
        {
            get { return _ipEndPoint; }
        }

        public IList<PDS> PDSs
        {
            get { return _pdss.AsReadOnly(); }
        }


        

    }

}
