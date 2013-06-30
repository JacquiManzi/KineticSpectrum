using System.Collections.Generic;
using RevKitt.KS.KineticEnvironment.Effects.ColorEffects;
using RevKitt.KS.KineticEnvironment.Scenes;

namespace RevKitt.KS.KineticEnvironment.Effects
{
    public class Sweep : AbstractLinearEffect
    {
        private const string StartEffectName = "Sweep Start";
        private const string EndEffectName = "Sweep End";

        public const string EffectName = "Sweep";

       
        private IColorEffect _startColorEffect;
        private IColorEffect _endColorEffect;
       

        public Sweep(Group group) : base(group) { }

        public static EffectFactroy SweepFactory = g=>new Sweep(g);

        public override string Name { get { return EffectName; } }

        protected override void ApplyProperties(EffectProperties properties)
        {
            base.ApplyProperties(properties);
            _startColorEffect = properties.GetColorEffect(StartEffectName);
            _endColorEffect = properties.GetColorEffect(EndEffectName);
        }


        protected override IColorEffect ApplyCycle(int time, LEDNode ledNode)
        {
            double pos = Tween.GetValue(time);

            if (Ordering.GetLEDPosition(ledNode) <= pos)
                return _endColorEffect;
            
            return _startColorEffect;
            
        }

        public static readonly PropertyDefinition StartColor = new PropertyDefinition(StartEffectName,
                                                                                      EffectPropertyTypes.ColorEffect,
                                                                                      ColorEffectDefinition.DefaultFixed);

        public static readonly PropertyDefinition EndColor = new PropertyDefinition(EndEffectName,
                                                                                    EffectPropertyTypes.ColorEffect,
                                                                                    ColorEffectDefinition.DefaultFixed);
        public static readonly EffectAttributes Attributes = new EffectAttributes(EffectName, SweepFactory,
                        new List<PropertyDefinition>(DefaultDefs)
                            {
                                StartColor,
                                EndColor
                            });
    }
}
