using System;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RevKitt.KS.KineticEnvironment.Effects;
using RevKitt.KS.KineticEnvironment.Scenes;

namespace RevKitt.KS.KineticEnvironment.JSConverters
{
    public class PatternConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var pattern = (Pattern) value;

            var jobj = new JObject(new JProperty("name", pattern.Name),
                                   new JProperty("groups", new JArray(pattern.Groups.Select(g => new JValue(g)))),
                                   new JProperty("effectName", pattern.EffectName),
                                   new JProperty("priority", pattern.Priority));

            var jwriter = jobj.CreateWriter();
            jwriter.WritePropertyName("effectProperties");
            serializer.Serialize(jwriter, pattern.EffectProperties.Properties);
           
            jobj.WriteTo(writer, serializer.Converters.ToArray());
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
                                        JsonSerializer serializer)
        {
            JObject obj = JObject.Load(reader);
            var name = (string) obj["name"];
            var groups = obj["groups"].Select(t => (string) t);
            var effectName = (string) obj["effectName"];
            var priority = (int) obj["priority"];

            //build the effect properties
            var attributes = EffectRegistry.EffectAttributes[effectName];
            var ser = BuildSerializer(attributes);
            var effectProps = ser.Deserialize<EffectProperties>(new JTokenReader(obj["effectProperties"]));

            return new Pattern
                       {
                           EffectName = effectName,
                           EffectProperties = effectProps,
                           Groups = groups.ToList(),
                           Name = name,
                           Priority = priority
                       };
        }

        private static JsonSerializer BuildSerializer(EffectAttributes attributes)
        {
            var ser = new JsonSerializer();
            ser.Converters.Add(new EffectConverter(attributes));
            return ser;
        }


        public override bool CanConvert(Type objectType)
        {
            return typeof (Pattern).IsAssignableFrom(objectType);
        }
    }
}