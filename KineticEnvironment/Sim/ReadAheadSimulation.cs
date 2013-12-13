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
    public class ReadAheadSimulation:ISimulation
    {
        private readonly Simulation _readAhead;
        private readonly Simulation _active;

        public ReadAheadSimulation(Scene scene)
        {
            _readAhead = new Simulation(scene);
            _active = new Simulation(scene) {Plugin = State.Plugin};
        }

        public ReadAheadSimulation(Scene scene, String name)
        {
            _readAhead = new Simulation(name, scene);
            _active = new Simulation(name, scene) {Plugin = State.Plugin};
        }

        public IEnumerable<LEDNode> Nodes
        {
            get { return _active.Nodes; }
        }

        public bool IsActive { get { return _active.IsActive; } set { _active.IsActive = value; }}

        public int Time
        {
            get { return _active.Time; }
            set
            {
                _active.Time = value;
                _readAhead.Time = value;
            }
        }

        public bool IsPlaying
        {
            get { return _active.IsPlaying; }
            set { 
                _active.IsPlaying = value;
                _readAhead.IsPlaying = value;
            }
        }

        public IList<LightState> LightState { get { return _active.LightState; } }

        public int EndTime { get { return _active.EndTime; }}
        public void WriteRange(Stream stream, int start, int end)
        {
            _readAhead.WriteRange(stream, start, end);
        }

        public void Clear()
        {
            _active.Clear();
            _readAhead.Clear();
        }

        public IList<PatternStart> PatternStarts { get { return _active.PatternStarts; } }

        public PatternStart AddPattern(string patternName, int startTime, int id, int priority)
        {
            PatternStart added = _active.AddPattern(patternName, startTime, id, priority);
            _readAhead.AddPattern(added.Pattern, startTime, id, priority);
            return added;
        }

        public PatternStart AddPattern(Pattern pattern, int startTime, int id, int priority)
        {
            PatternStart added = _active.AddPattern(pattern, startTime, id, priority);
            _readAhead.AddPattern(added.Pattern, startTime, id, priority);
            return added;
        }

        public void ShiftAfter(int shiftAfterTime, int timeToShift)
        {
            _active.ShiftAfter(shiftAfterTime, timeToShift);
            _readAhead.ShiftAfter(shiftAfterTime, timeToShift);
        }

        public void RemovePattern(int id)
        {
            _active.RemovePattern(id);
            _readAhead.RemovePattern(id);
        }

        public double Speed
        {
            get { return _active.Speed; }
            set { _active.Speed = value; }
        }

        public KinectPlugin Plugin
        {
            get { return _active.Plugin; }
            set
            {
                _active.Plugin = value;
                _readAhead.Plugin = value;
            }
        }
    }
}
