using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Windows.Media;
using KineticControl;
using Newtonsoft.Json;
using RevKitt.KS.KineticEnvironment;
using RevKitt.KS.KineticEnvironment.Coloring;
using RevKitt.KS.KineticEnvironment.Scenes;

namespace KineticUI.JSConverters
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
            if(reader.ValueType.Equals(typeof(int)))
                return ColorUtil.FromInt((int)reader.Value);
            return Colors.Black;
            //var nullable = reader.ReadAsInt32();
            //int color = 0;
            //if (nullable.HasValue)
            //    color = nullable.Value;
            //return ColorUtil.FromInt(color);

        }

        public override bool CanConvert(Type objectType)
        {
            return typeof (Color).IsAssignableFrom(objectType);
        }
    }
}