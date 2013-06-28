using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RevKitt.KS.KineticEnvironment.Scenes;

namespace RevKitt.KS.KineticEnvironment.Effects
{
    public interface IEffect
    {
        string Name { get; }

        EffectProperties Properties { get; set; }

        void Apply(int time);

        int TotalTime { get; }
    }
}
