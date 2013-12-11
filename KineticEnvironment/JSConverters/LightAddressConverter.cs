using System;
using KineticControl;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace RevKitt.KS.KineticEnvironment.JSConverters
{
    public class LightAddressConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var la = (LightAddress) value;
            writer.WriteStartObject();
            writer.WritePropertyName("fixtureNo");
            serializer.Serialize(writer, la.FixtureNo);
//            writer.WriteValue(la.FixtureNo);
            writer.WritePropertyName("portNo");
            serializer.Serialize(writer, la.PortNo);
//            writer.WriteValue(la.PortNo);
            writer.WritePropertyName("lightNo");
            serializer.Serialize(writer, la.LightNo);
//            writer.WriteValue(la.LightNo);
            writer.WriteEndObject();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
                                        JsonSerializer serializer)
        {

            JObject obj = JObject.Load(reader);
            return FromObj(obj);
        }

        public static LightAddress FromObj(JToken obj)
        {
            return new LightAddress((int) obj["fixtureNo"],
                                    (int) obj["portNo"],
                                    (int) obj["lightNo"]);
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof (LightAddress).IsAssignableFrom(objectType);
        }
    }
}