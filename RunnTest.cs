using System;

public class RunnTest
{
    class RunnTest
    {
        public static void Main()
        {
            NetworkInterface card;
            Network network = new Network();

            network.BroadCast();
            foreach (var netCard in network.NetworkCardList)
            {
                if (netCard.Name.ToString().Contains("Ethernet"))
                {
                    card = netCard;
                }
            }

        }

    }
}
