using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KineticControl;
using Newtonsoft.Json;


namespace RevKitt.KS.KineticEnvironment.Scenes
{
    [JsonObject(MemberSerialization.OptIn)]
    public interface IGroup
    {
        [JsonProperty]
        string Name { get; }

        [JsonProperty]
        IList<LightAddress> Lights { get; }

        IList<LEDNode> LEDNodes { get; }

        bool InGroup(LEDNode node);
    }
}
