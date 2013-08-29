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
        public Color SetColor(TimeRange range, double position, LEDNode led)
        {
            double pct = range.Portion * (Colors.Count-1);
            int start = (int)Math.Floor(pct);
            int end = (int) Math.Ceiling(pct);
            double diff = pct - start;
            Color color = ColorUtil.Interpolate(Colors[start], Colors[end], diff);

            return color;
        }

        public IList<Color> Colors { get; set; } 
    }
}
