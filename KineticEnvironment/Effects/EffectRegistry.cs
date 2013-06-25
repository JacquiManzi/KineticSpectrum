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
                                                     Sweep.Attributes
                                                 };

            _effectAttributes = new Dictionary<string, EffectAttributes>(effects.ToDictionary(e=>e.Name));
            
        }

        private static readonly Dictionary<string, EffectAttributes> _effectAttributes = new Dictionary<string, EffectAttributes>();
        

        public static IList<string> Effects
        {
            get { return new List<string>(_effectAttributes.Keys); }
        }

        public static IList<PropertyDefinition> GetProperties(string effectName)
        {
            return new List<PropertyDefinition>(_effectAttributes[effectName].PropertyMap.Values);
        }
    }
}
