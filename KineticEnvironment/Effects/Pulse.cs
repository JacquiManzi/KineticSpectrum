using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using KineticControl;
using RevKitt.KS.KineticEnvironment.Effects.ColorEffects;
using RevKitt.KS.KineticEnvironment.Effects.Order;
using RevKitt.KS.KineticEnvironment.Scenes;
using RevKitt.KS.KineticEnvironment.Tweening;

namespace RevKitt.KS.KineticEnvironment.Effects
{
    class Pulse : AbstractLinearEffect
    {
        private const string BackgroundEffectName = "Background";
        private const string PulseEffectName = "Pulse";
        private const string WidthName = "Width";

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
            _backgroundColor = properties.GetColorEffect(BackgroundEffectName);
            _pulseColor = properties.GetColorEffect(PulseEffectName);
            _width = properties.GetFloat(WidthName);
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

        private static void ApplyColors(IEnumerable<IEffect> colorEffect, int numCycles)
        {

            colorEffect = colorEffect.ToList();
            IColorEffect startEffect = ((Pulse)(colorEffect.First()))._backgroundColor;
            IColorEffect endEffect = ((Pulse)(colorEffect.First()))._pulseColor;
            
            List<Color> startColors = new List<Color>();
            List<Color> endColors = new List<Color>();
            Patterns patt = new Patterns();
            startColors.Add(patt.RandomColor());

            for (int i = 0; i < (numCycles - 1); i++)
            {
                Color next = patt.RandomColor();
                startColors.Add(next);
                endColors.Add(next);
            }
            endColors.Add(patt.RandomColor());
            if (startEffect.GetType() == typeof(ColorFade))
                startColors.Add(patt.RandomColor());
            if (endEffect.GetType() == typeof(ColorFade))
                endColors.Add(patt.RandomColor());

            foreach (var effect in colorEffect)
            {
                ((Pulse)effect)._backgroundColor.Colors = startColors;
                ((Pulse)effect)._pulseColor.Colors = endColors;
            }
        }

        public static readonly EffectAttributes Attributes = new EffectAttributes(EffectName, PulseFactory,ApplyColors,
                        new List<PropertyDefinition>(DefaultDefs)
                            {
                                new PropertyDefinition(BackgroundEffectName, EffectPropertyTypes.ColorEffect, ColorEffectDefinition.RedFixed),
                                new PropertyDefinition(PulseEffectName, EffectPropertyTypes.ColorEffect, ColorEffectDefinition.GreenFixed),
                                new PropertyDefinition(WidthName, EffectPropertyTypes.Float, 6)
                            });
    }
    
}
