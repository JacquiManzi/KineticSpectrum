using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using KineticControl;
using RevKitt.KS.KineticEnvironment.Effects.ColorEffect;
using RevKitt.KS.KineticEnvironment.Scenes;

namespace RevKitt.KS.KineticEnvironment.Effects
{
    class Pulse : AbstractLinearEffect
    {
        public const string BackgroundEffectName = "Background";
        public const string PulseEffectName = "Pulse";
        public const string WidthName = "Width";

        public const string EffectName = "Pulse";

        private IColorEffect _backgroundColor;
        private IColorEffect _pulseColor;
        private double _width;

        public Pulse(IGroup group) : base(group) { }

        public static EffectFactroy PulseFactory = g=> new Pulse(g);

        public override IList<IColorEffect> ColorEffects
        {
            get { return new List<IColorEffect>{_backgroundColor, _pulseColor}; }
        }

        public override IColorEffect StartEffect
        {
            get { return _backgroundColor; }
        }

        public override IColorEffect EndEffect
        {
            get { return _backgroundColor; }
        }

        public override string Name { get { return EffectName; } }


        protected override void ApplyProperties(EffectProperties properties)
        {
            base.ApplyProperties(properties);
            _backgroundColor = properties.GetColorEffect(BackgroundEffectName,Ordering);
            _pulseColor = properties.GetColorEffect(PulseEffectName, Ordering);
            _width = properties.GetFloat(WidthName);
        }


        protected override IColorEffect ApplyCycle(TimeRange range, LEDNode ledNode)
        {
            double pos = Tween.GetValue(range.Current);

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
                                new PropertyDefinition(PulseEffectName, EffectPropertyTypes.ColorEffect, ColorEffectDefinition.BlueFixed),
                                new PropertyDefinition(WidthName, EffectPropertyTypes.Float, 6)
                            });
    }
    
}
