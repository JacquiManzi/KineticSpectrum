using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Windows.Media.Media3D;
using KineticControl;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace WebApplication1.JSConverters
{
    public class Vector3Converter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var la = (Vector3D)value;
            writer.WriteStartObject();
            writer.WritePropertyName("X");
            serializer.Serialize(writer, la.X);
            //            writer.WriteValue(la.FixtureNo);
            writer.WritePropertyName("Y");
            serializer.Serialize(writer, la.Y);
            //            writer.WriteValue(la.PortNo);
            writer.WritePropertyName("Z");
            serializer.Serialize(writer, la.Z);
            //            writer.WriteValue(la.LightNo);
            writer.WriteEndObject();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
                                        JsonSerializer serializer)
        {

            JObject obj = JObject.Load(reader);
            return new Vector3D((double)obj["X"],
                                    (double)obj["Y"],
                                    (double)obj["Z"]);

        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Vector3D);
        }
    }
}