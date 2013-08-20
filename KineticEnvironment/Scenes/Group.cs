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
            if(name == null)
                throw new ArgumentException("Group name cannot be null");
            if(name.Length == 0)
                throw new ArgumentException("Group must have a name. The empty string is not a valid group name");

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
