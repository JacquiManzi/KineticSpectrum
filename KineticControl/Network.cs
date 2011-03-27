using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
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
        private IPHostEntry _entry = Dns.GetHostEntry(Dns.GetHostName());
        private IPAddress _localIPAddress;
        private IPEndPoint _destEndPoint;
        private NetworkInterface _bindedNetworkCard;
        private List<NetworkInterface> _networkCardList = new List<NetworkInterface>();
        IPEndPoint _ipEndPoint = new IPEndPoint(IPAddress.Any, 55350);
        private PDS60ca powerSupply = new PDS60ca();
        private Color[] _address;
        private readonly Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
   
        public Network()
        {
        }
        
        public Network(Color[] address)
        {
            _address = address;
        }

/*           
 * Retrieve a list of all the Network Interfaces on your system
 */

        public List<NetworkInterface> RetrieveNetworkCards()
        {
            NetworkInterface[] adapters = NetworkInterface.GetAllNetworkInterfaces();

            foreach (NetworkInterface networkInterface in adapters)
            {
                _networkCardList.Add(networkInterface);
            }

            return _networkCardList;
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
            Console.WriteLine("Received");
        }

/*
 * Broadcast to the power supplies
 */
        public void BroadCast()
        {
            IPEndPoint broadCastIp = new IPEndPoint(IPAddress.Parse("169.254.255.255"), 6038);
            string dataStr = "\0\0\0" + ((char)246).ToString();

            List<byte[]> bytes = new List<byte[]>();

            
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
                    System.Threading.Thread.Sleep(250);
                }
                socket.SendTo(data2, SocketFlags.None, broadCastIp);
                System.Threading.Thread.Sleep(3000);               
            }

                     
        }

        public void SendUpdate(Color[] colors)
        {
            int length = (powerSupply.IntialHex.Length + powerSupply.AddressOff.Length) / 2;
            byte[] colorData = new byte[length];
            byte[] initial = DecodeString(powerSupply.IntialHex);

            colorData.Initialize();
            
            initial.CopyTo(colorData, 0);
            for (int i = 0; i < colors.Length; i++)
            {
                colorData[initial.Length + i*3] = colors[i].R;
                colorData[initial.Length + i * 3 + 1] = colors[i].G;
                colorData[initial.Length + i * 3 + 2] = colors[i].B;
            }

            socket.SendTo(DecodeString(PDS60ca.byteStringOne), SocketFlags.None, _destEndPoint);
            System.Threading.Thread.Sleep(100);  
            socket.SendTo(colorData, SocketFlags.None, _destEndPoint); 
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

/*
 * Getters and setters for the network
 */

        public List<NetworkInterface> NetworkCardList
        {
            get { return _networkCardList; }
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
