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
}
