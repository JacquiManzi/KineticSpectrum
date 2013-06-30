using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace RevKitt.KS.KineticEnvironment.Tweening
{
    public delegate double EasingFunc(double inputPortion);

    [JsonObject(MemberSerialization.OptIn)]
    public interface IEasing
    {
        [JsonProperty]
        string Name { get; }

        EasingFunc In { get; }
        EasingFunc Out { get; }
        EasingFunc OutIn { get; } 
    }
}
