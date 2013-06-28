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
    public class Group
    {
        private readonly string _name;
        private readonly IList<LEDNode> _ledNodes; 


        public Group(string name, IEnumerable<LEDNode> ledNodes )
        {
            _name = name;
            _ledNodes = new List<LEDNode>(ledNodes).AsReadOnly();
        }

        [JsonProperty]
        public string Name { get { return _name; } }

        [JsonProperty]
        public IList<LightAddress> Lights { get { return new List<LightAddress>(_ledNodes.Select(l => l.Address)); } } 

        public IList<LEDNode> LEDNodes
        {
            get { return _ledNodes; }
        } 
    }
}
