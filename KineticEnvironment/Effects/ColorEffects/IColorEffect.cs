using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RevKitt.KS.KineticEnvironment.Effects.Order;
using RevKitt.KS.KineticEnvironment.Scenes;

namespace RevKitt.KS.KineticEnvironment.Effects.ColorEffects
{
    public interface IColorEffect
    {
        string Name { get; }


        IOrdering Ordering { get; set; }

        void SetColor(int time, LEDNode led);
    }
}
