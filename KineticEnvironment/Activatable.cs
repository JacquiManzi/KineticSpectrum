using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RevKitt.KS.KineticEnvironment.Scenes;

namespace RevKitt.KS.KineticEnvironment
{
    public interface IActivatable
    {
        IEnumerable<LEDNode> Nodes { get; }
    }
}
