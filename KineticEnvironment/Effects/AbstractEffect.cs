using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RevKitt.KS.KineticEnvironment.Effects.ColorEffects;
using RevKitt.KS.KineticEnvironment.Effects.Order;
using RevKitt.KS.KineticEnvironment.Scenes;
using RevKitt.KS.KineticEnvironment.Tweening;

namespace RevKitt.KS.KineticEnvironment.Effects
{
    abstract class AbstractEffect : IEffect
    {

        private readonly Group _group;

        private EffectProperties _properties;
        protected int Durration;
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

        public int TotalTime { get { return Durration*RepeatCount; } }

        public EffectProperties Properties { get { return _properties; }
        set
        {
            if(value == null)
                throw new ArgumentNullException("value");
            _properties = value;
            Durration = value.GetDurration();
            RepeatCount = value.GetRepeatCount();
            Reverse = RepeatMethods.Reverse.Equals(value.GetRepeatMethod());
            ApplyProperties(_properties);
        }}

        protected abstract void ApplyProperties(EffectProperties properties);
        protected abstract IColorEffect ApplyCycle(int time, LEDNode ledNode);


        public void Apply(int time)
        {
            if (time > Durration * RepeatCount)
                time = Durration*RepeatCount;
            int cycleTime = time%Durration;
            int cycleCount = time/Durration;

            if (Reverse && cycleCount % 2 == 1)
                cycleTime = Durration - cycleTime;
            foreach (var ledNode in _group.LEDNodes)
            {
                IColorEffect colorEffect = ApplyCycle(cycleTime, ledNode);
                colorEffect.SetColor(time, ledNode);
            }
        }

        public static IList<PropertyDefinition> DefaultDefs = new List<PropertyDefinition>{PropertyDefinition.Durration}; 
    }
    
}
