using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Text;
using RevKitt.KS.KineticEnvironment.Scenes;
using WebApplication1.JSConverters;

namespace WebApplication1
{
    [ServiceContract(Namespace = "")]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class PatternService
    {
        
        [OperationContract]
        public void TryPattern(Stream pattern)
        {
            var pObj = Serializer.FromPost<Pattern>(pattern);
            State.PatternSim.Clear();
            State.PatternSim.AddPattern(pObj, 0);
        }

    }
}
