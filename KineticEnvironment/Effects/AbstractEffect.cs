using System;
using System.Collections.Generic;
using RevKitt.KS.KineticEnvironment.Effects.ColorEffect;
using RevKitt.KS.KineticEnvironment.Scenes;

namespace RevKitt.KS.KineticEnvironment.Effects
{
    public abstract class AbstractEffect : IEffect
    {

        private readonly Group _group;

        private EffectProperties _properties;
        protected int Duration;
        protected int RepeatCount;
        protected bool Reverse; 

        protected AbstractEffect(Group group)
        {
            if(group == null)
                throw new ArgumentNullException("group");
            _group = group;
        }

        public abstract string Name { get; }
        protected Group Group { get { return _group; } }

        public int TotalTime { get { return Duration*RepeatCount; } }

        public EffectProperties Properties { get { return _properties; }
        set
        {
            if(value == null)
                throw new ArgumentNullException("value");
            _properties = value;
            Duration = value.GetDuration();
            RepeatCount = value.GetRepeatCount();
            Reverse = RepeatMethods.Reverse.Equals(value.GetRepeatMethod());
            ApplyProperties(_properties);
        }}

        protected abstract void ApplyProperties(EffectProperties properties);
        protected abstract IColorEffect ApplyCycle(int time, LEDNode ledNode);


        public void Apply(int time)
        {
            if (time > Duration * RepeatCount)
                time = Duration*RepeatCount;
            int cycleTime = time%Duration;
            int cycleCount = time/Duration;

            if (Reverse && cycleCount % 2 == 1)
                cycleTime = Duration - cycleTime;
            foreach (var ledNode in _group.LEDNodes)
            {
                IColorEffect colorEffect = ApplyCycle(cycleTime, ledNode);
                colorEffect.SetColor(cycleTime, Duration, ledNode);
            }
        }

        public static IList<PropertyDefinition> DefaultDefs = new List<PropertyDefinition>{PropertyDefinition.Duration}; 
    }
    
}
