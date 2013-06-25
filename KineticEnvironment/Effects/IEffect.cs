using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevKitt.KS.KineticEnvironment.Effects
{
    interface IEffect
    {
        string Name { get; }

        EffectProperties Properties { get; }
    }
}
