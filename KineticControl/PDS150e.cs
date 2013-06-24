using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace KineticControl
{
    public class PDS150e : PDS, IEquatable<PDS150e>
    {
        private readonly Network _network;
        private readonly IPEndPoint _endPoint;
        

        private readonly ColorData _data1;

        public PDS150e(Network network, IPEndPoint endPoint)
        {
            _network = network;
            _endPoint = endPoint;
            _data1 = new ColorData(this, IntialHex,new LightType{Name = "Accent List", NoLights = 50, Spacing=14}, 5);
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
        }

        public bool Equals(PDS150e other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(_endPoint, other._endPoint);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((PDS150e) obj);
        }

        public override int GetHashCode()
        {
            return (_endPoint != null ? _endPoint.GetHashCode() : 0);
        }

        public static bool operator ==(PDS150e left, PDS150e right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(PDS150e left, PDS150e right)
        {
            return !Equals(left, right);
        }


        public ColorData Fixture { get { return _data1; } }

        private const String _initalHex = //"0401dc4a01000801000000000000000002ef00000002f0ff";
                                            "0401dc4a010001010000000000000000ffffffff00";

        public static byte[] IntialHex
        {
            get { return HexStrings.DecodeString(_initalHex); }
        }

        public IList<ColorData> AllColorData
        {
            get { return new List<ColorData>{_data1}; }
        }

        public ColorData this[int portNo]
        {
            get { return AllColorData[portNo]; }
        }
    }
}
