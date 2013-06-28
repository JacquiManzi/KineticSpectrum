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
    class Pulse : IEffect
    {
        public const string EffectName = "Pulse";

        private readonly Group _group;

        private EffectProperties _properties;
        private IColorEffect _backgroundColor;
        private IColorEffect _pulseColor;
        private int _durration;
        private int _repeatCount;
        private bool _reverse;
        private double _width;
        private IOrdering _ordering;
        private Tween _tween;

        public Pulse(Group group)
        {
            if(group == null)
                throw new ArgumentNullException("group");
            _group = group;
        }

        public static EffectFactroy PulseFactory = g=> new Pulse(g);

        public string Name { get { return EffectName; } }

        public int TotalTime { get { return _durration*_repeatCount; } }

        public EffectProperties Properties { get { return _properties; }
        set
        {
            if(value == null)
                throw new ArgumentNullException("value");
            _properties = value;
            _durration = value.GetTime(PropertyDefinition.Durration.Name);
            _repeatCount = value.GetRepeatCount();
            _backgroundColor = value.GetColorEffect("background");
            _pulseColor = value.GetColorEffect("pulseColor");
            _reverse = RepeatMethods.Reverse.Equals(value.GetRepeatMethod());
            _ordering = value.GetOrdering(_group);
            _width = value.GetFloat("width");

            IEasing easing = value.GetEasing(PropertyDefinition.Easing.Name);
            _tween = new Tween(easing);
            _tween.StartTime = 0;
            _tween.EndTime = _durration;
            _tween.StartValue = _ordering.GetMin();
            _tween.EndValue = _ordering.GetMax();
        }}

        public void Apply(int time)
        {
            if (time > _durration * _repeatCount)
                time = _durration*_repeatCount;
            int cycleTime = time%_durration;
            int cycleCount = time/_durration;

            if (_reverse && cycleCount % 2 == 1)
                cycleTime = _durration - cycleTime;
            foreach (var ledNode in _group.LEDNodes)
            {
                IColorEffect colorEffect = ApplyCycle(cycleTime, ledNode);
                colorEffect.SetColor(time, ledNode);
            }
        }

        private IColorEffect ApplyCycle(int time, LEDNode ledNode)
        {
            double pos = _tween.GetValue(time);

            double rangeStart = pos - _width/2;
            double rangeEnd = pos + _width/2;
            double ledPos = _ordering.GetLEDPosition(ledNode);


            if(ledPos < rangeStart || ledPos > rangeEnd)
                return _backgroundColor;

            return _pulseColor;

        }

        public static readonly EffectAttributes Attributes = new EffectAttributes(EffectName, PulseFactory,
                        new List<PropertyDefinition>
                            {
                                PropertyDefinition.Durration,
                                new PropertyDefinition("background", EffectPropertyTypes.ColorEffect, ColorEffectDefinition.DefaultFixed),
                                new PropertyDefinition("pulseColor", EffectPropertyTypes.ColorEffect, ColorEffectDefinition.DefaultFixed),
                                PropertyDefinition.RepeatCount,
                                PropertyDefinition.RepeatMethod,
                                PropertyDefinition.Ordering,
                                PropertyDefinition.Easing,
                                new PropertyDefinition("width", EffectPropertyTypes.Float, 6)
                            });
    }
    
}
