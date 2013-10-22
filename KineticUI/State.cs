using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RevKitt.KS.KineticEnvironment;
using RevKitt.KS.KineticEnvironment.Scenes;
using RevKitt.KS.KineticEnvironment.Sim;

namespace KineticUI
{
    public class State
    {
        private static SimulationMode _mode = SimulationMode.Scene;

        public static Scene Scene = new Scene();

        public static Simulation Simulation = new Simulation(Scene);

        public static Simulation PatternSim = new Simulation(Scene);

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
                bool playing = Active.IsPlaying;
                Active.IsPlaying = false;
                _mode = value;
                Active.IsPlaying = playing;
            }
        }

    }
    public enum SimulationMode { Scene=1, Pattern=2, Simulation=3 }
}