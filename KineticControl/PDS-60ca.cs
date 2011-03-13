using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KineticControl
{
    class PDS60ca
    {
        //Squence needed to get the attention of the PDS-60ca
        private static String _dataOne = "0401dc4a0100090100000000a0310567";
        private static String _dataTwo = "0401dc4a0100010000000000a9fe8889";

        public static String DataOne
        {
            get { return _dataOne; }
            set { _dataOne = value;}
        }

        public static String DataTwo
        {
            get { return _dataTwo; }
            set { _dataTwo = value; }
        }

    }
}
