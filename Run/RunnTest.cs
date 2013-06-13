﻿using System.Net.NetworkInformation;
using System.Threading;
using System.Windows.Media;

namespace KineticControl
{
    class RunnTest
    {
        private static Patterns pattern = new Patterns();
        public static void Main()
        {
            NetworkInterface card;
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

           network.SetInterface("Local Area Connection 2");


           
           network.BroadCast();
           PDS powerSupply = network.PDSs[0];// PDSs[0];
           while (true)
           {
               for (int i = 0; i < pattern.GetColors().Count; i++)
               {
                   Color color = pattern.GetColors()[i];
                   for (int j = 0; j < 50; j++)
                   {
                       foreach (var colorData in powerSupply.AllColorData)
                       {
                           colorData[j] = color;
                       }
                       powerSupply.UpdateSystem();
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
