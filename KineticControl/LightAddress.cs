using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KineticControl
{
    public class LightAddress : IEquatable<LightAddress>, IComparable<LightAddress>
    {
        private readonly int _fixtureNumber;
        private readonly int _portNumber;
        private readonly int _lightNumber;
        
        public static readonly LightAddress Unknown = new LightAddress(-1,-1,-1);

        public LightAddress(int fixtureNumber, int portNumber, int lightNumber)
        {
            _fixtureNumber = fixtureNumber;
            _portNumber = portNumber;
            _lightNumber = lightNumber;
        }

        public int LightNo
        {
            get { return _lightNumber; }
        }

        public int PortNo
        {
            get { return _portNumber; }
        }

        public int FixtureNo
        {
            get { return _fixtureNumber; }
        }

        public bool IsUnknown
        {
            get { return FixtureNo==-1 && PortNo==-1; }
        }

        private const string SplitChar = "/";
        private const string UnknownString = "?";

        public static bool TryParse(string addressString, out LightAddress lightAddress)
        {
            lightAddress = null;
            if(UnknownString.Equals(addressString))
            {
                lightAddress = Unknown;
                return true;
            }
            string[] split = addressString.Split(SplitChar.ToCharArray());
            if(split.Count() != 3) return false;
            int fixtureNo, portNo, lightNo;
            bool success = int.TryParse(split[0], out fixtureNo);
            success &= int.TryParse(split[1], out portNo);
            success &= int.TryParse(split[2], out lightNo);

            if(success)
            {
                lightAddress = new LightAddress(fixtureNo, portNo, lightNo);
                return true;
            }
            return false;
        }

        // Equals and HashCode Operators
        public int CompareTo(LightAddress other)
        {
            int comp = _fixtureNumber - other._fixtureNumber;
            if (comp == 0) 
                comp = _portNumber - other._portNumber;
            if(comp == 0)
                comp = _lightNumber - other._lightNumber;
            return comp;
        }

        public override String ToString()
        {
            return IsUnknown ? "?" : _fixtureNumber + "-" + _portNumber + "-" + _lightNumber;
        }

        public bool Equals(LightAddress other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return _fixtureNumber == other._fixtureNumber && _lightNumber == other._lightNumber &&
                   _portNumber == other._portNumber;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashcode = (_fixtureNumber * 397) ^ _lightNumber;
                return (hashcode*397) ^ _portNumber;
            }
        }

        public static bool operator ==(LightAddress left, LightAddress right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(LightAddress left, LightAddress right)
        {
            return !Equals(left, right);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((LightAddress) obj);
        }
    }
}
