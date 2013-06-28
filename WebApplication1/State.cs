using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RevKitt.KS.KineticEnvironment.Scenes;
using RevKitt.KS.KineticEnvironment.Sim;

namespace WebApplication1
{
    public class State
    {
        public static Scene Scene = new Scene();

        public static Simulation Simulation = new Simulation(Scene);

        public static Simulation PatternSim = new Simulation(Scene);
    }
}