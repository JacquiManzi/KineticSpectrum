using System;
using System.Collections.Generic;
using RevKitt.KS.KineticEnvironment.Effects.ColorEffect;
using RevKitt.KS.KineticEnvironment.Effects.Order;
using RevKitt.KS.KineticEnvironment.Scenes;
using System.Windows.Media;

namespace RevKitt.KS.KineticEnvironment.Effects
{
    public abstract class AbstractEffect : IEffect
    {

        private readonly IGroup _group;

        public int Priority { get; internal set; }

        private EffectProperties _properties;
        protected int Duration { get; private set; }
        protected int RepeatCount { get; private set; }
        protected bool Reverse { get; private set; }
        protected IOrdering Ordering { get; private set; }

        protected AbstractEffect(IGroup group)
        {
            if(group == null)
                throw new ArgumentNullException("group");
            _group = group;
        }

        public abstract string Name { get; }
        protected IGroup Group { get { return _group; } }

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
        protected abstract IColorEffect ApplyCycle(TimeRange range, LEDNode ledNode);


        public void Apply(int time)
        {
            if (time > Duration * RepeatCount)
                time = Duration*RepeatCount;
            int cycleTime = time%Duration;
            int cycleCount = time/Duration;

            if (Reverse && cycleCount % 2 == 1)
                cycleTime = Duration - cycleTime - 1;

            TimeRange range = new TimeRange(cycleTime, Duration, cycleCount, Duration*RepeatCount);
            double orderingMin = Ordering.GetMin();
            double orderingSize = Ordering.GetMax() - Ordering.GetMin();
            foreach (var ledNode in _group.LEDNodes)
            {
                IColorEffect colorEffect = ApplyCycle(range, ledNode);
                double position = (Ordering.GetLEDPosition(ledNode) - orderingMin)/orderingSize;
                Color color = ColorUtil.Clone(colorEffect.SetColor(range, position, ledNode));
                color.A = (byte)Priority;
                Color ledColor = ledNode.Color;

                if (Priority < ledColor.A)
                    ledNode.Color = color;
                else if (Priority == ledColor.A)
                    ledNode.Color = ColorUtil.Interpolate(ledColor, color, .5);
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
