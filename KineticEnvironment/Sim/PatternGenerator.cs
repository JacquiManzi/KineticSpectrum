using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using RevKitt.KS.KineticEnvironment.Coloring;
using RevKitt.KS.KineticEnvironment.Effects;
using RevKitt.KS.KineticEnvironment.Effects.ColorEffect;
using RevKitt.KS.KineticEnvironment.Scenes;
using RevKitt.KS.KineticEnvironment.Tweening;

namespace RevKitt.KS.KineticEnvironment.Sim
{
    class PatternParams
    {
        public int PatternLengthMin = 1;
        public int PatternLengthMax = 10;
        public double PatternIntervalMin = .5;
        public double PatternIntervalMax = 5;
        public int PulseSizeMin = 10;
        public int PulseSizeMax = 50;
        public int PulseRepeatMin = 1;
        public int PulseRepeastMax = 5;
        public int ColorsMin = 1;
        public int ColorMax = 6;
    }

    class PatternGenerator
    {
        private readonly Scene _scene;
        private Random _random = new Random();
        public PatternParams Params = new PatternParams();
        private int _nextTime = 0;

        public PatternGenerator(Scene scene)
        {
            _scene = scene;
        }

        public IEnumerable<Pattern> GetPattern(int time)
        {
            if (IsNewPattern(time))
            {
                return new List<Pattern> {GenPattern()};
            }
            return Enumerable.Empty<Pattern>();
        }

        private List<IGroup> GetGroups()
        {
            return new List<IGroup> {_scene.GetGroup("All")};
        }

        private Pattern GenPattern()
        {
            switch (_random.Next(4))
            {
                case 1:
                    return GenSweep(GetGroups());
                default:
                    return GenPulse(GetGroups());
            }
        }

        private IColorEffect GetTransparent()
        {
            return new FixedColor(Colors.Transparent);
        }

        private IColorEffect GenerateColorEffect()
        {
            switch (_random.Next(2))
            {
                case 1:
                    return new ChasingColors(GenColors());
                default:
                    return new ColorFade(GenColors());
            }
        }

        private IEnumerable<Color> GenColors()
        {
            return ColorPicker.PickColors(GenRange(Params.ColorsMin, Params.ColorMax));
        }

        private Pattern GenPulse(IList<IGroup> groups )
        {
            EffectProperties effectProperties = new EffectProperties();
            effectProperties[Pulse.BackgroundEffectName] = GetTransparent();
            effectProperties[Pulse.PulseEffectName] = GenerateColorEffect();
            ApplyEasing(effectProperties);
            effectProperties[PropertyDefinition.RepeatMethod.Name] = RepeatMethods.GetRandom();
            ApplyDuration(effectProperties);
            ApplyOrdering(effectProperties);
            ApplyRepeat(effectProperties, true);
            Pattern pattern = new Pattern();
            pattern.EffectName = Pulse.EffectName;
            pattern.EffectProperties = effectProperties;
            pattern.Groups = groups.Select(g => g.Name).ToList();
            return pattern;
        }

        private Pattern GenSweep(IList<IGroup> groups )
        {
            EffectProperties effectProperties = new EffectProperties();
            effectProperties[Sweep.StartEffectName] = GetTransparent();
            IColorEffect colorEffect = GenerateColorEffect();
            effectProperties[Sweep.EndEffectName] = colorEffect;
            ApplyEasing(effectProperties);
            effectProperties[PropertyDefinition.RepeatMethod.Name] = RepeatMethods.Reverse;
            ApplyDuration(effectProperties);
            ApplyOrdering(effectProperties);
            ApplyRepeat(effectProperties, colorEffect.Colors.Count != 1 );
            Pattern pattern = new Pattern();
            pattern.EffectName = Sweep.EffectName;
            pattern.EffectProperties = effectProperties;
            pattern.Groups = groups.Select(g => g.Name).ToList();
            return pattern;
        }

        private void ApplyOrdering(EffectProperties effectProperties)
        {
            effectProperties[PropertyDefinition.Ordering.Name] = OrderingTypes.Spatial;
        }

        private void ApplyEasing(EffectProperties effectProperties)
        {
            int select = _random.Next(20);
            IEasing easing = Easings.Linear;
            if (select >= 10 && select < 15)
                easing = Easings.Quadratic;
            if (select >= 15 && select < 17)
                easing = Easings.Bounce;
            if (select >= 17 && select < 19)
                easing = Easings.Sinusoidal;
            if (select >= 19)
                easing = Easings.Circular;

            effectProperties[PropertyDefinition.Easing.Name] = easing;
        }

        private void ApplyDuration(EffectProperties effectProperties)
        {
            effectProperties[PropertyDefinition.Duration.Name] = GenRange(Params.PatternLengthMin,
                                                                          Params.PatternLengthMax);
        }

        private void ApplyRepeat(EffectProperties effectProperties, bool onlyEven)
        {
            int repeatCount = GenRange(Params.PulseRepeatMin, Params.PulseRepeastMax);
            if (onlyEven)
            {
                repeatCount = repeatCount - 1;
                repeatCount = Math.Max(2, repeatCount);
            }
            effectProperties[PropertyDefinition.RepeatCount.Name] = repeatCount;
        }

        private bool IsNewPattern(int time)
        {
            if (time >= _nextTime)
            {
                _nextTime = (int) (1000*GenRange(Params.PatternIntervalMin, Params.PatternIntervalMax));
                return true;
            }
            return false;
        }

        private int GenRange(int min, int max)
        {
            return _random.Next(max - min) - min + 1;
        }
        private double GenRange(double min, double max)
        {
            return _random.NextDouble()* (max-min) + min;
        }

    }
}
