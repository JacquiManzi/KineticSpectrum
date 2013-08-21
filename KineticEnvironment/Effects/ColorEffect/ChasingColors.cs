using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RevKitt.KS.KineticEnvironment.Effects.Order;
using RevKitt.KS.KineticEnvironment.Scenes;
using System.Windows.Media;

namespace RevKitt.KS.KineticEnvironment.Effects.ColorEffect
{
    class ChasingColors : IColorEffect
    {
        public const string EffectName = "ChasingColors";

        public ChasingColors(IEnumerable<Color> colors)
        {
            Colors = colors.ToList();
        }


        public string Name { get { return EffectName; } }
        public IOrdering Ordering { get; set; }
        public void SetColor(double time, double position, LEDNode led)
        {
            double timePos = (time + 2*position)%1;
            double pct = timePos * (Colors.Count-1);
            int start = (int)Math.Floor(pct);
            int end = (int) Math.Ceiling(pct);
            double diff = pct - start;
            Color color = ColorUtil.Interpolate(Colors[start], Colors[end], diff);

            led.Color = color;
        }

        public IList<Color> Colors { get; set; } 
    }
}
