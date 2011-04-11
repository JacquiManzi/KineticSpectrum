using System;
using System.Net;
using System.Net.NetworkInformation;
using System.Windows.Media;

namespace KineticControl
{
    class RunnTest
    {
        private static Patterns pattern = new Patterns();
        public static void Main()
        {
            NetworkInterface card;
            PDS60ca powerSupply = new PDS60ca();
            Network network = new Network(powerSupply.ColorAddress);
            Network network2 = new Network(powerSupply.ColorAddress);

            network.RetrieveNetworkCards();
            foreach (var netCard in network.NetworkCardList)
            {
                if (netCard.Name.Equals("Local Area Connection"))
                {
                    card = netCard;
                    network.NetworkCard = card;
                    network.InitializeLocalIP();
                    network.InitializeLocalPort();

                    network2.NetworkCard = card;
                    network2.InitializeLocalIP();
                    network2.InitializeLocalPort();

                    break;
                }
            }
           
           network.BroadCast();
           network2.ipEndpoint = new IPEndPoint(IPAddress.Any, 5022);
           network2.BroadCast();
           //network.FindPowerSupply();
           for (int i = 0; i < pattern.GetColors().Count; i++)
           {
               for (int j = 0; j < 50; j++)
               {

                   powerSupply.setColor(pattern.GetColors()[i], j, network, 1);
                   
               }
           }

//           for (int i = 0; i < pattern.GetColors().Count; i++)
//           {
//               for (int j = 0; j < 50; j++)
//               {
//                   powerSupply.setColor(pattern.GetColors()[i], j, network2, 2);
//               }
//           }

        }

    }
}