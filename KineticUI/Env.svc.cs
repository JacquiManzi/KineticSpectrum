/*
*   @Author: Jacqui Manzi
*    September 13th, 2013
*    jacqui@revkitt.com
*
*   Env.svc -  
*/

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using KineticControl;
using Newtonsoft.Json;
using RevKitt.KS.KineticEnvironment;
using RevKitt.KS.KineticEnvironment.JSConverters;
using RevKitt.KS.KineticEnvironment.Scenes;

namespace KineticUI
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
            IGroup groupObj = Serializer.FromPost<IGroup>(group);
            State.Scene.SetGroup(groupObj);
            Saver.LocalSave();
        }

        [OperationContract]
        public Stream AddLED(Stream ledStream)
        {
            LEDNode node = Serializer.FromPost<LEDNode>(ledStream);
            var ledAddress = LightSystemProvider.AddLED(node);
            Saver.LocalSave();
            return Serializer.ToStream(ledAddress);
        }

        [OperationContract]
        public Stream AddLEDs(Stream ledStream)
        {
            IEnumerable<LEDNode> node = Serializer.FromPost<IEnumerable<LEDNode>>(ledStream);
            var addresses = LightSystemProvider.AddLEDs(node);
            Saver.LocalSave();
            return Serializer.ToStream(addresses);
        }

        [OperationContract]
        public void RemoveLED(Stream addressStream)
        {
            LightAddress node = Serializer.FromPost<LightAddress>(addressStream);
            LightSystemProvider.RemoveLED(node);
            Saver.LocalSave();
        }

        [OperationContract]
        public void SetPattern(Stream pattern)
        {
            var patternObj = Serializer.FromPost<Pattern>(pattern);
            State.Scene.SetPattern(patternObj);
            Saver.LocalSave();
        }

        [OperationContract]
        [WebGet]
        public void DeleteGroup(string groupName)
        {
            State.Scene.DeleteGroup(groupName);
            Saver.LocalSave();
        }

        [OperationContract]
        [WebGet]
        public void RenameGroup(string oldName, string newGroup)
        {
            IGroup groupObj = Serializer.Ser.Deserialize<IGroup>(new JsonTextReader(new StreamReader(newGroup)));
            State.Scene.RenameGroup(oldName, groupObj);
            Saver.LocalSave();
        }

        [OperationContract]
        [WebGet]
        public void DeletePattern(string patternName)
        {
            State.Scene.DeletePattern(patternName);
            Saver.LocalSave();
        }

        [OperationContract]
        [WebGet]
        public void RenamePattern(string oldName, Stream newPattern)
        {
            var patternObj = Serializer.FromStream<Pattern>(newPattern);
            State.Scene.RenamePattern(oldName, patternObj);
            Saver.LocalSave();
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
        public Stream GetFixtures()
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
        public void SelectGroups(Stream groupNames)
        {
            var names = Serializer.FromPost<IEnumerable<string>>(groupNames);
            //var names = Serializer.Deserialize<IEnumerable<string>>(new JsonTextReader(new StringReader(groupNames)));
            State.Scene.SelectGroups(names);
        }

        [OperationContract]
        public void SelectLights(Stream lightAddresses)
        {
            var las = Serializer.FromPost<IEnumerable<LightAddress>>(lightAddresses);
            State.Scene.SelectLights(las);
        }

        [OperationContract]
        [WebGet]
        public Stream Save()
        {
            Stream s = new MemoryStream();
            StreamWriter writer = new StreamWriter(s);
            Saver.Save(writer);
            s.Position = 0;
            return s;
        }
    }
}
