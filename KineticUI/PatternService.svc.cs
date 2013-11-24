using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Text;
using System.Threading;
using System.Web;
using RevKitt.KS.KineticEnvironment;
using RevKitt.KS.KineticEnvironment.Scenes;
using KineticUI.JSConverters;

namespace KineticUI
{
    [ServiceContract(Namespace = "")]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class PatternService
    {
        
        [OperationContract]
        public Stream TryPattern(Stream pattern)
        {
            var pObj = Serializer.FromPost<Pattern>(pattern);

            var sim = State.PatternSim;
            sim.Clear();
            sim.AddPattern(pObj, 0, 0);
            var stateMap = new Dictionary<double, IEnumerable<LightState>>();

            int steps = sim.EndTime*30/1000;
            for (int i = 0; i < steps; i++)
            {
                int time = i*1000/30;
                sim.Time = time;
                var timeState = sim.LightState.Select(l => new LightState {Address = l.Address, Color = l.Color});
                stateMap.Add(i/30.0, new List<LightState>(timeState));
            }
            HttpContext context = HttpContext.Current;
            context.Response.AppendHeader("Content-encoding", "gzip");
            context.Response.Filter = new GZipStream(context.Response.Filter, CompressionMode.Compress);
            return Serializer.ToStream(stateMap);
        }
    }
}
