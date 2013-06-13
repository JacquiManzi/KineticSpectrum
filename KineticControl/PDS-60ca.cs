using System;
using System.Collections.Generic;
using System.Net;

namespace KineticControl
{
    public class PDS60Ca : PDSca
    {
        public PDS60Ca() : base(InitHex)
        { }

        public PDS60Ca(Network network, IPEndPoint endPoint) : base(network, endPoint, InitHex) 
        { }

        public void SetColorData1(LightType lightType)
        {
            SetColorData(new List<LightType>{lightType, AllColorData[1].LightType});
        }

        public void SetColorData2(LightType lightType)
        {
            SetColorData(new List<LightType>{AllColorData[0].LightType, lightType});
        }

        public override string getType()
        {
            return "PDS60ca";
        }

        private const String INITIAL_HEX =  "0401dc4a01000801000000000000000002ef00000002f0ff";
        private const String INITIAL_HEX2 = //"0401dc4a010008010000000000000000011b00000002f0ff";
                                           "0401dc4a010001010000000000000000ffffffff00";
        private static IEnumerable<string> InitHex
        {
            get { return new List<string>{INITIAL_HEX, INITIAL_HEX2};}
        }
    }
}
