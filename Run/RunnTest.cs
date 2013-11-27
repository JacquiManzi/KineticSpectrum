using System;
using System.IO;
using System.Net.NetworkInformation;
using System.Threading;
using System.Windows.Media;
using RevKitt.KS.KineticEnvironment;

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

            network.SetInterface("Local Area Connection 2");




            int colorCount = pattern.GetColors().Count;
           Color[,] pairs = new Color[colorCount*colorCount, 2]; 
           network.BroadCast();
           for (int i = 0; i < pattern.GetColors().Count; i++)
           {
               for (int j = 0; j < pattern.GetColors().Count; j++)
               {
                   int index = i*colorCount + j;
                   pairs[index, 0] = pattern.GetColors()[i];
                   pairs[index, 1] = pattern.GetColors()[j];
               }
           }

            string path = @"colorData.txt";
            File.Delete(path);
            StreamWriter writer = File.AppendText(path);
            ConsoleKeyInfo key;
            int cursor = 0;
            do
            {
                key = Console.ReadKey(true);
                if (key.Key == ConsoleKey.RightArrow)
                {
                    cursor = (cursor + 1)%pairs.GetLength(0);
                }
                if (key.Key == ConsoleKey.UpArrow)
                {
                    cursor = (cursor + colorCount)%pairs.GetLength(0);
                }
                if (key.Key == ConsoleKey.DownArrow)
                {
                    cursor = (cursor - colorCount);
                    if (cursor < 0)
                        cursor = colorCount + cursor;
                }
                if (key.Key == ConsoleKey.LeftArrow)
                {
                    cursor = (cursor - 1);
                    if (cursor < 0)
                        cursor = colorCount - 1;
                }
                foreach (PDS pds in network.PDSs)
                {
                    foreach (var colorData in pds.AllColorData)
                    {
                        for (int i = 0; i < colorData.Count; i++)
                        {
                            colorData[i] = i < colorData.Count/2 ? pairs[cursor, 0] : pairs[cursor, 1];
                        }
                    }
                    pds.UpdateSystem();
                }

                if (key.Key == ConsoleKey.Y)
                {
                    writer.WriteLine("["+ColorUtil.ToInt(pairs[cursor,0])+"," 
                        + ColorUtil.ToInt(pairs[cursor,1]) + "],");
                    writer.Flush();
                }
            } while (key.Key != ConsoleKey.Escape);

            writer.Flush();
            writer.Close();
        }

    }
}
