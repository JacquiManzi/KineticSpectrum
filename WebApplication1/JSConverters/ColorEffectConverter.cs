using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Windows.Media;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RevKitt.KS.KineticEnvironment;
using RevKitt.KS.KineticEnvironment.Effects.ColorEffect;

namespace WebApplication1.JSConverters
{
    public class ColorEffectConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var val = (IColorEffect) value;
            var nameProp = new JProperty("name", val.Name);

            var jobj = new JObject(nameProp);

            var objWriter = jobj.CreateWriter();
            if (value is FixedColor)
            {
                objWriter.WritePropertyName("color");
                serializer.Serialize(objWriter, ((FixedColor) val).Colors[0]);
            }
            else
            {
                objWriter.WritePropertyName("colors");
                //serializer.Serialize(objWriter, ((ColorFade)val).Colors.Select(c => new JValue(c)));
                serializer.Serialize(objWriter, val.Colors);
            }
            jobj.WriteTo(writer);
        } 

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var jobj = JObject.Load(reader);
            string type = (string)jobj["type"];
            if (type == null)
                type = (string) jobj["name"];

            return ColorEffects.GetEffect(type, ConvertColorList(jobj));
//            if (type.Equals(FixedColor.EffectName))
//            {
//                if(jobj["color"].Type != JTokenType.Integer)
//                    throw new JsonReaderException("Invalid Fixed Color Effect. 'color' property must be an int, but was: " + jobj["color"]);
//                var colorInt = (int) jobj["color"];
//                var color = ColorUtil.FromInt(colorInt);
//                return new FixedColor(color);
//            }
//
//            if (type.Equals(ColorFade.EffectName))
//            {
//                if(jobj["colors"].Type != JTokenType.Array)
//                    throw new JsonReaderException("Invalid Color Fade effect. 'colors' property must be a color array, but was: " + jobj["colors"]);
//
//                JArray jarr = (JArray) jobj["colors"];
//                if(jarr.Any(t => t.Type != JTokenType.Integer))
//                    throw new JsonReaderException("Invalid Color Fade effect. 'colors' property must be a color array, but was:" +jobj["colors"]);
//                var colors = jarr.Select(t => ColorUtil.FromInt((int) t));
//                return new ColorFade(colors);
//            }
//
//            throw new JsonReaderException("Invalid Color Effect: " + type);
        }

        private IList<Color> ConvertColorList(JObject parent)
        {
            JToken colorObj = parent["color"];
            if (colorObj == null)
                colorObj = parent["colors"];

            if(colorObj == null)
                throw new JsonReaderException("JSON object does not define color or colors for ColorEffect");
            if (colorObj.Type == JTokenType.Integer)
                return new List<Color>{ColorUtil.FromInt((int) colorObj)};
            if (colorObj.Type == JTokenType.Array)
                return colorObj.Select(t => ColorUtil.FromInt((int) t)).ToList();
            throw new ArgumentException("JSON object colors is not a valid type");
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof (IColorEffect).IsAssignableFrom(objectType);
        }
    }
}