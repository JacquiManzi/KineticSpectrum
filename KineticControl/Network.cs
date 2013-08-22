using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;

/*
 * This class will collect all of the necessary information to set up your network
 */

namespace KineticControl
{
    class NetworkState
    {
        public readonly Socket Socket;
        public const int BUFFER_SIZE = 1024;
        public byte[] Buffer = new byte[BUFFER_SIZE];

        internal NetworkState(Socket socket)
        {
            Socket = socket;
        }
    }

    public class Network
    {
        private int _localPort;
        private IPAddress _localIPAddress;
        private NetworkInterface _bindedNetworkCard;
        private List<NetworkInterface> _networkCardList;
        readonly IPEndPoint _ipEndPoint = new IPEndPoint(IPAddress.Any, 55350);
        private readonly Socket _socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        private bool _connected = false;

        private List<PDS> _pdss = new List<PDS>();

        private static Network _instance;
        public static Network GetInstance()
        {
            return _instance ?? (_instance = new Network());
        }

        private Network()
        {
            _pdss.Add(new PDS480Ca(this, new IPEndPoint(IPAddress.Parse( "192.168.1.50" ),6038 )));
            _pdss.Add(new PDS480Ca(this, new IPEndPoint(IPAddress.Parse( "192.168.1.51" ),6038 )));
            _pdss.Add(new PDS480Ca(this, new IPEndPoint(IPAddress.Parse("192.168.1.52"), 6038)));
            _pdss.Add(new PDS480Ca(this, new IPEndPoint(IPAddress.Parse("192.168.1.53"), 6038)));
//            _pdss.Add(new PDS150e(this, new IPEndPoint(IPAddress.Parse( "169.254.49.149" ),6038)));
        }

        public void AddPDS(PDS pds)
        {
            _pdss.Add(pds);
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
            if (_networkCardList.Count == 0)
                RetrieveNetworkCards();
            _connected = false;
            foreach(NetworkInterface inter in NetworkCardList)
            {
                if(inter.Name.Equals(networkInterface))
                {
                    NetworkCard = inter;
                    InitializeLocalIP();
                    InitializeLocalPort();
                    _connected = true;
                    return;
                }
            }
            throw new ArgumentException("Specified network interface '" + networkInterface + "' doesn't exist.");
        }


/*
 * Retrieve your local IP Address for your default network interface
 */

        public IPAddress InitializeLocalIP()
        {
            foreach(var unicastAddress in _bindedNetworkCard.GetIPProperties().UnicastAddresses)
            {
                if(unicastAddress.Address.AddressFamily == AddressFamily.InterNetwork)
                {
                    return _localIPAddress = unicastAddress.Address;
                }
            }
            throw new InvalidOperationException("Specified network card has no IPv4 IP Address...");
        }


/*
 * Initialize your the local port to communicate over
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
            NetworkState state = (NetworkState)result.AsyncState;
            state.Socket.EndReceive(result);
            IPEndPoint newDevice = new IPEndPoint(GetAddressFromResponse(state.Buffer), 6038);
            
            String model = GetModelFromResponse(state.Buffer);
            PDS pds;
            if(model.Equals("SPDS-480ca"))
                pds = new PDS480Ca(this, newDevice);
            else if(model.Equals("PDS-60ca"))
                pds = new PDS60Ca(this, newDevice);
            else
              throw new InvalidOperationException("No handler available for PDS type: " + model);
            
            if (!_pdss.Contains(pds))
                _pdss.Add(pds);
            Console.WriteLine("Found Device '" + model + "' at endpoint:" + newDevice);
        }

        private static IPAddress GetAddressFromResponse(byte[] response)
        {
            if (response == null) throw new ArgumentNullException("response");
            //take bytes 13-16
            byte[] ipBytes = response.Skip(12).Take(4).ToArray();
            return new IPAddress(ipBytes);
        }

        private static String GetModelFromResponse(byte[] response)
        {
            byte[] modelBytes = response.Skip(32).SkipWhile(b => b != 0x0A).Skip(3).TakeWhile(b => b != 0x0A).ToArray();
            return Encoding.ASCII.GetString(modelBytes);
        }


        private IPAddress GetLocalIP(byte endAddress)
        {
            byte[] ipBytes = _localIPAddress.GetAddressBytes();
            ipBytes[3] = endAddress;
            return new IPAddress(ipBytes);
        }

/*
 * Broadcast to the power supplies
 */
        public void BroadCast()
        {
//            IPEndPoint broadCastIp = new IPEndPoint(IPAddress.Parse("255.255.255.255"), 6038);
//            IPEndPoint localBroadCastIp = new IPEndPoint(GetLocalIP(0xff), 6038);
//
//            //arbitrary IP address that we tell the remote device to reply from...
//            IPAddress mscNetworkIP = GetLocalIP(0xC0);
//
//            _socket.Bind(_ipEndPoint);
//            _socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, true);
//
//            NetworkState state = new NetworkState(_socket);
//
//
//            //update this to accept connections, which will trigger the BeginReceive
//            _socket.BeginReceive(state.Buffer, 0, NetworkState.BUFFER_SIZE, 0, HandleCallback, state); 
//            
//            for (int i = 0; i < 5; i++)
//                _socket.SendTo(HexStrings.FullSearch, broadCastIp);
//
//            Thread.Sleep(1000);
//
//            for (int i = 0; i < 4; i++)
//                _socket.SendTo(HexStrings.PartialSearch, localBroadCastIp);
//                
//            Thread.Sleep(10);
//            _socket.SendTo(HexStrings.LocalRequestHex(mscNetworkIP), localBroadCastIp);
//            Thread.Sleep(1000);
            
        }

       
        public void SendUpdate(IPEndPoint endPoint, ColorData colorData)
        {
            if (_connected)
            {
//                var args = new SocketAsyncEventArgs();
//                args.SetBuffer(colorData.Bytes, 0, colorData.Bytes.Count());
//                args.RemoteEndPoint = endPoint;
                //            _socket.SendAsync(args);
                _socket.BeginSendTo(colorData.Bytes, 0, colorData.Bytes.Length,
                    SocketFlags.None, endPoint, null, null);
//                _socket.SendToAsync(args);
            }
        }

//        public void SendUpdate(Color[] colors, int network)
//        {
//            int length = (PDS60Ca.InitialHex.Length + HexStrings.AddressOff.Length) / 2;
//            byte[] colorData = new byte[length];
//            byte[] initial;
//
//            if (network == 1)
//            {
//                initial = PDS60Ca.InitialHex;
//            }
//            else
//            {
//                initial = PDS60Ca.InitialHex2;
//            }
//
//            colorData.Initialize();
//            
//            initial.CopyTo(colorData, 0);
//            for (int i = 0; i < colors.Length; i++)
//            {
//                colorData[initial.Length + i*3] = colors[i].R;
//                colorData[initial.Length + i * 3 + 1] = colors[i].G;
//                colorData[initial.Length + i * 3 + 2] = colors[i].B;
//            }
//
////            if (network == 2)
////            {
////                _socket.SendTo(DecodeString(PDS60ca.byteStringThree), SocketFlags.None, _destEndPoint);
////            }
////            else
////            {
////                _socket.SendTo(DecodeString(PDS60ca.byteStringOne), SocketFlags.None, _destEndPoint);
////            }
//            Thread.Sleep(20);  
//            _socket.SendTo(colorData, SocketFlags.None, _destEndPoint);
//            Thread.Sleep(20);
//        }

        
//        public void FindPowerSupplies(int timeOutInMs = int.MaxValue)
//        {
//            DateTime start = DateTime.Now;
//            IPEndPoint broadCastIp = new IPEndPoint(IPAddress.Parse("255.255.255.255"), 6038);
//
//            _socket.Bind(_ipEndPoint);
//            _socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, true);
//
//            byte[] data1 = HexStrings.PsSearch;
//           
//            EndPoint endPoint = _ipEndPoint;
//
//            _socket.BeginReceiveFrom(new byte[500], 0, 500, SocketFlags.None, ref endPoint,
//                                           HandleCallback, _socket);
//            int ms = 0;
//            do
//            {
//                for (int i = 0; i < 5; i++)
//                {
//                    ms = (int) Math.Min(250, (DateTime.Now - start).TotalMilliseconds + timeOutInMs);
//                    _socket.SendTo(data1, SocketFlags.None, broadCastIp);
//                    Thread.Sleep(ms);
//                }
//                // _socket.SendTo(data2, SocketFlags.None, broadCastIp);
//                if (_destEndPoint == null)
//                    Thread.Sleep(3000);
//            } while (_destEndPoint == null && ms > 0);
//        }

     

/*
 * Decode
 */

        

/*
 * Getters and setters for the network
 */

        public List<NetworkInterface> NetworkCardList
        {
            get { return _networkCardList ?? RetrieveNetworkCards(); }
            set { _networkCardList = value; }
        }

        public int LocalPort
        {
            get { return _localPort; }
            set { _localPort = value; }
        }

        public int DestnPort { get; set; }

        public IPAddress LocalIPAddress
        {
            get { return _localIPAddress; }
            set { _localIPAddress = value; }
        }

        public NetworkInterface NetworkCard
        {
            get { return _bindedNetworkCard; }
            internal set { _bindedNetworkCard = value; }
        }
        
        public IList<PDS> PDSs
        {
            get { return _pdss.AsReadOnly(); }
        }
    }
}
