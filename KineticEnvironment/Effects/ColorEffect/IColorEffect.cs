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
    public interface IColorEffect
    {
        string Name { get; }
        IList<Color> Colors { get; set; }

        IOrdering Ordering { get; set; }

        void SetColor(int time, int endTime, LEDNode led);
    }
}
