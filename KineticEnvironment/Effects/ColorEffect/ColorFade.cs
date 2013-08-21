using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using RevKitt.KS.KineticEnvironment.Effects.Order;
using RevKitt.KS.KineticEnvironment.Scenes;

namespace RevKitt.KS.KineticEnvironment.Effects.ColorEffect
{
    public class ColorFade : IColorEffect
    {
        public const string EffectName = "ColorFade";

        public ColorFade(IEnumerable<Color> colors)
        {
            Colors = colors.ToList();
        }


        public string Name { get { return EffectName; } }
        public IOrdering Ordering { get; set; }
        public void SetColor(double time, double position, LEDNode led)
        {
            double pct = time * (Colors.Count-1);
            int start = (int)Math.Floor(pct);
            int end = (int) Math.Ceiling(pct);
            double diff = pct - start;
            Color color = ColorUtil.Interpolate(Colors[start], Colors[end], diff);

            led.Color = color;
        }

        public IList<Color> Colors { get; set; } 
    }
}
