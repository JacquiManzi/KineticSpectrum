using System;
using System.Net.NetworkInformation;
using System.Windows.Media;

namespace KineticControl
{
    class RunnTest
    {
        public static void Main()
        {
            NetworkInterface card;
            PDS60ca powerSupply = new PDS60ca();
            Network network = new Network(powerSupply.ColorAddress);

            network.RetrieveNetworkCards();
            foreach (var netCard in network.NetworkCardList)
            {
                if (netCard.Name.Equals("Local Area Connection"))
                {
                    card = netCard;
                    network.NetworkCard = card;
                    network.InitializeLocalIP();
                    network.InitializeLocalPort();

                    break;
                }
            }
           
            network.BroadCast();
            powerSupply.setColor(Colors.Crimson, 0, network);
           
        }

    }
}
