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
using RevKitt.KS.KineticEnvironment.Effects.Order;
using RevKitt.KS.KineticEnvironment.Tweening;
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

        [OperationContract]
        [WebGet]
        public Stream GetOrderingTypes()
        {
            return Serializer.ToStream(OrderingTypes.All);
        }

        [OperationContract]
        [WebGet]
        public Stream GetRepeatMethods()
        {
            return Serializer.ToStream(RepeatMethods.All);
        }

        [OperationContract]
        [WebGet]
        public Stream GetEasings()
        {
            return Serializer.ToStream(Easings.EasingNameMap.Select(k=>k.Key));
        }

        [OperationContract]
        [WebGet]
        public Stream GetOrderingForType(string type)
        {
            return Serializer.ToStream(OrderingTypes.GetOrderingSubTypes(type));
        }

        // Add more operations here and mark them with [OperationContract]
    }
}
