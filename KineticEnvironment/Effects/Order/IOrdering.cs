using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RevKitt.KS.KineticEnvironment.Scenes;

namespace RevKitt.KS.KineticEnvironment.Effects.Order
{
    [JsonObject(MemberSerialization.OptIn)]
    public interface IOrdering
    {

        IGroup Group { get; set; } 
        [JsonProperty]
        string Type { get; }
        [JsonProperty]
        string Ordering { get; }
        bool Runnable { get; }

        double GetLEDPosition(LEDNode ledNode);
        double GetMax();
        double GetMin();
        double GetAngle(LEDNode ledNode);
    }

}
