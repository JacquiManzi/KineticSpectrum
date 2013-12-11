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

        IList<PatternStart> PatternStarts { get; }

        PatternStart AddPattern(string patternName, int startTime, int id, int priority);
        PatternStart AddPattern(Pattern pattern, int startTime, int id, int priority);

        void ShiftAfter(int shiftAfterTime, int timeToShift);

        void RemovePattern(int id);

        double Speed { get; set; }

        KinectPlugin Plugin { get; set; }
    }
}
