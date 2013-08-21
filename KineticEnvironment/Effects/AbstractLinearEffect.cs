using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RevKitt.KS.KineticEnvironment.Effects.Order;
using RevKitt.KS.KineticEnvironment.Scenes;
using RevKitt.KS.KineticEnvironment.Tweening;

namespace RevKitt.KS.KineticEnvironment.Effects
{
    public abstract class AbstractLinearEffect : AbstractEffect
    {

        protected Tween Tween;

        protected AbstractLinearEffect(Group group) : base(group) {}

        protected override void ApplyProperties(EffectProperties properties)
        {
            IEasing easing = properties.GetEasing(PropertyDefinition.Easing.Name);
            Tween = new Tween(easing)
                        {
                            StartTime = 0,
                            EndTime = Duration,
                            StartValue = Ordering.GetMin(),
                            EndValue = Ordering.GetMax()
                        };
        }

        public new static readonly IList<PropertyDefinition> DefaultDefs =
            new List<PropertyDefinition>(AbstractEffect.DefaultDefs)
                {
                    PropertyDefinition.RepeatCount,
                    PropertyDefinition.RepeatMethod,
                    PropertyDefinition.Easing
                }.AsReadOnly();
    }
}
