using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace KineticControl
{
    public interface PDS
    {
        IList<ColorData> AllColorData { get; }
        void UpdateSystem();
        string getType();

        IPEndPoint EndPoint { get; }

        ColorData this[int portNo] { get; }
    }

    public class PDSAddressComparitor : IEqualityComparer<PDS>
    {
        public bool Equals(PDS x, PDS y)
        {
            if (x.EndPoint == null)
                return y.EndPoint == null;
            return x.EndPoint.Address.ToString() == y.EndPoint.Address.ToString();
        }

        public int GetHashCode(PDS obj)
        {
            return obj.EndPoint.Address.ToString().GetHashCode();
        }
    }
}
