using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace KineticControl
{
    public class PDS150e : PDS
    {
        private readonly Network _network;
        private IPEndPoint _endPoint;
        

        private readonly ColorData _data1;

        public PDS150e(Network network, IPEndPoint endPoint)
        {
            _network = network;
            _endPoint = endPoint;
            _data1 = new ColorData(this, IntialHex,new LightType(){Name = "Accent List", NoLights = 50, Spacing=14}, 5);
        }

        public void UpdateSystem()
        {
            _network.SendUpdate(_endPoint, _data1);
        }

        public string getType()
        {
            return "PDS150e";
        }

        public IPEndPoint EndPoint
        {
            get { return _endPoint; }
            set { _endPoint = value; }
        }


        public ColorData Fixture { get { return _data1; } }

        private const String _initalHex = //"0401dc4a01000801000000000000000002ef00000002f0ff";
                                            "0401dc4a010001010000000000000000ffffffff00";

        public static byte[] IntialHex
        {
            get { return HexStrings.DecodeString(_initalHex); }
        }

        public List<ColorData> AllColorData
        {
            get { return new List<ColorData>(){_data1}; }
        }
    }
}
