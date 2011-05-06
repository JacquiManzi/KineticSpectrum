using System;
using System.Collections.Generic;
using System.Net;

namespace KineticControl
{
    public class PDS60ca : PDS
    {
        private readonly Network _network;
        private readonly IPEndPoint _endPoint;
        

        private readonly ColorData _data1;
        private readonly ColorData _data2;

        public PDS60ca(Network network, IPEndPoint endPoint)
        {
            _network = network;
            _endPoint = endPoint;
            _data1 = new ColorData(IntialHex, 50);
            _data2 = new ColorData(IntialHex2, 50);
        }

        public void UpdateSystem()
        {
            _network.SendUpdate(_endPoint, _data1);
            _network.SendUpdate(_endPoint, _data2);
        }

        public string getType()
        {
            return "PDS60ca";
        }


        public ColorData FixtureOne { get { return _data1; } }
        public ColorData FixtureTwo { get { return _data2; } }
        private const String _initalHex =  "0401dc4a01000801000000000000000002ef00000002f0ff";
        private const String _intialHex2 = "0401dc4a010008010000000000000000011b00000002f0ff";

        public static byte[] IntialHex
        {
            get { return HexStrings.DecodeString(_initalHex); }
        }

        public static byte[] IntialHex2
        {
            get { return HexStrings.DecodeString(_intialHex2); }
        }


        public IList<ColorData> AllColorData
        {
            get { return new List<ColorData>(){_data1, _data2}; }
        }
    }
}
