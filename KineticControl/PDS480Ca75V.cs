using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace KineticControl
{
    class PDS480Ca75V : PDSca
    {
        public PDS480Ca75V()
            : base(InitHex, LightType.Panel)
        { }

        public PDS480Ca75V(Network network, IPEndPoint endPoint)
            : base(network, endPoint, InitHex, LightType.Panel)
        { }

        public override string getType()
        {
            return "PDS480Ca75V";
        }

        private const String INIT_HEX_A = "0401dc4a0100080100000000ffffffff";
        private const String INIT_HEX_B = "df000000020000";

        private static String HexForPort(int portNo)
        {
            string hextPortVal = portNo.ToString("X2");
            return INIT_HEX_A + hextPortVal + INIT_HEX_B;
        }

        private static IEnumerable<String> InitHex
        {
            get { return Enumerable.Range(1, 16).Select(HexForPort); }
        }
    }
}
