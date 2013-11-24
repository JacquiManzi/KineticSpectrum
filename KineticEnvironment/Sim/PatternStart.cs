using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RevKitt.KS.KineticEnvironment.Effects;
using RevKitt.KS.KineticEnvironment.Scenes;

namespace RevKitt.KS.KineticEnvironment.Sim
{
    public class PatternStart
    {
        private readonly Scene _scene;
        private readonly IActivatable _lightProvider;
        private bool _localPriority = true;
        private readonly int _id;
        private int _priority;
        private Pattern _pattern;
        private Dictionary<IGroup, IEffect> _groupToEffect ; 

        public PatternStart(IActivatable lightProvider, Scene scene, int id, int startTime, Pattern pattern) : this(lightProvider, scene, id, startTime, pattern, pattern.Priority)
        {
            _localPriority = false;
        }

        public PatternStart(IActivatable lightProvider, Scene scene, int id, int startTime, Pattern pattern, int priority)
        {
            _id = id;
            StartTime = startTime;
            _priority = priority;
            _scene = scene;
            _lightProvider = lightProvider;
            Pattern = pattern;
        }

        public Pattern Pattern
        {
            get { return _pattern; }
            set
            {
                _pattern = value;
                _pattern.OnChanged += SetupPattern;
                SetupPattern(_pattern);
            }
        }

        public int Priority
        {
            get { return _priority; }
            set
            {
                _priority = value;
                _localPriority = true;
            }
        }

        public int Id { get { return _id; } }

        public int StartTime { get; set; }

        private void SetupPattern(Pattern pattern)
        {
            Dictionary<IGroup, IEffect> groupToEffect = new Dictionary<IGroup, IEffect>();
            IList<IGroup> groups = _scene.GetGroupRefs(pattern.Groups, _lightProvider);
            EffectAttributes attr = EffectRegistry.EffectAttributes[pattern.EffectName];
            foreach (var group in groups)
            {
                IEffect effect = attr.EffectFactory(group);
                effect.Properties = pattern.EffectProperties;
                groupToEffect.Add(group, effect);
            }
            if (!_localPriority)
                _priority = pattern.Priority;
            _groupToEffect = groupToEffect;
        }


        public bool Applies(int time)
        {
            return time >= StartTime && time < EndTime;
        }

        public void Apply(int time)
        {
            time = time - StartTime;
            foreach (var effect in _groupToEffect.Values)
            {
                effect.Apply(time);                
            }
        }

        public int EndTime
        {
            get { return StartTime + _groupToEffect.First().Value.TotalTime; }
        }

        public static readonly IComparer<PatternStart> StartTimeComparer = new PatternStartComparer(); 
        public static readonly IComparer<PatternStart> PriorityComparer = new PattenPriorityComparer(); 
    }

    public class PatternStartComparer : IComparer<PatternStart>
    {
        public int Compare(PatternStart x, PatternStart y)
        {
            return x.StartTime - y.StartTime;
        }
    }

    public class PattenPriorityComparer : IComparer<PatternStart>
    {
        public int Compare(PatternStart x, PatternStart y)
        {
            return y.Priority - x.Priority;
        }
    }
}
