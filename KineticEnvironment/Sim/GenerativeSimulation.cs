using System.Collections.Generic;
using System.IO;
using RevKitt.KS.KineticEnvironment.Interact;
using RevKitt.KS.KineticEnvironment.Scenes;

namespace RevKitt.KS.KineticEnvironment.Sim
{
    public class GenerativeSimulation : ISimulation
    {
        private IList<LightStatePattern> _lightState; 

        public IEnumerable<LEDNode> Nodes { get; private set; }
        public bool IsActive { get; set; }
        public int Time { get; set; }
        public bool IsPlaying { get; set; }
        public IList<LightState> LightState { get { return new List<LightState>(_lightState); } }
        public int EndTime { get; private set; }

        public void WriteRange(Stream stream, int start, int end)
        {
            throw new System.NotImplementedException();
        }

        public void Clear()
        {
            throw new System.NotImplementedException();
        }

        public IList<PatternStart> PatternStarts { get; private set; }
        public IPatternProvider PatternProvider { get; private set; }

        public PatternStart AddPattern(string patternName, int startTime, int id, int priority)
        {
            throw new System.NotImplementedException();
        }

        public PatternStart AddPattern(Pattern pattern, int startTime, int id, int priority)
        {
            throw new System.NotImplementedException();
        }

        public void ShiftAfter(int shiftAfterTime, int timeToShift)
        {
            throw new System.NotImplementedException();
        }

        public void RemovePattern(int id)
        {
            throw new System.NotImplementedException();
        }

        public double Speed { get; set; }
        public KinectPlugin Plugin { get; set; }
    }
}