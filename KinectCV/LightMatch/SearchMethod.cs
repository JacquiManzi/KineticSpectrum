using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RevKitt.ks.KinectCV.LightMatch
{
    interface SearchMethod
    {
        IEnumerable<Light> SearchUpdate(ISet<Light> missingLights);

        bool Done { get; }
    }
}
