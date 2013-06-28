using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevKitt.KS.KineticEnvironment.Effects
{
    public class EffectRegistry
    {
        static EffectRegistry()
        {
            List<EffectAttributes> effects = new List<EffectAttributes>
                                                 {
                                                     Sweep.Attributes,
                                                     Pulse.Attributes
                                                 };

            EffectAttributes = new Dictionary<string, EffectAttributes>(effects.ToDictionary(e=>e.Name));
        }

        public static readonly Dictionary<string, EffectAttributes> EffectAttributes;
        

        public static IList<string> Effects
        {
            get { return new List<string>(EffectAttributes.Keys); }
        }

        public static IList<PropertyDefinition> GetProperties(string effectName)
        {
            return new List<PropertyDefinition>(EffectAttributes[effectName].PropertyMap.Values);
        }
    }
}
