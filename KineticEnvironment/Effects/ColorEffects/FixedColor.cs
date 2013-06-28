﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using RevKitt.KS.KineticEnvironment.Scenes;

namespace RevKitt.KS.KineticEnvironment.Effects.ColorEffects
{
    class FixedColor : IColorEffect
    {

        public readonly Color Color;

        public FixedColor(Color color)
        {
            Color = color;
        }

        public string Name { get { return "Fixed"; } }
        public EffectProperties EffectProperties { get; set; }
        public void SetColor(int time, LEDNode led)
        {
            led.Color = Color;
        }
    }
}