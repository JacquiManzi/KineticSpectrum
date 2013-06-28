using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using KineticControl;

namespace WebApplication1.JSConverters
{
    public class LightAddressConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var la = (LightAddress) value;
            writer.WriteStartObject();
            writer.WritePropertyName("FixtureNo");
            writer.WriteValue(la.FixtureNo);
            writer.WritePropertyName("PortNo");
            writer.WriteValue(la.PortNo);
            writer.WritePropertyName("LightNo");
            writer.WriteValue(la.LightNo);
            writer.WriteEnd();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
                                        JsonSerializer serializer)
        {
            int fixture=-1, port=-1, light=-1;
            while(reader.Read())
            {
                if(reader.Value != null)
                {
                    if(reader.TokenType == JsonToken.PropertyName)
                    {
                        string pName = (string)reader.Value;
                        reader.Read();
                        int val = int.Parse((string) reader.Value);
                        if (pName.Equals("FixtureNo"))
                            fixture = val;
                        if (pName.Equals("LightNo"))
                            light = val;
                        if (pName.Equals("PortNo"))
                            port = val;
                    }
                }
            }
            return new LightAddress(fixture, port, light);

        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof (LightAddress);
        }
    }
}