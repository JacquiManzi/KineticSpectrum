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
        private int _destnPort;
        private IPAddress _localIPAddress;
        private IPEndPoint _destEndPoint;
        private NetworkInterface _bindedNetworkCard;
        private List<NetworkInterface> _networkCardList = null;
        IPEndPoint _ipEndPoint = new IPEndPoint(IPAddress.Any, 55350);
        private PDS60ca powerSupply = new PDS60ca();
        private readonly Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
   
        public Network()
        {
        }

/*           
 * Retrieve a list of all the Network Interfaces on your system
 */

        public static List<NetworkInterface> RetrieveNetworkCards()
        {
            NetworkInterface[] adapters = NetworkInterface.GetAllNetworkInterfaces();
            return new List<NetworkInterface>(adapters);
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

        private void handleCallback(/*Object obj, SocketAsyncEventArgs sockArgs*/ IAsyncResult result)
        {
            Socket socket = (Socket)result.AsyncState;
            SocketFlags flags = SocketFlags.None;
            EndPoint endPoint = _ipEndPoint;
            socket.EndReceiveFrom(result, ref endPoint);
            _destEndPoint = (IPEndPoint) endPoint;
            Console.WriteLine("Received response from: {0}", _destnPort);
        }

/*
 * Broadcast to the power supplies
 */
        public void BroadCast()
        {
            IPEndPoint broadCastIp = new IPEndPoint(IPAddress.Parse("169.254.255.255"), 6038);

            socket.Bind(_ipEndPoint);
            socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, true);
              
            byte[] data1 = DecodeString(PDS60ca.DataOne);
            byte[] data2 = DecodeString(PDS60ca.DataTwo);
            EndPoint endPoint = _ipEndPoint;
            
            socket.BeginReceiveFrom(new byte[500], 0, 500, SocketFlags.None, ref endPoint,
                                           new AsyncCallback(handleCallback), socket); 
            
            while (_destEndPoint == null)
            {
                for (int i = 0; i < 5; i++)
                {
                    socket.SendTo(data1, SocketFlags.None, broadCastIp);
                    Thread.Sleep(250);
                }
                socket.SendTo(data2, SocketFlags.None, broadCastIp);
                Thread.Sleep(3000);               
            } 
        }

        public void SendUpdate(ColorData colorData)
        {
            socket.SendTo(DecodeString(PDS60ca.byteStringOne), SocketFlags.None, _destEndPoint);
            Thread.Sleep(10);
            socket.SendTo(colorData.Bytes, SocketFlags.None, _destEndPoint);
        }

        public void SendUpdate(Color color)
        {
            ColorData colorData = new ColorData(DecodeString(powerSupply.IntialHex),50);
            for(int i=0; i<colorData.Count; i++)
            {
                colorData[i] = color;
            }
            SendUpdate(colorData);
        }

        public void SendUpdate(Color[] colors)
        {
            ColorData colorData = new ColorData(DecodeString(powerSupply.IntialHex), 50);

            for(int i=0; i<colors.Length; i++)
            {
                colorData[i] = colors[i];
            }

            SendUpdate(colorData);
        }

        /*
         * Decode
         */
        public static byte[] DecodeString(String hexString)
        {
            byte[] hexBytes = new byte[hexString.Length / 2];
            for (int i = 0; i < hexString.Length / 2; i++)
            {
                hexBytes[i] = Byte.Parse(hexString.Substring(i*2, 2).ToUpper(), NumberStyles.AllowHexSpecifier);
            }
            return hexBytes;
        }

        public void SetInterface(String interfaceName)
        {
            foreach(var card in NetworkCardList)
            {
               if(card.Name.Equals(interfaceName))
               {
                   NetworkCard = card;
                   return;
               }
            }
            throw new ArgumentException("Interface '" +interfaceName + "' does not exist.");
        }

/*
 * Getters and setters for the network
 */

        public List<NetworkInterface> NetworkCardList
        {
            get { return _networkCardList ?? (_networkCardList = RetrieveNetworkCards()); }
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

    }

}
