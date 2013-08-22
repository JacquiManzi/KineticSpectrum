using System;
using System.Collections.Generic;
using RevKitt.KS.KineticEnvironment.Effects.ColorEffect;
using RevKitt.KS.KineticEnvironment.Effects.Order;
using RevKitt.KS.KineticEnvironment.Scenes;

namespace RevKitt.KS.KineticEnvironment.Effects
{
    public abstract class AbstractEffect : IEffect
    {

        private readonly Group _group;

        private EffectProperties _properties;
        protected int Duration { get; private set; }
        protected int RepeatCount { get; private set; }
        protected bool Reverse { get; private set; }
        protected IOrdering Ordering { get; private set; }

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
            Ordering = value.GetOrdering(Group);
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
                cycleTime = Duration - cycleTime - 1;

            double orderingMin = Ordering.GetMin();
            double orderingSize = Ordering.GetMax() - Ordering.GetMin();
            foreach (var ledNode in _group.LEDNodes)
            {
                IColorEffect colorEffect = ApplyCycle(cycleTime, ledNode);
                double position = (Ordering.GetLEDPosition(ledNode) - orderingMin)/orderingSize;
                colorEffect.SetColor((1.0*cycleTime)/Duration, position, ledNode);
            }
        }

        public static IList<PropertyDefinition> DefaultDefs = new List<PropertyDefinition>
                                                                  {
                                                                      PropertyDefinition.Duration,
                                                                      PropertyDefinition.RepeatCount,
                                                                      PropertyDefinition.RepeatMethod,
                                                                      PropertyDefinition.Ordering
                                                                  };
    }
    
}
