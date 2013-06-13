using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace KineticControl
{
    public abstract class PDSca : PDS
    {
        private readonly Network _network;
        public IPEndPoint EndPoint { get; set; }

        private List<ColorData> _ports;
        private readonly List<String>    _initialHexs; 
 

        public PDSca( IEnumerable<String> initialHexs )
        {
            EndPoint = new IPEndPoint(IPAddress.Any, 6038);
            _network = Network.GetInstance();
            _ports = new List<ColorData>();
            _initialHexs = new List<string>(initialHexs);
            foreach (var initialHex in _initialHexs)
            {
                byte[] hexBytes = HexStrings.DecodeString(initialHex);
                _ports.Add(new ColorData(this, hexBytes, LightType.Short));
            }
        }

        public PDSca( Network network, IPEndPoint endPoint, IEnumerable<String> initialHexs  ) : this(initialHexs)
        {
            _network = network;
            EndPoint = endPoint;
        }

        public void SetColorData(IList<LightType> lightTypes )
        {
            var rebuild = new Func<String, LightType, ColorData>((initHex, type) =>
                                   new ColorData(this, HexStrings.DecodeString(initHex), type));
            _ports = new List<ColorData>(Enumerable.Zip(_initialHexs, lightTypes, rebuild));
        }


        public void UpdateSystem()
        {
            _ports.ForEach(port => _network.SendUpdate(EndPoint, port));
        }

        public abstract String getType();

        public override string ToString()
        {
            return getType() + EndPoint.Address;
        }

        public List<ColorData> AllColorData
        {
            get { return new List<ColorData>(_ports); }
        }
    }
}
