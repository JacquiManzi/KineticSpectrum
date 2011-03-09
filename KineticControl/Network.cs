using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;

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
        private IPAddress _destnIPAddress;
        private NetworkInterface _bindedNetworkCard;
        private List<NetworkInterface> _networkCardList = new List<NetworkInterface>();


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
            _localIPAddress = _entry.AddressList.First();
           
            return _localIPAddress;
        }


/*
 * Broadcast to the power supplies
 */
        public void BroadCast()
        {

            IPEndPoint broadCastIp = new IPEndPoint(IPAddress.Parse("255.255.255.255"), _localPort);
            string dataStr = "\0\0\0" + ((char)246).ToString();

            UdpClient udp = new UdpClient();
            byte[] data = new UTF8Encoding().GetBytes(dataStr);
            List<byte[]> bytes = new List<byte[]>();

            udp.Send(data, data.Length, broadCastIp);
            System.Threading.Thread.Sleep(481);

            while (udp.Available > 0)
                bytes.Add(udp.Receive(ref broadCastIp));
            FindDeviceIP(udp, bytes, broadCastIp);
            udp.Close();
            
        }


/*
* Find the IP Address of the connected power supply
*/

        public IPAddress FindDeviceIP(UdpClient udp, List<byte[]> bytes, IPEndPoint broadCastIp)
        {
            //Broadcat your local information to outside devices to find their information
            //BroadCast();

            while (udp.Available > 0)
                bytes.Add(udp.Receive(ref broadCastIp));

            while (udp.Available > 0)
            {
                IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);
                udp.Receive(ref remoteEndPoint);
                // remoteEndPoint holds the remote IP

                _destnIPAddress = remoteEndPoint.Address;
            }
            udp.Close();
            
            return _destnIPAddress;
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

    }

}
