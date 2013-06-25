using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevKitt.KS.KineticEnvironment.Effects
{


    class EffectAttributes
    {
        public readonly string Name;
        public readonly IDictionary<string, PropertyDefinition> PropertyMap;

        public EffectAttributes(string name, IEnumerable<PropertyDefinition> properties )
        {
            Name = name;
            PropertyMap = properties.ToDictionary(p => p.Name);
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
