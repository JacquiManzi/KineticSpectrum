using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using RevKitt.KS.KineticEnvironment.Coloring;
using RevKitt.KS.KineticEnvironment.Effects;
using RevKitt.KS.KineticEnvironment.Effects.ColorEffect;
using RevKitt.KS.KineticEnvironment.Effects.Order;
using RevKitt.KS.KineticEnvironment.Interact;
using RevKitt.KS.KineticEnvironment.Scenes;
using RevKitt.KS.KineticEnvironment.Tweening;

namespace RevKitt.KS.KineticEnvironment.Sim
{
    class KinectPatternStart : PatternStart
    {
        private readonly PatternGenerator _patternGen;
        private readonly KinectPlugin _plugin;

        public KinectPatternStart(IActivatable lightProvider, Scene scene, KinectPlugin kinectPlugin) :
            base(lightProvider, scene, 0, 0, GetBasePattern(), 1)
        {
            _plugin = kinectPlugin;
            _patternGen = new PatternGenerator(scene);
            _patternGen.AvailableOrderings = new List<string>{SpatialOrderingTypes.Expand, SpatialOrderingTypes.TwoDirectionsOut};
        }

        public new int EndTime { get { return int.MaxValue; } }

        private static Pattern GetBasePattern()
        {
            EffectProperties effectProperties = new EffectProperties();
            effectProperties[PropertyDefinition.Duration.Name] = 1.0;
            effectProperties[PropertyDefinition.RepeatCount.Name] = 1;
            effectProperties[PropertyDefinition.Easing.Name] = Easings.Linear;
            effectProperties[PropertyDefinition.Ordering.Name] = SpatialOrderings.GetOrdering(SpatialOrderingTypes.Expand);
            effectProperties[PropertyDefinition.RepeatMethod.Name] = RepeatMethods.Reverse;
            effectProperties[FixedEffect.ColorEffectName] = new FixedColor(ColorUtil.GetTransparent());

            return new Pattern
                {
                    EffectName = FixedEffect.EffectName,
                    EffectProperties = effectProperties,
                    Groups = new List<string>{"All"}
                };
        }

        private Pattern GenPattern()
        {
            Pattern pattern = _patternGen.GenSweep(new List<string> {"All"});
            pattern.EffectProperties[PropertyDefinition.RepeatCount.Name] = 1;
            pattern.EffectProperties[PropertyDefinition.Easing.Name] = Easings.Linear;
            return pattern;
        }

        public override IEnumerable<IEffectApplier> GetApplier(int time)
        {
            if (!_plugin.Enabled)
                return base.GetApplier(0);

            double gesturePortion = _plugin.GesturePortion;
            if (Double.IsNaN(gesturePortion))
            {
                _appliedState = _appliedState*_plugin.FallOff;
                if (_appliedState < .05)
                    _appliedState = 0;
                gesturePortion = _lastPortion;
            }
            else if (_appliedState < .5)
            {
                Pattern = GenPattern();
                _patternStart = time;
                _appliedState = 1;
                _lastPortion = gesturePortion;
            }
            else
            {
                _appliedState = 1;
                _lastPortion = gesturePortion;
            }

            //align time with when the underlying pattern start thinks it started
            time = time - _patternStart;
            int effectTime = SampleEffect.TotalTime;
            int gestureTime = (int) (effectTime*gesturePortion);
            int timeTime = GetTimeTime(time, effectTime);
            if (DateTime.Now - new TimeSpan(0, 0, 1) > _lastPrint)
            {
                _lastPrint = DateTime.Now;
                System.Diagnostics.Debug.WriteLine("timetime {0} gesturetime {1} gesture {2}", timeTime, gestureTime,
                                                   gesturePortion);
            }

            return base.GetApplier(gestureTime)
                .Zip(base.GetApplier(timeTime), (gesture, node) => new KinectApplier(gesture, node, _appliedState));

        }
        private DateTime _lastPrint = DateTime.Now;

        private int GetTimeTime(int time, int totalTime)
        {
            int rangeTime = time%totalTime;
            if ((time/totalTime)%2 == 1)
                return totalTime - rangeTime;
            return rangeTime;
        }
        /// <summary>
        /// The ammount to apply this pattern as a range from 0 to 1
        /// Once the gesture ends we don't want to abruptly clear the pattern
        /// so this allows for a falloff
        /// </summary>
        private double _appliedState = 0;
        /// <summary>
        /// The last state when the gesture was applied. When the gesture ends
        /// it stays in its last state, but falls off to transparent
        /// </summary>
        private double _lastPortion = 0;
        private int _patternStart = 0;
    }

    class KinectApplier : IEffectApplier
    {
        private readonly IEffectApplier _nodeApplier;
        private readonly IEffectApplier _colorApplier;
        private readonly double _alphaPortion;

        public KinectApplier(IEffectApplier nodeApplier, IEffectApplier colorApplier, double alphaPortion)
        {
            _nodeApplier = nodeApplier;
            _colorApplier = colorApplier;
            _alphaPortion = alphaPortion;
        }

        public IColorEffect GetEffect(LEDNode node)
        {
            return _nodeApplier.GetEffect(node);
        }

        public Color GetNodeColor(LEDNode node, IColorEffect colorEffect)
        {
            Color color = _colorApplier.GetNodeColor(node, colorEffect);
            color.A = (byte)(color.A*_alphaPortion);
            return color;
        }

        public IGroup Group { get { return _nodeApplier.Group; } }
        public IColorEffect StartColor { get { return _nodeApplier.StartColor; } }
        public IColorEffect EndColor { get { return _nodeApplier.EndColor; } }
        public int EndTime { 
            get { return int.MaxValue; }
            set { }
        }
    }
}
