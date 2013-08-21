﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using KineticControl;
using RevKitt.KS.KineticEnvironment.Effects.ColorEffect;
using RevKitt.KS.KineticEnvironment.Effects.Order;
using RevKitt.KS.KineticEnvironment.Scenes;

namespace RevKitt.KS.KineticEnvironment.Effects
{
    class FixedEffect : AbstractEffect
    {
        private const string ColorEffectName = "Color Effect";
        public const string EffectName = "Fixed";
        public static EffectFactroy FixedFactory = g=>new FixedEffect(g);

        private IColorEffect _colorEffect;
        
        public FixedEffect(Group group) : base(group) { }

        public override string Name
        {
            get { return EffectName; }
        }

        protected override void ApplyProperties(EffectProperties properties)
        {
            IOrdering ordering = properties.GetOrdering(Group);
            _colorEffect = properties.GetColorEffect(ColorEffectName, ordering);
        }

        protected override IColorEffect ApplyCycle(int time, LEDNode ledNode)
        {
            return _colorEffect;
        }

        private static void ApplyColors(IEnumerable<IEffect> colorEffect, int numCycles)
        {
            Patterns patt = new Patterns();
            IList<Color> colors = Enumerable.Range(0, numCycles).Select(i => patt.RandomColor()).ToList();
            foreach (var effect in colorEffect)
            {
                ((FixedEffect) effect)._colorEffect.Colors = colors;
            }
        }

        public static readonly PropertyDefinition StartColor = new PropertyDefinition(ColorEffectName,
                                                                                      EffectPropertyTypes.ColorEffect,
                                                                                      ColorEffectDefinition.BlueFixed);

        public static readonly EffectAttributes Attributes = new EffectAttributes(EffectName, FixedFactory,ApplyColors,
            new List<PropertyDefinition>(DefaultDefs)
                {
                    PropertyDefinition.RepeatCount,
                    PropertyDefinition.RepeatMethod,
                    PropertyDefinition.Ordering,
                    StartColor
                }.AsReadOnly());
    }
}