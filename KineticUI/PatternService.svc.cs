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
using RevKitt.KS.KineticEnvironment.Coloring;
using RevKitt.KS.KineticEnvironment.JSConverters;
using RevKitt.KS.KineticEnvironment.Scenes;

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
            sim.AddPattern(pObj, 0, 0, 0);
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
            var sim = State.Active;
            var ms = new MemoryStream();

            sim.WriteRange(ms, start, end);

            HttpContext context = HttpContext.Current;
            context.Response.AppendHeader("Content-encoding", "gzip");
            context.Response.Filter = new GZipStream(context.Response.Filter, CompressionMode.Compress);
            ms.Position = 0;
            System.Diagnostics.Debug.WriteLine((DateTime.Now - timerStart).TotalMilliseconds);
            return ms;
        }
    }
}
