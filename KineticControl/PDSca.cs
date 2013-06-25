using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace KineticControl
{
    public abstract class PDSca : PDS, IEquatable<PDSca>
    {
        private readonly Network _network;
        public IPEndPoint EndPoint { get; private set; }

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

        public bool Equals(PDSca other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(EndPoint, other.EndPoint);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((PDSca) obj);
        }

        public override int GetHashCode()
        {
            return (EndPoint != null ? EndPoint.GetHashCode() : 0);
        }

        public static bool operator ==(PDSca left, PDSca right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(PDSca left, PDSca right)
        {
            return !Equals(left, right);
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

        public IList<ColorData> AllColorData
        {
            get { return new List<ColorData>(_ports); }
        }

        public ColorData this[int portNo]
        {
            get { return _ports[portNo]; }
        }
    }
}
