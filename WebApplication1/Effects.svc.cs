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
using System.IO;
using WebApplication1.JSConverters;

namespace WebApplication1
{
    [ServiceContract(Namespace = "")]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class Effects
    {

        [OperationContract]
        [WebGet]
        public Stream GetEffects()
        {
             return Serializer.ToStream(EffectRegistry.Effects);
        }

        [OperationContract]
        [WebGet]
        public Stream GetEffectDef(string effectName)
        {
            return Serializer.ToStream(EffectRegistry.GetProperties(effectName));
        }

        [OperationContract]
        [WebGet]
        public Stream GetColorEffects()
        {
            return Serializer.ToStream(ColorEffectDefinition.AllDefaults.Select(d => d.Name));
        }

        // Add more operations here and mark them with [OperationContract]
    }
}
