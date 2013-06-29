using System.Collections.Generic;
using RevKitt.KS.KineticEnvironment.Effects.ColorEffects;
using RevKitt.KS.KineticEnvironment.Scenes;

namespace RevKitt.KS.KineticEnvironment.Effects
{
    class Sweep : AbstractLinearEffect
    {
        public const string EffectName = "Sweep";

       
        private IColorEffect _startColorEffect;
        private IColorEffect _endColorEffect;
       

        public Sweep(Group group) : base(group) { }

        public static EffectFactroy SweepFactory = g=>new Sweep(g);

        public override string Name { get { return EffectName; } }

        protected override void ApplyProperties(EffectProperties properties)
        {
            base.ApplyProperties(properties);
            _startColorEffect = properties.GetColorEffect("startColor");
            _endColorEffect = properties.GetColorEffect("endColor");
        }


        protected override IColorEffect ApplyCycle(int time, LEDNode ledNode)
        {
            double pos = Tween.GetValue(time);

            if (Ordering.GetLEDPosition(ledNode) <= pos)
                return _endColorEffect;
            
            return _startColorEffect;
            
        }

        public static readonly EffectAttributes Attributes = new EffectAttributes(EffectName, SweepFactory,
                        new List<PropertyDefinition>(DefaultDefs)
                            {
                                new PropertyDefinition("startColor", EffectPropertyTypes.ColorEffect, ColorEffectDefinition.DefaultFixed),
                                new PropertyDefinition("endColor", EffectPropertyTypes.ColorEffect, ColorEffectDefinition.DefaultFixed),
                            });
    }
}
