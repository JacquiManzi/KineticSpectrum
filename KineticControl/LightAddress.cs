using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KineticControl
{
    public class LightAddress : IEquatable<LightAddress>, IComparable<LightAddress>
    {
        private readonly int _fixtureNumber;
        private readonly int _lightNumber;
        public static readonly LightAddress Unknown = new LightAddress(-1,-1);

        public LightAddress(int fixtureNumber, int lightNumber)
        {
            _fixtureNumber = fixtureNumber;
            _lightNumber = lightNumber;
        }

        public int LightNo
        {
            get { return _lightNumber; }
        }

        public int FixtureNo
        {
            get { return _fixtureNumber; }
        }

        public bool IsUnknown
        {
            get { return Equals(Unknown); }
        }

        // Equals and HashCode Operators
        public int CompareTo(LightAddress other)
        {
            int comp = _fixtureNumber - other._fixtureNumber;
            if(comp == 0)
                comp = _lightNumber - other._lightNumber;
            return comp;
        }

        public override String ToString()
        {
            return IsUnknown ? "?" : _fixtureNumber + "-" + _lightNumber;
        }

        public bool Equals(LightAddress other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return _fixtureNumber == other._fixtureNumber && _lightNumber == other._lightNumber;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (_fixtureNumber * 397) ^ _lightNumber;
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
            if (obj.GetType() != this.GetType()) return false;
            return Equals((LightAddress) obj);
        }
    }
}
