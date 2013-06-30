using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Windows.Media;
using KineticControl;
using Newtonsoft.Json;
using RevKitt.KS.KineticEnvironment;
using RevKitt.KS.KineticEnvironment.Scenes;

namespace WebApplication1.JSConverters
{
    public class ColorConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var la = (Color)value;
            writer.WriteValue(ColorUtil.ToInt(la));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
                                        JsonSerializer serializer)
        {

            return ColorUtil.FromInt(reader.ReadAsInt32().Value);

        }

        public override bool CanConvert(Type objectType)
        {
            return typeof (Color).IsAssignableFrom(objectType);
        }
    }
}