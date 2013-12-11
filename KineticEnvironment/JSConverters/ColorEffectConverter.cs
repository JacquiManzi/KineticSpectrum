using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RevKitt.KS.KineticEnvironment.Coloring;
using RevKitt.KS.KineticEnvironment.Effects.ColorEffect;

namespace RevKitt.KS.KineticEnvironment.JSConverters
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
            if (value is ImageEffect)
            {
                objWriter.WritePropertyName("imageName");
                objWriter.WriteValue(((ImageEffect)value).ImageName);
                objWriter.WritePropertyName("width");
                objWriter.WriteValue(((ImageEffect)value).Width);
            }
            objWriter.WritePropertyName("colors");
            //serializer.Serialize(objWriter, ((ColorFade)val).Colors.Select(c => new JValue(c)));
            serializer.Serialize(objWriter, val.Colors);
            jobj.WriteTo(writer);
        } 

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var jobj = JObject.Load(reader);
            string type = (string)jobj["type"];
            if (type == null)
                type = (string) jobj["name"];

            IColorEffect colorEffect =  ColorEffects.GetEffect(type, ConvertColorList(jobj));

            ImageEffect imageEffect = colorEffect as ImageEffect;
            if (imageEffect != null)
            {
                imageEffect.ImageName = (string) jobj["imageName"];
                imageEffect.Width = (double) jobj["width"];
            }

            return colorEffect;
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