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
using System.ServiceModel.Channels;
using System.Net;

namespace WebApplication1
{
    [ServiceContract(Namespace = "")]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class SceneService
    {
        
        // To use HTTP GET, add [WebGet] attribute. (Default ResponseFormat is WebMessageFormat.Json)
        // To create an operation that returns XML,
        //     add [WebGet(ResponseFormat=WebMessageFormat.Xml)],
        //     and include the following line in the operation body:
        //         WebOperationContext.Current.OutgoingResponse.ContentType = "text/xml";
        [OperationContract]
        public void EditGroup(Stream group)
        {
            Group groupObj = Serializer.FromPost<Group>(group);
            State.Scene.SetGroup(groupObj);
        }

        [OperationContract]
        public void AddLED(Stream ledStream)
        {
            LEDNode node = Serializer.FromStream<LEDNode>(ledStream);
            LightSystemProvider.AddLED(node);
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
            Group groupObj = Serializer.Ser.Deserialize<Group>(new JsonTextReader(new StreamReader(newGroup)));
            State.Scene.RenameGroup(oldName, groupObj);
        }

        [OperationContract]
        [WebGet]
        public void SetPattern(Stream pattern)
        {
            var patternObj = Serializer.FromStream<Pattern>(pattern);
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
        public void RenamePattern(string oldName, Stream newPattern)
        {
            var patternObj = Serializer.FromStream<Pattern>(newPattern);
            State.Scene.RenamePattern(oldName, patternObj);
        }

        [OperationContract]
        [WebGet]
        public Stream GetGroups()
        {
            return Serializer.ToStream(State.Scene.Groups);
        }

        [OperationContract]
        [WebGet]
        public Stream GetGroupNames()
        {
            return Serializer.ToStream(State.Scene.Groups.Select(g => g.Name));
        }

        [OperationContract]
        [WebGet]
        public Stream GetPatterns()
        {
            return Serializer.ToStream(State.Scene.Patterns);
        }

        [OperationContract]
        [WebGet]
        public Stream GetPatterns()
        {
            return Serializer.ToStream(LightSystemProvider.getFixtures());
        }

        [OperationContract]
        [WebGet]
        public Stream GetPatternNames()
        {
            return Serializer.ToStream(State.Scene.Patterns.Select(p=>p.Name));
        }

        [OperationContract]
        [WebGet]
        public Stream GetSelectedGroups()
        {
            return Serializer.ToStream(State.Scene.SelectedGroups.Select(g => g.Name));
        }

        [OperationContract]
       // [WebGet(BodyStyle = WebMessageBodyStyle.Bare)]
        [WebGet]
        public Stream GetLEDNodes()
        {
            return Serializer.ToStream(LightSystemProvider.Lights);
        }

        [OperationContract]
        [WebGet]
        public Stream GetGroup(string groupName)
        {
            return Serializer.ToStream(State.Scene.Groups.First(g => groupName.Equals(g.Name)));
        }

        [OperationContract]
        [WebGet]
        public Stream GetPattern(string patternName)
        {
            return Serializer.ToStream(State.Scene.Patterns.First(p=>patternName.Equals(p.Name)));
        }

        [OperationContract]
        [WebGet]
        public void SelectGroups(IEnumerable<string> groupNames)
        {
            //var names = Serializer.Deserialize<IEnumerable<string>>(new JsonTextReader(new StringReader(groupNames)));
            State.Scene.SelectGroups(groupNames);
        }

        [OperationContract]
        [WebGet]
        public void SelectLights(Stream lightAddresses)
        {
            var las = Serializer.FromStream<IEnumerable<LightAddress>>(lightAddresses);
            State.Scene.SelectLights(las);
        }

        [OperationContract]
        [WebGet]
        public Stream Save()
        {
            Stream s = new MemoryStream();
            Serializer.Ser.Formatting = Formatting.Indented;
            string groups = Serializer.ToString(State.Scene.Groups);
            string patterns = Serializer.ToString(State.Scene.Patterns);
            Serializer.Ser.Formatting = Formatting.None;

            var writer = new StreamWriter(s);
            writer.WriteLine("\n\n### Groups");
            writer.Write(groups);

            writer.WriteLine("\n\n### Patterns");
            writer.WriteLine(patterns);
            writer.Flush();

            s.Position = 0;
            return s;
        }



        

        // Add more operations here and mark them with [OperationContract]
    }
}
