using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using RevKitt.KS.KineticEnvironment.Effects.Order;
using RevKitt.KS.KineticEnvironment.Scenes;

namespace RevKitt.KS.KineticEnvironment.Effects.ColorEffects
{
    class FixedColor : IColorEffect
    {
        public const string EffectName = "fixed";

        public readonly Color Color;

        public FixedColor(Color color)
        {
            Color = color;
        }

        public string Name { get { return EffectName; } }
        public IDictionary<string, object> Properties { get; set; }
        public IOrdering Ordering { get; set; }


        public void SetColor(int time, LEDNode led)
        {
            led.Color = Color;
        }
    }
}
