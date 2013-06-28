using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RevKitt.KS.KineticEnvironment.Scenes;

namespace RevKitt.KS.KineticEnvironment.Effects.Order
{
    public interface IOrdering
    {
        Group Group { get; set; } 
        string OrderingType { get; }
        string Ordering { get; }
        bool Runnable { get; }

        double GetLEDPosition(LEDNode ledNode);
        double GetMax();
        double GetMin();
    }
}
