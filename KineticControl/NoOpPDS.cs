using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace KineticControl
{
    class NoOpPDS : PDS
    {

        public NoOpPDS(int[] lightsPerPort, IPAddress ipAddress)
        {
            EndPoint = new IPEndPoint(ipAddress, 6038);
            List<ColorData> allColorData = new List<ColorData>(lightsPerPort.Count());
            foreach (var i in lightsPerPort)
            {
                allColorData.Add(new ColorData(this, new byte[0],LightType.Long ));
            }
            AllColorData = allColorData.AsReadOnly();
            
        }
        public IList<ColorData> AllColorData { get; private set; }

        public void UpdateSystem()
        {
            //no-op
        }

        public string getType()
        {
            return "PDS-Stub";
        }

        public IPEndPoint EndPoint { get; private set; }

        public ColorData this[int portNo]
        {
            get { return AllColorData[portNo]; }
        }
    }
}
