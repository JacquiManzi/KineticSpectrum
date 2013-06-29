using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RevKitt.KS.KineticEnvironment.Effects.ColorEffects;
using RevKitt.KS.KineticEnvironment.Effects.Order;
using RevKitt.KS.KineticEnvironment.Scenes;
using RevKitt.KS.KineticEnvironment.Tweening;

namespace RevKitt.KS.KineticEnvironment.Effects
{
    class Pulse : AbstractLinearEffect
    {
        public const string EffectName = "Pulse";

        private IColorEffect _backgroundColor;
        private IColorEffect _pulseColor;
        private double _width;

        public Pulse(Group group) : base(group) { }

        public static EffectFactroy PulseFactory = g=> new Pulse(g);

        public override string Name { get { return EffectName; } }


        protected override void ApplyProperties(EffectProperties properties)
        {
            base.ApplyProperties(properties);
            _backgroundColor = properties.GetColorEffect("background");
            _pulseColor = properties.GetColorEffect("pulseColor");
            _width = properties.GetFloat("width");
        }


        protected override IColorEffect ApplyCycle(int time, LEDNode ledNode)
        {
            double pos = Tween.GetValue(time);

            double rangeStart = pos - _width/2;
            double rangeEnd = pos + _width/2;
            double ledPos = Ordering.GetLEDPosition(ledNode);


            if(ledPos < rangeStart || ledPos > rangeEnd)
                return _backgroundColor;

            return _pulseColor;

        }

        public static readonly EffectAttributes Attributes = new EffectAttributes(EffectName, PulseFactory,
                        new List<PropertyDefinition>(DefaultDefs)
                            {
                                new PropertyDefinition("background", EffectPropertyTypes.ColorEffect, ColorEffectDefinition.DefaultFixed),
                                new PropertyDefinition("pulseColor", EffectPropertyTypes.ColorEffect, ColorEffectDefinition.DefaultFixed),
                                new PropertyDefinition("width", EffectPropertyTypes.Float, 6)
                            });
    }
    
}
