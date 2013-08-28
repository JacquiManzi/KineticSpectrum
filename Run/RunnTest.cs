using System.Net.NetworkInformation;
using System.Threading;
using System.Windows.Media;

namespace KineticControl
{
    class RunnTest
    {
        private static Patterns pattern = new Patterns();
        public static void Main()
        {
            Network network = Network.GetInstance();

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

            network.SetInterface("Wireless Network Connection");


           
           network.BroadCast();
           while (true)
           {
               for (int i = 0; i < pattern.GetColors().Count; i++)
               {
                   Color color = pattern.GetColors()[i];
                   for (int j = 0; j < 50; j++)
                   {
                       foreach (PDS pds in network.PDSs)
                       {
                           foreach (var colorData in pds.AllColorData)
                           {
                               colorData[j] = color;
                           }
                           pds.UpdateSystem();
                       }
                       Thread.Sleep(100);
                   }
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
