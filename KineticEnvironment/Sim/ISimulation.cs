using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RevKitt.KS.KineticEnvironment.Interact;
using RevKitt.KS.KineticEnvironment.Scenes;

namespace RevKitt.KS.KineticEnvironment.Sim
{
    public interface ISimulation : IStateProvider
    {
        void Clear();

        IPatternProvider PatternProvider { get; }

        double Speed { get; set; }

        KinectPlugin Plugin { get; set; }
    }
}
