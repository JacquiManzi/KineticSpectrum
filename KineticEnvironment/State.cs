using RevKitt.KS.KineticEnvironment.Scenes;
using RevKitt.KS.KineticEnvironment.Sim;

namespace RevKitt.KS.KineticEnvironment
{
    public class State
    {
        private static readonly object         Lock = new object();
        private static volatile SimulationMode _mode = SimulationMode.Scene;
        private static volatile ISimulation     _runningComposition;

        public static Scene Scene = new Scene();

        public static ISimulation Simulation = new ReadAheadSimulation(Scene);

        public static ISimulation PatternSim = new ReadAheadSimulation(Scene);

        public static ISimulation RunningComposition { set { _runningComposition = value; } }

        public static bool IsCompositionRunning { get { return _runningComposition != null; } }

        public static void EndRunningComposition()
        {
            _runningComposition = null;
        }

        public static IStateProvider Active
        {
            get
            {
                switch (_mode)
                {
                    case SimulationMode.Pattern:
                        return PatternSim;
                    case SimulationMode.Simulation:
                        return Simulation;
                    default:
                        return Scene;
                }
            }
        }

        public static SimulationMode Mode
        {
            get { return _mode; }
            set
            {
                lock (Lock)
                {
                    if (Mode != value)
                    {
                        IStateProvider current = Active;
                        _mode = value;
                        SwitchMode(current, Active);
                    }
                }
            }
        }

        private static void SwitchMode(IStateProvider currentModeState, IStateProvider newModeState)
        {
            bool isPlaying = currentModeState.IsPlaying;
            currentModeState.IsPlaying = false;
            Deactivate(currentModeState);

            newModeState.IsPlaying = isPlaying;
            if(!IsCompositionRunning)
                Activate(newModeState);
        }

        private static void Deactivate(IActivatable activatable)
        {
            foreach (LEDNode node in activatable.Nodes)
            {
                node.IsActive = false;
            }
        }

        private static void Activate(IActivatable activatable)
        {
            foreach (LEDNode node in activatable.Nodes)
            {
                node.IsActive = true;
            }
        }
    }
    public enum SimulationMode { Scene=1, Pattern=2, Simulation=3 }
}