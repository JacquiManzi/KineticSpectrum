using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevKitt.KS.KineticEnvironment.Tweening
{
    public delegate double EasingFunc(double inputPortion);

    public interface IEasing
    {
        string Name { get; }
        EasingFunc In { get; }
        EasingFunc Out { get; }
        EasingFunc OutIn { get; } 
    }
}
