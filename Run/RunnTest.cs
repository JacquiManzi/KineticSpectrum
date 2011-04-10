using System;
using System.Net.NetworkInformation;
using System.Windows.Media;

namespace KineticControl
{
    class RunnTest
    {
        public static void Main()
        {
            Network network = new Network();

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
                if ( netCard.Name.Equals("Local Area Connection") )
                {
                    network.NetworkCard = netCard;
                    network.InitializeLocalIP();
                    network.InitializeLocalPort();

                    break;
                }
            }
           
            Color[] colors = new Color[50];
            for (int i = 0; i < 50; i++)
                colors[i] = Colors.Blue;
            network.BroadCast();
            network.SendUpdate(colors);
        }

    }
}
