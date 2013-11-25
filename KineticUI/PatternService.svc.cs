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
using KineticControl;
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
            var ms = new MemoryStream();
            var sw = new StreamWriter(ms);
            sw.Write(sim.EndTime);
            sw.Flush();
            ms.Position = 0;
            return ms;
        }

        [OperationContract]
        [WebGet]
        public Stream GetRange(int start, int end)
        {
            DateTime timerStart = DateTime.Now;
            var sim = State.PatternSim;
            var ms = new MemoryStream();
            var streamWriter = new StreamWriter(ms);
            if (end > sim.EndTime || start < 0)
            {
                throw new ArgumentException("Range ("+start + ',' +end+") exceeds Pattern Range (0,"+sim.EndTime+").");
            }

            int steps = (end-start)*30/1000;
            streamWriter.Write('[');
            for (int i = 0; i < steps; i++)
            {
                int time = start + i*1000/30;
                sim.Time = time;
                WriteState(streamWriter, time, sim.LightState, i==steps-1);
            }
            streamWriter.Write(']');
            streamWriter.Flush();
            HttpContext context = HttpContext.Current;
            context.Response.AppendHeader("Content-encoding", "gzip");
            context.Response.Filter = new GZipStream(context.Response.Filter, CompressionMode.Compress);
            ms.Position = 0;
            System.Diagnostics.Debug.WriteLine((DateTime.Now - timerStart).TotalMilliseconds);
            return ms;
        }

        private static void WriteState(StreamWriter streamWriter, int time, IList<LightState> states, bool last)
        {
                streamWriter.Write('[');
                streamWriter.Write(time);
                streamWriter.Write(',');
                for (int i = 0; i < states.Count; i++ )
                {
                    var state = states[i];
                    LightAddress a = state.Address;
                    streamWriter.Write(a.FixtureNo);
                    streamWriter.Write(',');
                    streamWriter.Write(a.PortNo);
                    streamWriter.Write(',');
                    streamWriter.Write(a.LightNo);
                    streamWriter.Write(',');
                    streamWriter.Write(ColorUtil.ToInt(state.Color));
                    if(i<states.Count-1)
                        streamWriter.Write(',');
                }
                streamWriter.Write(']');
                if (!last)
                {
                    streamWriter.Write(',');
                }
        }
    }
}
