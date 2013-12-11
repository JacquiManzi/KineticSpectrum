using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using KineticControl;
using RevKitt.KS.KineticEnvironment.Effects.ColorEffect;
using RevKitt.KS.KineticEnvironment.Scenes;

namespace RevKitt.KS.KineticEnvironment.Effects
{
    public interface IEffect
    {
        string Name { get; }

        EffectProperties Properties { get; set; }

        void Apply(int time);

        IEffectApplier GetApplier(int time);

        int TotalTime { get; }

        IList<IColorEffect> ColorEffects { get; }

        IColorEffect StartEffect { get; }
        IColorEffect EndEffect { get; }
    }
}
