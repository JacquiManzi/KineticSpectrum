using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Media3D;
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
        private readonly int _bodyIndex;
        private readonly object _gestureLock = new object();
        private readonly object _taskLock = new object();

        private IOrdering _ordering;
        private bool _tracked = false;
        private bool _isLast = false;
        private bool _restart = false;

        private Task _currentTask;

        public KinectPatternStart(IActivatable lightProvider, Scene scene, KinectPlugin kinectPlugin, int bodyIndex) :
            base(lightProvider, scene, 0, 0, GetBasePattern(), bodyIndex+1)
        {
            _ordering = GetOrdering(scene, new Vector3D());
            _bodyIndex = bodyIndex;
            _plugin = kinectPlugin;
            _patternGen = new PatternGenerator(scene);
            _patternGen.AvailableOrderings = new List<string>{SpatialOrderingTypes.Expand};
            _patternGen.Params.PatternLengthMin = .5;
            _patternGen.Params.PatternLengthMax = 1;
            _patternGen.Params.ColorsMin = 2;
            kinectPlugin.GestureUpdateHandler += HandleGestureUpdate;
        }

        private static IOrdering GetOrdering(Scene scene, Vector3D position)
        {
            IOrdering ordering = SpatialOrderings.GetPositionalOrdering(SpatialOrderingTypes.Expand, position);
            ordering.Group = scene.GetGroup("All");
            return ordering;
        }

        private void HandleGestureUpdate(GesturePosition[] gesturePositions, int lastBodyIndex)
        {
            lock (_taskLock)
            {
                if (_currentTask != null)
                {
                    return;
                }
                _currentTask = Task.Factory.StartNew(() =>
                {
                    try
                    {
                        GesturePosition gesturePosition = gesturePositions[_bodyIndex];
                        IOrdering ordering = null;
                        if (gesturePosition.IsTracked)
                        {
                            ordering = GetOrdering(Scene, gesturePosition.Head);
                        }
                        lock (_gestureLock)
                        {
                            if (ordering != null)
                                _ordering = ordering;
                            if (!_tracked && gesturePosition.IsTracked)
                                _restart = true;
                            _tracked = gesturePosition.IsTracked;
                            _isLast = _bodyIndex == lastBodyIndex;
                        }
                    }
                    finally
                    {
                        lock (_taskLock)
                        {
                            _currentTask = null;
                        }

                    }
                });
            }
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

        private Pattern GenPattern(IOrdering ordering)
        {
            Pattern pattern = _patternGen.GenSweep(new List<string> {"All"});
            pattern.EffectProperties[PropertyDefinition.RepeatCount.Name] = 1;
            pattern.EffectProperties[PropertyDefinition.Easing.Name] = Easings.Linear;
            pattern.EffectProperties[PropertyDefinition.Ordering.Name] = ordering;
            pattern.EffectProperties[PropertyDefinition.RepeatCount.Name] = 1000;
            return pattern;
        }

        public override IEnumerable<IEffectApplier> GetApplier(int time)
        {
            if (!_plugin.Enabled)
                return base.GetApplier(0);
            IOrdering ordering;
            bool tracked = false;
            bool isLast = false;
            bool restart = false;
            lock (_gestureLock)
            {
                ordering = _ordering;
                tracked = _tracked;
                isLast = _isLast;
                restart = _restart;
                _restart = false;
            }

            if (!tracked)
            {
                _appliedState = _appliedState*_plugin.FallOff;
                if (_appliedState < .05)
                    _appliedState = 0;
            }
            else if (_appliedState < .2)
            {
                _appliedState = .2;
            }
            else
            {
                _appliedState = Math.Min(1, _appliedState/_plugin.FallOff);
            }
            if (restart)
            {
                Pattern = GenPattern(ordering);
                StartTime = time;
            }

            //align time with when the underlying pattern start thinks it started

            IEffect sample = SampleEffect;
            sample.Ordering = ordering;
            IEffectApplier colorApplier = base.GetApplier(time).First();
            int fifth = sample.Properties.GetTime(PropertyDefinition.Duration.Name)/10 + StartTime;
            IEffectApplier nodeApplier = base.GetApplier(tracked?fifth:0).First();

            return new List<IEffectApplier>(1) {new KinectApplier(nodeApplier, colorApplier, _appliedState, isLast)};
        }
        /// <summary>
        /// The ammount to apply this pattern as a range from 0 to 1
        /// Once the gesture ends we don't want to abruptly clear the pattern
        /// so this allows for a falloff
        /// </summary>
        private double _appliedState = 0;
    }

    class KinectApplier : IEffectApplier
    {
        private readonly IEffectApplier _nodeApplier;
        private readonly IEffectApplier _colorApplier;
        private readonly double _alphaPortion;
        private readonly bool _applyBackground;

        public KinectApplier(IEffectApplier nodeApplier, IEffectApplier colorApplier, double alphaPortion, bool applyBackground)
        {
            _nodeApplier = nodeApplier;
            _colorApplier = colorApplier;
            _alphaPortion = alphaPortion;
            _applyBackground = applyBackground;
        }

        public IColorEffect GetEffect(LEDNode node)
        {
            return _nodeApplier.GetEffect(node);
        }

        public Color GetNodeColor(LEDNode node, IColorEffect colorEffect)
        {
            if (_applyBackground && colorEffect == StartColor)
                return new Color() {A = (byte) (256*_alphaPortion), B=1};

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
