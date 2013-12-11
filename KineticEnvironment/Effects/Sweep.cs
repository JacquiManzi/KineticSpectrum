using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using KineticControl;
using RevKitt.KS.KineticEnvironment.Effects.ColorEffect;
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
       

        public Sweep(IGroup group) : base(group) { }

        public static EffectFactroy SweepFactory = g=>new Sweep(g);

        public override IList<IColorEffect> ColorEffects
        {
            get { return new List<IColorEffect>{_startColorEffect, _endColorEffect}; }
        }

        public override IColorEffect StartEffect
        {
            get { return _startColorEffect; }
        }

        public override IColorEffect EndEffect
        {
            get { return _endColorEffect; }
        }

        public override string Name { get { return EffectName; } }

        protected override void ApplyProperties(EffectProperties properties)
        {
            base.ApplyProperties(properties);
            _startColorEffect = properties.GetColorEffect(StartEffectName,Ordering);
            _endColorEffect = properties.GetColorEffect(EndEffectName,Ordering);
        }


        protected override IColorEffect ApplyCycle(TimeRange range, LEDNode ledNode)
        {
            double pos = Tween.GetValue(range.Current);

            if (Ordering.GetLEDPosition(ledNode) <= pos)
                return _endColorEffect;
            
            return _startColorEffect;   
        }

        private static void ApplyColors(IEnumerable<IEffect> colorEffect, int numCycles)
        {
            colorEffect = colorEffect.ToList();
            List<Color> startColors = new List<Color>();
            List<Color> endColors = new List<Color>();
            Patterns patt = new Patterns();
            startColors.Add(patt.RandomColor());
            
            for (int i = 0; i < (numCycles-1); i++)
            {
                Color next = patt.RandomColor();
                startColors.Add(next);
                endColors.Add(next);
            }
            endColors.Add(patt.RandomColor());
            IColorEffect startEffect = ((Sweep)(colorEffect.First()))._startColorEffect;
            IColorEffect endEffect = ((Sweep)(colorEffect.First()))._endColorEffect;
            if(startEffect.GetType() == typeof(ColorFade))
                startColors.Add(patt.RandomColor());
            if (endEffect.GetType() == typeof(ColorFade))
                endColors.Add(patt.RandomColor());

            foreach (var effect in colorEffect)
            {
                ((Sweep) effect)._startColorEffect.Colors = startColors;
                ((Sweep) effect)._endColorEffect.Colors = endColors;
            }
        }

        public static readonly PropertyDefinition StartColor = new PropertyDefinition(StartEffectName,
                                                                                      EffectPropertyTypes.ColorEffect,
                                                                                      ColorEffectDefinition.RedFixed);

        public static readonly PropertyDefinition EndColor = new PropertyDefinition(EndEffectName,
                                                                                    EffectPropertyTypes.ColorEffect,
                                                                                    ColorEffectDefinition.BlueFixed);
        public static readonly EffectAttributes Attributes = new EffectAttributes(EffectName, SweepFactory, ApplyColors,
                        new List<PropertyDefinition>(DefaultDefs)
                            {
                                StartColor,
                                EndColor
                            });
    }
}
