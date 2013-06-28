using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Text;
using Newtonsoft.Json;
using RevKitt.KS.KineticEnvironment.Effects;
using RevKitt.KS.KineticEnvironment.Effects.ColorEffects;

namespace WebApplication1
{
    [ServiceContract(Namespace = "")]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class Effects
    {
      
        // To use HTTP GET, add [WebGet] attribute. (Default ResponseFormat is WebMessageFormat.Json)
        // To create an operation that returns XML,
        //     add [WebGet(ResponseFormat=WebMessageFormat.Xml)],
        //     and include the following line in the operation body:
        //         WebOperationContext.Current.OutgoingResponse.ContentType = "text/xml";
        [OperationContract]
        [WebGet]
        public IEnumerable<string> GetEffects()
        {
            // Add your operation implementation here
             return EffectRegistry.Effects;
        }

        [OperationContract]
        [WebGet]
        public string GetEffectDef(string effectName)
        {
            return JsonConvert.SerializeObject(EffectRegistry.GetProperties(effectName));
        }

        public string GetColorEffects()
        {
            return JsonConvert.SerializeObject(ColorEffectDefinition.AllDefaults.Select(d => d.Name));
        }

        // Add more operations here and mark them with [OperationContract]
    }
}
