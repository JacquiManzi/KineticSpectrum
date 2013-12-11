using System;
using System.Collections.Generic;
using System.Linq;
using KineticControl;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RevKitt.KS.KineticEnvironment.Scenes;

namespace RevKitt.KS.KineticEnvironment.JSConverters
{
    public class GroupConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var la = (IGroup)value;
            writer.WriteStartObject();
            writer.WritePropertyName("name");
            writer.WriteValue(la.Name);
            writer.WritePropertyName("lights");
            serializer.Serialize(writer, la.Lights);
            writer.WriteEnd();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
                                        JsonSerializer serializer)
        {

            JObject groupObj = JObject.Load(reader);
            string name = (string)groupObj["name"];
//            JArray lightArray = (JArray) groupObj["lights"].Sel;

            IEnumerable<LightAddress> addresses = groupObj["lights"].Select(LightAddressConverter.FromObj);
            return new GroupStub(name, addresses);


//            if(reader.TokenType != JsonToken.StartObject)
//                throw new JsonReaderException("Expected start of object");
//            reader.Read();
//
//            while (reader.TokenType != JsonToken.EndObject && reader.Read())
//            {
//                if(reader.TokenType == JsonToken.EndObject)
//                if (reader.Value != null)
//                {
//                    if (reader.TokenType == JsonToken.PropertyName)
//                    {
//                        string pName = (string)reader.Value;
//                        reader.Read();
//                        if (pName.Equals("name"))
//                        {
//                            name = (string)reader.Value;
//                        }
//                        if (pName.Equals("lights"))
//                            addresses = serializer.Deserialize<IEnumerable<LightAddress>>(reader);
//                    }
//                }
//            }

//            if(addresses == null)
//                throw new JsonReaderException("Illegal Group object. No property 'lights' containing an array of light addresses");

        }

        public override bool CanConvert(Type objectType)
        {
            return typeof (IGroup).IsAssignableFrom(objectType);
        }
    }
}