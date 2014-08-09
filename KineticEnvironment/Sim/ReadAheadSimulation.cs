using System.Collections.Generic;
using System.IO;
using RevKitt.KS.KineticEnvironment.Interact;
using RevKitt.KS.KineticEnvironment.Scenes;

namespace RevKitt.KS.KineticEnvironment.Sim
{
    public class ReadAheadSimulation:ISimulation
    {
        private readonly Simulation _readAhead;
        private readonly Simulation _active;


        public static ISimulation TempSimulation(Scene scene)
        {
            return new ReadAheadSimulation(Simulation.TempSimulation(scene),
                                           Simulation.TempSimulation(scene));
        }

        public static ISimulation GenerativeSimulation(Scene scene)
        {
            return new ReadAheadSimulation(Simulation.GenerativeSimulation(scene),
                                           Simulation.GenerativeSimulation(scene));

        }

        public static ISimulation ProgrammedSimulation(string name, Scene scene)
        {
            return new ReadAheadSimulation(Simulation.ProgrammedSimulation(name, scene),
                                           Simulation.ProgrammedSimulation(name, scene));
        }

        public ReadAheadSimulation(Simulation readAhead, Simulation active)
        {
            _readAhead = readAhead;
            _active = active;
            _active.Plugin = State.Plugin;
            PatternProvider = new CompositePatternProvider(_active.PatternProvider, _readAhead.PatternProvider);
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

        public IPatternProvider PatternProvider { get; private set; }

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
            }
        }
    }
}
