using System.Net;

namespace KineticControl
{
    public class PDS60ca
    {
        private readonly Network _network;
        private readonly IPEndPoint _endPoint;
        

        private readonly ColorData _data1;
        private readonly ColorData _data2;

        public PDS60ca(Network network, IPEndPoint endPoint)
        {
            _network = network;
            _endPoint = endPoint;
            _data1 = new ColorData(HexStrings.IntialHex, 50);
            _data2 = new ColorData(HexStrings.IntialHex2, 50);
        }

        public void UpdateSystem()
        {
            _network.SendUpdate(_endPoint, _data1);
            _network.SendUpdate(_endPoint, _data2);
        }


        public ColorData FixtureOne { get { return _data1; } }
        public ColorData FixtureTwo { get { return _data2; } }
    }
}
