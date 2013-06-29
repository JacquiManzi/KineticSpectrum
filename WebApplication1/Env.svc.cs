using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Text;
using System.Web.SessionState;
using KineticControl;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RevKitt.KS.KineticEnvironment;
using RevKitt.KS.KineticEnvironment.Scenes;
using WebApplication1.JSConverters;

namespace WebApplication1
{
    [ServiceContract(Namespace = "")]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class SceneService
    {
        static SceneService()
        {
            Serializer = new JsonSerializer();
            Serializer.Converters.Add(new LightAddressConverter());
            Serializer.Converters.Add(new GroupConverter());
        }

        public static readonly JsonSerializer Serializer;
        // To use HTTP GET, add [WebGet] attribute. (Default ResponseFormat is WebMessageFormat.Json)
        // To create an operation that returns XML,
        //     add [WebGet(ResponseFormat=WebMessageFormat.Xml)],
        //     and include the following line in the operation body:
        //         WebOperationContext.Current.OutgoingResponse.ContentType = "text/xml";
        [OperationContract]
        [WebGet]
        public void EditGroup(string group)
        {
            Group groupObj = Serializer.Deserialize<Group>(new JsonTextReader(new StringReader(group)));
            State.Scene.SetGroup(groupObj);
        }

        [OperationContract]
        [WebGet]
        public void DeleteGroup(string groupName)
        {
            State.Scene.DeleteGroup(groupName);
        }

        [OperationContract]
        [WebGet]
        public void RenameGroup(string oldName, string newGroup)
        {
            Group groupObj = Serializer.Deserialize<Group>(new JsonTextReader(new StreamReader(newGroup)));
            State.Scene.RenameGroup(oldName, groupObj);
        }

        [OperationContract]
        [WebGet]
        public void SetPattern(string pattern)
        {
            var patternObj = Serializer.Deserialize<Pattern>(new JsonTextReader(new StringReader(pattern)));
            State.Scene.SetPattern(patternObj);
        }

        [OperationContract]
        [WebGet]
        public void DeletePattern(string patternName)
        {
            State.Scene.DeletePattern(patternName);
        }

        [OperationContract]
        [WebGet]
        public void RenamePattern(string oldName, string newPattern)
        {
            var patternObj = Serializer.Deserialize<Pattern>(new JsonTextReader(new StringReader(newPattern)));
            State.Scene.RenamePattern(oldName, patternObj);
        }

        [OperationContract]
        [WebGet]
        public string GetGroups()
        {
            var sw = new StringWriter();
            Serializer.Serialize(sw, State.Scene.Groups);
            return sw.ToString();
        }

        [OperationContract]
        [WebGet]
        public string GetPatterns()
        {
            var sw = new StringWriter();
            Serializer.Serialize(sw, State.Scene.Patterns);
            return sw.ToString();
        }

        [OperationContract]
        [WebGet]
        public string GetSelectedGroups()
        {
            var sw = new StringWriter();
            Serializer.Serialize(sw, State.Scene.SelectedGroups.Select(g=>g.Name));
            return sw.ToString();
        }

        [OperationContract]
        [WebGet]
        public string GetLEDNodes()
        {
            var sw = new StringWriter();
            Serializer.Serialize(sw, LightSystemProvider.Lights);
            return sw.ToString();
        }

        [OperationContract]
        [WebGet]
        public string GetGroup(string groupName)
        {
            var sw = new StringWriter();
            Serializer.Serialize(sw, State.Scene.Groups.First(g=>groupName.Equals(g.Name)));
            return sw.ToString();
        }

        [OperationContract]
        [WebGet]
        public string GetPattern(string patternName)
        {
            var sw = new StringWriter();
            Serializer.Serialize(sw, State.Scene.Patterns.First(p=>patternName.Equals(p.Name)));
            return sw.ToString();
        }

        [OperationContract]
        [WebGet]
        public void SelectGroups(string groupNames)
        {
            var names = Serializer.Deserialize<IEnumerable<string>>(new JsonTextReader(new StringReader(groupNames)));
            State.Scene.SelectGroups(names);
        }

        [OperationContract]
        [WebGet]
        public void SelectLights(string lightAddresses)
        {
            var las = Serializer.Deserialize<IEnumerable<LightAddress>>(new JsonTextReader(new StringReader(lightAddresses)));
            State.Scene.SelectLights(las);
        }

        

        // Add more operations here and mark them with [OperationContract]
    }
}
