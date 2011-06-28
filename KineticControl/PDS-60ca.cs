using System;
using System.Collections.Generic;
using System.Net;

namespace KineticControl
{
    public class PDS60ca : PDS
    {
        private readonly Network _network;
        public IPEndPoint EndPoint { get; set; }


        public ColorData Data1 { get; private set; }
        public ColorData Data2 { get; private set; }

        public PDS60ca()
        {
            _network = Network.GetInstance();
            Data1 = new ColorData(InitialHex, LightType.None);
            Data2 = new ColorData(InitialHex2, LightType.None);
            EndPoint = new IPEndPoint(IPAddress.IPv6None, 6038);
        }

        public PDS60ca(Network network, IPEndPoint endPoint)
        {
            _network = network;
            EndPoint = endPoint;
            Data1 = new ColorData(InitialHex, LightType.Short);
            Data2 = new ColorData(InitialHex2, LightType.Short);
        }

        public void SetColorData1(LightType lightType)
        {
            Data1 = new ColorData(InitialHex, lightType);
        }

        public void SetColorData2(LightType lightType)
        {
            Data2 = new ColorData(InitialHex2, lightType); 
        }

        public void UpdateSystem()
        {
            _network.SendUpdate(EndPoint, Data1);
            _network.SendUpdate(EndPoint, Data2);
        }

        public string getType()
        {
            return "PDS60ca";
        }

        public override string ToString()
        {
            return getType() + EndPoint.Address;
        }

        private const String INITAL_HEX =  "0401dc4a01000801000000000000000002ef00000002f0ff";
        private const String INTIAL_HEX2 = "0401dc4a010008010000000000000000011b00000002f0ff";

        public static byte[] InitialHex
        {
            get { return HexStrings.DecodeString(INITAL_HEX); }
        }

        public static byte[] InitialHex2
        {
            get { return HexStrings.DecodeString(INTIAL_HEX2); }
        }


        public IList<ColorData> AllColorData
        {
            get { return new List<ColorData>{Data1, Data2}; }
        }
    }
}
