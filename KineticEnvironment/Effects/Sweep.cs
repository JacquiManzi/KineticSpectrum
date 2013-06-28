﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RevKitt.KS.KineticEnvironment.Effects.ColorEffects;
using RevKitt.KS.KineticEnvironment.Effects.Order;
using RevKitt.KS.KineticEnvironment.Scenes;
using RevKitt.KS.KineticEnvironment.Tweening;

namespace RevKitt.KS.KineticEnvironment.Effects
{
    class Sweep : IEffect
    {
        public const string EffectName = "Sweep";

        private readonly Group _group;

        private EffectProperties _properties;
        private IColorEffect _startColorEffect;
        private IColorEffect _endColorEffect;
        private int _durration;
        private int _repeatCount;
        private bool _reverse; 
        private IOrdering _ordering;
        private Tween _tween;

        public Sweep(Group group)
        {
            if(group == null)
                throw new ArgumentNullException("group");
            _group = group;
        }

        public static EffectFactroy SweepFactory = g=>new Sweep(g);

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
            _startColorEffect = value.GetColorEffect("startColor");
            _endColorEffect = value.GetColorEffect("endColor");
            _reverse = RepeatMethods.Reverse.Equals(value.GetRepeatMethod());
            _ordering = value.GetOrdering(_group);

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

            if (_ordering.GetLEDPosition(ledNode) <= pos)
                return _endColorEffect;
            
            return _startColorEffect;
            
        }

        public static readonly EffectAttributes Attributes = new EffectAttributes(EffectName, SweepFactory,
                        new List<PropertyDefinition>
                            {
                                PropertyDefinition.Durration,
                                new PropertyDefinition("startColor", EffectPropertyTypes.ColorEffect, ColorEffectDefinition.DefaultFixed),
                                new PropertyDefinition("endColor", EffectPropertyTypes.ColorEffect, ColorEffectDefinition.DefaultFixed),
                                PropertyDefinition.RepeatCount,
                                PropertyDefinition.RepeatMethod,
                                PropertyDefinition.Ordering,
                                PropertyDefinition.Easing
                            });
    }
}
