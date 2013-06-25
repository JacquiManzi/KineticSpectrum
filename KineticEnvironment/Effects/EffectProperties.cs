using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RevKitt.KS.KineticEnvironment.Effects.ColorEffects;

namespace RevKitt.KS.KineticEnvironment.Effects
{
    class EffectProperties
    {

        public EffectProperties() {}

        public IDictionary<string, object> Properties { get; private set; }

        public object this[string propertyName]
        {
            get { return Properties[propertyName]; }
            set { Properties[propertyName] = value; }
        }

        public IColorEffect GetColorEffect(string propertyName)
        {
            return (IColorEffect) this[propertyName];
        }

    }
}
