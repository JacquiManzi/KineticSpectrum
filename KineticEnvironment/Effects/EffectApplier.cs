using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using RevKitt.KS.KineticEnvironment.Coloring;
using RevKitt.KS.KineticEnvironment.Effects.ColorEffect;
using RevKitt.KS.KineticEnvironment.Scenes;

namespace RevKitt.KS.KineticEnvironment.Effects
{
    public interface IEffectApplier
    {
        IColorEffect GetEffect(LEDNode node);
        Color GetNodeColor(LEDNode node, IColorEffect colorEffect);
        IGroup Group { get; }

        IColorEffect StartColor { get; }
        IColorEffect EndColor { get; }
        int EndTime { get; set; }
    }
}
