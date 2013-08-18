using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KineticControl;
using RevKitt.KS.KineticEnvironment.Effects.Order;
using RevKitt.KS.KineticEnvironment.Tweening;

namespace RevKitt.KS.KineticEnvironment.Effects
{
    public class PropertyDefinition
    {
        public readonly string Name;
        public readonly string PropertyType;
        public readonly object DefaultValue;
        public readonly bool   IsOptional;

        public PropertyDefinition([NotNull] string propertyName, [NotNull] string propertyType, [NotNull] object defaultValue, bool isOptional = false)
        {
            Name = propertyName;
            PropertyType = propertyType;
            DefaultValue = defaultValue;
            IsOptional = isOptional;
        }

        public static readonly PropertyDefinition RepeatCount = new PropertyDefinition("Repeat Count", EffectPropertyTypes.Int, 1);
        public static readonly PropertyDefinition RepeatMethod = new PropertyDefinition("Repeat Method", EffectPropertyTypes.RepeatMethod, RepeatMethods.Reverse);
        public static readonly PropertyDefinition Duration = new PropertyDefinition("Duration", EffectPropertyTypes.Time, 5);
        public static readonly PropertyDefinition Ordering = new PropertyDefinition("Ordering", EffectPropertyTypes.Ordering, new Dictionary<string, string>{{Orderings.OrderingTypeKey, OrderingTypes.Group},{Orderings.OrderingKey,GroupOrderingTypes.Forward}});
        public static readonly PropertyDefinition Easing = new PropertyDefinition("Tween", EffectPropertyTypes.Easing, Easings.Linear.Name );
    }
}
