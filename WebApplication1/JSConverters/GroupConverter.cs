using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using KineticControl;
using Newtonsoft.Json;
using RevKitt.KS.KineticEnvironment;
using RevKitt.KS.KineticEnvironment.Scenes;

namespace WebApplication1.JSConverters
{
    public class GroupConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var la = (Group)value;
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
            string name = null;
            IEnumerable<LightAddress> addresses = null;

            while (reader.Read())
            {
                if (reader.Value != null)
                {
                    if (reader.TokenType == JsonToken.PropertyName)
                    {
                        string pName = (string)reader.Value;
                        reader.Read();
                        if (pName.Equals("name"))
                        {
                            name = (string)reader.Value;
                        }
                        if (pName.Equals("lights"))
                            addresses = serializer.Deserialize<IEnumerable<LightAddress>>(reader);
                    }
                }
            }
            return new Group(name, LightSystemProvider.GetNodeMapping(addresses).Values);

        }

        public override bool CanConvert(Type objectType)
        {
            return typeof (Group).IsAssignableFrom(objectType);
        }
    }
}