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
        private int _destnPort = 0;
        private IPHostEntry _entry = Dns.GetHostEntry(Dns.GetHostName());
        private IPAddress _localIPAddress;
        private IPEndPoint _destEndPoint;
        private NetworkInterface _bindedNetworkCard;
        private List<NetworkInterface> _networkCardList;
        IPEndPoint _ipEndPoint = new IPEndPoint(IPAddress.Any, 55350);
        private PDS60ca powerSupply = new PDS60ca();
        private readonly Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
    
   
        public Network()
        {
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
            byte[] data3 = DecodeString(PDS60ca.DataThree);
            byte[] data4 = DecodeString(PDS60ca.DataFour);

            EndPoint endPoint = _ipEndPoint;
            
            socket.BeginReceiveFrom(new byte[500], 0, 500, SocketFlags.None, ref endPoint,
                                           new AsyncCallback(handleCallback), socket); 
            
            while (_destEndPoint == null)
            {
                for (int i = 0; i < 5; i++)
                {
                    socket.SendTo(data3, SocketFlags.None, broadCastIp);
                    System.Threading.Thread.Sleep(250);
                }
                socket.SendTo(data4, SocketFlags.None, broadCastIp);
                System.Threading.Thread.Sleep(3000);               
            }
        }

        public void SendUpdate(ColorData colorData)
        {
            socket.SendTo(colorData.Bytes, SocketFlags.None, _destEndPoint);
        }

        public void SendUpdate(Color[] colors, int network)
        {
            int length = (powerSupply.IntialHex.Length + powerSupply.AddressOff.Length) / 2;
            byte[] colorData = new byte[length];
            byte[] initial;

            if (network == 1)
            {
                initial = DecodeString(powerSupply.IntialHex);
            }
            else
            {
                initial = DecodeString(powerSupply.IntialHex2);
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
//                socket.SendTo(DecodeString(PDS60ca.byteStringThree), SocketFlags.None, _destEndPoint);
//            }
//            else
//            {
//                socket.SendTo(DecodeString(PDS60ca.byteStringOne), SocketFlags.None, _destEndPoint);
//            }
            System.Threading.Thread.Sleep(20);  
            socket.SendTo(colorData, SocketFlags.None, _destEndPoint);
            System.Threading.Thread.Sleep(20);
        }

        public void FindPowerSupply()
        {
            IPEndPoint broadCastIp = new IPEndPoint(IPAddress.Parse("255.255.255.255"), 6038);
            string dataStr = "\0\0\0" + ((char)246).ToString();

            List<byte[]> bytes = new List<byte[]>();


            socket.Bind(_ipEndPoint);
            socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, true);

            byte[] data1 = DecodeString("0401dc4a01000100000000000a0101de");
           
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
                // socket.SendTo(data2, SocketFlags.None, broadCastIp);
                System.Threading.Thread.Sleep(3000);
            }

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
            set { this._ipEndPoint = value; }
        }

        

    }

}
