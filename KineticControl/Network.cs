using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

/*
 * This class will collect all of the necessary information to set up your network
 */

namespace KineticControl
{
    public class Network
    {
        private int _localPort;
        private readonly IPHostEntry _entry = Dns.GetHostEntry(Dns.GetHostName());
        private volatile IPEndPoint _destEndPoint;
        private readonly List<NetworkInterface> _networkCardList = new List<NetworkInterface>();
        private readonly IPEndPoint _ipEndPoint = new IPEndPoint(IPAddress.Any, 53868);
        private IPAddress _localIPAddress;

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

        public IPAddress InitializeLocalIp()
        {
            _localIPAddress = _entry.AddressList.First();
           
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

        private void HandleCallback(/*Object obj, SocketAsyncEventArgs sockArgs*/ IAsyncResult result)
        {
            Socket socket = (Socket)result.AsyncState;
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
                  
            UdpClient udp = new UdpClient();
            List<byte[]> bytes = new List<byte[]>();

            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            socket.Bind(_ipEndPoint);
           
            byte[] data1 = DecodeString(PDS60ca.DataOne);
            byte[] data2 = DecodeString(PDS60ca.DataTwo);
            EndPoint endPoint = _ipEndPoint;

            //socket.BeginReceive(new List<ArraySegment<byte>>(){new ArraySegment<byte>(new byte[500])}, SocketFlags.None, new AsyncCallback(handleCallback), this);
            socket.BeginReceiveFrom(new byte[500], 0, 500, SocketFlags.None, ref endPoint,
                                           HandleCallback, socket);  
 
            //_socketArgs.BufferList = new List<ArraySegment<byte>>() {new ArraySegment<byte>(new byte[500])};
            //_socketArgs.Completed += new EventHandler<SocketAsyncEventArgs>(handleCallback);
            //_socketArgs.SetBuffer(new byte[500],0,500 );
            //socket.ReceiveAsync(_socketArgs);)

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

            while (udp.Available > 0)
                bytes.Add(udp.Receive(ref broadCastIp));
            //FindDeviceIP(udp, bytes, broadCastIp);
            udp.Close();
            
        }

/*
 * Decode
 */

        public static byte[] DecodeString(String hexString)
        {
            byte[] hexBytes = new byte[hexString.Length / 2];
            for (int i = 0; i < hexString.Length / 2; i += 2)
            {
                hexBytes[i / 2] = Byte.Parse(hexString.Substring(i, 2).ToUpper(), NumberStyles.AllowHexSpecifier);
            }
            return hexBytes;
        }

/*
* Find the IP Address of the connected power supply
*/

//        public IPAddress FindDeviceIP(UdpClient udp, List<byte[]> bytes, IPEndPoint broadCastIp)
//        {
            //Broadcat your local information to outside devices to find their information
            //BroadCast();
//
//            while (udp.Available > 0)
//                bytes.Add(udp.Receive(ref broadCastIp));
//
//            while (udp.Available > 0)
//            {
//                IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);
//                udp.Receive(ref remoteEndPoint);
                // remoteEndPoint holds the remote IP
//
//                _destnIPAddress = remoteEndPoint.Address;
//            }
//            udp.Close();
//            
//            return _destnIPAddress;
//        }


/*
 * Getters and setters for the network
 */

        public List<NetworkInterface> NetworkCardList
        {
            get { return _networkCardList; }
        }

        public int LocalPort
        {
            get { return _localPort; }
            set { _localPort = value; }
        }

        public IPEndPoint DestEndPoint
        {
            get { return _destEndPoint; }
            set { _destEndPoint = value; }
        }

        public IPAddress LocalIPAddress
        {
            get { return _localIPAddress; }
            set { _localIPAddress = value; }
        }

    }

}
