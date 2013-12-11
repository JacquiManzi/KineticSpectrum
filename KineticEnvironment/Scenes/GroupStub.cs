using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KineticControl;

namespace RevKitt.KS.KineticEnvironment.Scenes
{
    public class GroupStub : IGroup
    {
        private readonly string _name;
        private readonly IList<LightAddress> _addresses; 
        public GroupStub(String name, IEnumerable<LightAddress> addresses)
        {
            _name = name;
            _addresses = addresses.ToList().AsReadOnly();
        }

        public string Name { get { return _name; } }
        public IList<LightAddress> Lights { get { return _addresses; } }

        public IList<LEDNode> LEDNodes
        {
            get { throw new NotImplementedException("This operation is not valid on a stub group"); }
        }

        public bool InGroup(LEDNode node)
        {
            throw new NotImplementedException("This operation is not valid on a stub group");
        }
    }
}
