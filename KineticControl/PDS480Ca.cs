using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace KineticControl
{
    class PDS480Ca : PDSca
    {
        public PDS480Ca() : base(InitHex)
        { }

        public PDS480Ca(Network network, IPEndPoint endPoint) : base(network, endPoint, InitHex) 
        { }

        public override string getType()
        {
            return "PDS60ca";
        }

        private const String INIT_HEX_A = "0401dc4a0100080100000000ffffffff0";
        private const String INIT_HEX_B = "df000000020000";

        private static String HexForPort(int portNo)
        {
            return INIT_HEX_A + portNo + INIT_HEX_B;
        }

        private static IEnumerable<String> InitHex
        {
            get { return Enumerable.Range(1, 8).Select(HexForPort); }
        }
    }
}
