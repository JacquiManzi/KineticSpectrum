using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RevKitt.KS.KineticEnvironment.Scenes;

namespace RevKitt.KS.KineticEnvironment.Effects.Order
{
    interface IOrdering
    {
        Point GetLEDPosition(LEDNode ledNode);
    }
}
