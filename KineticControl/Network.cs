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
    class Network
    {
        private int localPort;
        private int destnPort;
        private IPHostEntry entry = Dns.GetHostEntry(Dns.GetHostName());
        private IPAddress localIPAddress;
        private IPAddress destnIPAddress;
        private NetworkInterface bindedNetworkCard;

        private List<NetworkInterface> networkCardList = new List<NetworkInterface>();


/*
 * Retrieve a list of all the Network Interfaces on your system
 */

        public List<NetworkInterface> RetrieveNetworkCards()
        {
            NetworkInterface[] adapters = NetworkInterface.GetAllNetworkInterfaces();

            foreach (NetworkInterface networkInterface in adapters)
            {
                networkCardList.Add(networkInterface);
            }

            return networkCardList;
        }

/*
 * Retrieve your local IP Address for your default network interface
 */

        public IPAddress InitializeLocalIP()
        {
            localIPAddress = entry.AddressList.FirstOrDefault();
           
            return localIPAddress;
        }

        public IPAddress FindDeviceIP()
        {
            //destnIPAddress = entry.AddressList.

            return destnIPAddress;
        }

/*
 * Getters and setters for the network
 */

        public List<NetworkInterface> NetworkCardList
        {
            get { return networkCardList; }
            set { this.networkCardList = value; }
        }

        public int LocalPort
        {
            get { return localPort; }
            set { this.localPort = value; }
        }

        public int DestnPort
        {
            get { return destnPort; }
            set { this.destnPort = value; }
        }

        public IPAddress LocalIPAddress
        {
            get { return localIPAddress; }
            set { this.localIPAddress = value; }
        }

    }
}
