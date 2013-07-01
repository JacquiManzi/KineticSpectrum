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
    public class ColorFade : IColorEffect
    {
        public const string EffectName = "Color Fade";

        public ColorFade(IEnumerable<Color> colors)
        {
            Colors = colors.ToList();
        }


        public string Name { get; private set; }
        public IOrdering Ordering { get; set; }
        public void SetColor(int time, LEDNode led)
        {
            throw new NotImplementedException();
        }

        public IList<Color> Colors { get; set; } 
    }
}
