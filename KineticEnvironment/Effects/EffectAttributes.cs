using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RevKitt.KS.KineticEnvironment.Effects.ColorEffects;
using RevKitt.KS.KineticEnvironment.Scenes;

namespace RevKitt.KS.KineticEnvironment.Effects
{

    public delegate IEffect EffectFactroy(Group group);

    public delegate void ApplyColors(IEnumerable<IEffect> effect, int numCycles);

    public class EffectAttributes
    {
        public readonly string Name;
        public readonly IDictionary<string, PropertyDefinition> PropertyMap;
        private readonly EffectFactroy _factory;
        private readonly ApplyColors _apply;

        public EffectAttributes(string name, EffectFactroy factory, ApplyColors apply, IEnumerable<PropertyDefinition> properties )
        {
            Name = name;
            PropertyMap = properties.ToDictionary(p => p.Name);
            _factory = factory;
            _apply = apply;
        }

        public EffectFactroy EffectFactory
        {
            get { return _factory; }
        }

        public ApplyColors ApplyColors
        {
            get { return _apply; }
        }

        public IEnumerable<string> PropertyNames()
        {
            return PropertyMap.Keys;
        }

        public PropertyDefinition this[string propertyName]
        {
            get { return PropertyMap[propertyName]; }
        }

        public object GetDefaultValue(string propertyName)
        {
            return PropertyMap[propertyName].DefaultValue;
        }

        public bool IsOptional(string propertyName)
        {
            return PropertyMap[propertyName].IsOptional;
        }

        public string GetEffectPropertyType(string propertyName)
        {
            return PropertyMap[propertyName].PropertyType;
        }
    }
}
