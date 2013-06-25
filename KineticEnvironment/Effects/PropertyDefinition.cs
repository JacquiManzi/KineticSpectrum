using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KineticControl;
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

        public static readonly PropertyDefinition RepeatCount = new PropertyDefinition("repeatCount", EffectPropertyTypes.Int, 1);
        public static readonly PropertyDefinition RepeatMethod = new PropertyDefinition("repeatMethod", EffectPropertyTypes.RepeatMethod, RepeatMethods.Reverse);
        public static readonly PropertyDefinition Durration = new PropertyDefinition("durration", EffectPropertyTypes.Time, 5);
        public static readonly PropertyDefinition Ordering = new PropertyDefinition("ordering", EffectPropertyTypes.Ordering, new[]{"Group","Forward"});
        public static readonly PropertyDefinition Easing = new PropertyDefinition("tween", EffectPropertyTypes.Easing, Easings.Linear.Name );
    }
}
