using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Text;
using Newtonsoft.Json;
using RevKitt.KS.KineticEnvironment;
using WebApplication1.CSharp;
using WebApplication1.JSConverters;

namespace WebApplication1
{
    [ServiceContract(Namespace = "")]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class SimState
    {
        
        /// <summary>
        /// Sets the current simulation mode. This should be set whenever the user changes the page that they are on. 
        /// 1: Client mode - whatever the user selects is reflected in the lighting system. Here,
        ///    the web client is in charge and the light system reflects what is selected.
        /// 2: Pattern mode - Sets it in pattern creation mode. This allows the user to set a single pattern.
        /// 3: Simulation mode - Used for playback and simulation creation
        /// </summary>
        /// <param name="mode"></param>
        [OperationContract]
        [WebGet]
        public void SetMode(int mode)
        {
            State.Mode = (SimulationMode)mode;
        }

        [OperationContract]
        [WebGet]
        public Stream GetLightState()
        {
            var states = State.Active.LightState;
            return Serializer.ToStream(states);
        }

        [OperationContract]
        [WebGet]
        public void Play()
        {
            State.Active.IsPlaying = true;
        }

        [OperationContract]
        [WebGet]
        public void Pause()
        {
            State.Active.IsPlaying = false;
        }

        [OperationContract]
        [WebGet]
        public bool IsPlaying()
        {
            return State.Active.IsPlaying;
        }

        [OperationContract]
        [WebGet]
        public double GetTime()
        {
            return State.Active.Time/1000.0;
        }

        [OperationContract]
        [WebGet]
        public void SetTime(double time)
        {
            State.Active.Time = (int)(time*1000);
        }

        [OperationContract]
        [WebGet]
        public double GetEndTime()
        {
            return State.Active.EndTime/1000.0;
        }

    }
}
