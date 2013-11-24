using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KineticControl;

namespace RevKitt.KS.KineticEnvironment.Scenes
{
    class GroupReference : IGroup
    {
        private readonly string _name;
        private readonly IActivatable _lightProvider;

        internal GroupReference(String name, IEnumerable<LightAddress> addresses, IActivatable lightProvider)
        {
            if(name == null)
                throw new ArgumentException("Group name cannot be null");
            if(name.Length == 0)
                throw new ArgumentException("Group must have a name. The empty string is not a valid group name");

            _name = name;
            _lightProvider = lightProvider;
            UpdateMembers(addresses);
        }

        internal void UpdateMembers(IEnumerable<LightAddress> addresses)
        {
            IEnumerable<LEDNode> availableNodes = _lightProvider.Nodes;
            IDictionary<LightAddress, LEDNode> dict = availableNodes.ToDictionary(n => n.Address);
            LEDNodes = new List<LEDNode>(addresses.Select(a => dict[a])).AsReadOnly();
        }

        public string Name { get { return _name; } }
        public IList<LightAddress> Lights { get { return LEDNodes.Select(led => led.Address).ToList(); } }
        public IList<LEDNode> LEDNodes { get; private set; }
    }
}
