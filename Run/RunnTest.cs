using System.Net.NetworkInformation;
using System.Threading;

namespace KineticControl
{
    class RunnTest
    {
        private static Patterns pattern = new Patterns();
        public static void Main()
        {
            NetworkInterface card;
            Network network = new Network();

            network.RetrieveNetworkCards();
			//            var cards = Network.RetrieveNetworkCards();
//
//            while (true)
//            {
//                Console.WriteLine("Select a Network card:");
//                for (int i = 0; i < cards.Count; i++)
//                {
//                    Console.WriteLine("[{0}] {1}", i, cards[i].Name);
//                }
//
//                Console.Write("Selection: ");
//                String read = Console.ReadLine();
//                int num;
//                if(int.TryParse(read, out num) && num >=0 && num < cards.Count)
//                {
//                    network.NetworkCard = cards[num];
//                    break;
//                }
//            }
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
           
           //network.BroadCast();
           PDS powerSupply = network.PDSs[0];// PDSs[0];
           for (int i = 0; i < pattern.GetColors().Count; i++)
           {
               for (int j = 0; j < 50; j++)
               {
                   powerSupply.AllColorData[0][j] = pattern.GetColors()[i];
                   powerSupply.UpdateSystem();
                   Thread.Sleep(30);
                   Thread.Sleep(100);
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
