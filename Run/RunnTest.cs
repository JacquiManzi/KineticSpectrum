using System.Net.NetworkInformation;

namespace KineticControl
{
    class RunnTest
    {
        private static Patterns pattern = new Patterns();
        public static void Main()
        {
            NetworkInterface card;
            Network network = new Network();
            Network network2 = new Network();

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

                    network2.NetworkCard = card;
                    network2.InitializeLocalIP();
                    network2.InitializeLocalPort();

                    break;
                }
            }
           
           network.BroadCast();
            PDS60ca powerSupply = network.PDSs[0];
           for (int i = 0; i < pattern.GetColors().Count; i++)
           {
               for (int j = 0; j < 50; j++)
               {
                   powerSupply.FixtureOne[j] = pattern.GetColors()[i];
                   powerSupply.FixtureTwo[j] = pattern.GetColors()[i];
                   powerSupply.UpdateSystem();
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
