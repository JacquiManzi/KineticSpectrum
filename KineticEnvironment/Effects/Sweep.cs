using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RevKitt.KS.KineticEnvironment.Effects.ColorEffects;
using RevKitt.KS.KineticEnvironment.Scenes;

namespace RevKitt.KS.KineticEnvironment.Effects
{
    class Sweep : IEffect
    {
        public const string EffectName = "Sweep";

        public Sweep(EffectProperties properties, List<Group> groups)
        {
            
        }


        public string Name { get { return EffectName; } }

        public EffectProperties Properties { get; private set; }

        public static readonly EffectAttributes Attributes = new EffectAttributes(EffectName,
                        new List<PropertyDefinition>
                            {
                                PropertyDefinition.Durration,
                                new PropertyDefinition("startColor", EffectPropertyTypes.ColorEffect, ColorEffectDefinition.DefaultFixed),
                                new PropertyDefinition("endColor", EffectPropertyTypes.ColorEffect, ColorEffectDefinition.DefaultFixed),
                                PropertyDefinition.RepeatCount,
                                PropertyDefinition.RepeatMethod,
                                PropertyDefinition.Ordering,
                                PropertyDefinition.Easing
                            });
    }
}
