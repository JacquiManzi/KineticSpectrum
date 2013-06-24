using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KineticControl;

namespace RevKitt.KS.KineticEnvironment.Scenes
{
    class Group
    {
        private readonly string _name;
        private readonly IList<LightAddress> _lights; 


        public Group(string name, IEnumerable<LightAddress> lights )
        {
            _name = name;
            _lights = new List<LightAddress>(lights).AsReadOnly();
        }

        public string Name { get { return _name; } }

        public IList<LightAddress> Lights { get { return _lights; } } 
    }
}
