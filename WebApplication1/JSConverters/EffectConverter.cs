using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RevKitt.KS.KineticEnvironment.Effects;
using RevKitt.KS.KineticEnvironment.Effects.ColorEffects;
using RevKitt.KS.KineticEnvironment.Effects.Order;
using RevKitt.KS.KineticEnvironment.Tweening;

namespace WebApplication1.JSConverters
{
    public class EffectConverter : JsonConverter
    {
        private readonly EffectAttributes _attributes;

        public EffectConverter(EffectAttributes attributes)
        {
            _attributes = attributes;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) {}

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            EffectProperties props = new EffectProperties();
            JObject effectProps = JObject.Load(reader);
            foreach (var propDef in _attributes.PropertyMap.Values)
            {
                JToken value = TryGetToken(effectProps, propDef);
                if (null == value)
                {
                    continue;
                }
                if (propDef.PropertyType.Equals(EffectPropertyTypes.Time)
                    || propDef.PropertyType.Equals(EffectPropertyTypes.Float))
                {
                    props[propDef.Name] = TryConvert<double>(ConvertFloat, value, propDef);
                }
                else if (propDef.PropertyType.Equals(EffectPropertyTypes.Int))
                    props[propDef.Name] = TryConvert<int>(ConvertInt, value, propDef);
                else if (propDef.PropertyType.Equals(EffectPropertyTypes.Ordering))
                    props[propDef.Name] = TryConvert<IOrdering>(ConvertOrdering, value, propDef);
                else if (propDef.PropertyType.Equals(EffectPropertyTypes.RepeatMethod))
                    props[propDef.Name] = TryConvert(ConvertRepeatMethod, value, propDef);
                else if (propDef.PropertyType.Equals(EffectPropertyTypes.Easing))
                    props[propDef.Name] = TryConvert<IEasing>(ConvertEasing, value, propDef);
                else if (propDef.PropertyType.Equals(EffectPropertyTypes.ColorEffect))
                {
                    props[propDef.Name] = Serializer.Ser.Deserialize<IColorEffect>(new JTokenReader(value)); 
                }
            }
            return props;
        }

        private JToken TryGetToken(JObject effectProps, PropertyDefinition propDef)
        {
            JToken value = effectProps[propDef.Name];
            if (value == null)
            {
                char[] propDefChars = propDef.Name.ToCharArray();
                propDefChars[0] = char.ToLower(propDefChars[0]);
                string propDefName = new string(propDefChars);

                value = effectProps[propDefName];
            }
            if (null == value && !propDef.IsOptional)
            {
                throw new JsonReaderException("Invalid Effect properties for effect '" + _attributes.Name +
                                                  "', Required property '" + propDef.Name + "' is missing.");
            }
            return value;
        }

        private T TryConvert<T>(Convert<T> convert, JToken obj, PropertyDefinition propDef)
        {
            T oObj;
            if (!convert(obj, out oObj))
                throw GetException(propDef, obj);
            return oObj;
        }

        private delegate bool Convert<T>(JToken obj, out T ret);


        private readonly Convert<string> ConvertRepeatMethod = (JToken token, out string ret) => ConvertValidString(token, out ret,
                                                                                                           RepeatMethods.All);
        
        private bool ConvertFloat(JToken obj, out double flt)
        {
            flt = -1;
            if (obj.Type != JTokenType.Float && obj.Type != JTokenType.Integer)
                return false;
            flt = (double) obj;
            return true;
        }

        private bool ConvertInt(JToken obj, out int integ)
        {
            integ = -1;
            if (obj.Type != JTokenType.Integer)
                return false;
            integ = (int) obj;
            return true;
        }

        private bool ConvertOrdering(JToken obj, out IOrdering ordering)
        {
            ordering = null;
            if (obj.Type != JTokenType.Object)
                return false;
            var jobj = (JObject) obj;
            if (jobj[Orderings.OrderingTypeKey].Type != JTokenType.String)
                return false;
            if (jobj[Orderings.OrderingKey].Type != JTokenType.String)
                return false;

            ordering = Orderings.GetOrdering((string) jobj[Orderings.OrderingTypeKey], (string) jobj[Orderings.OrderingKey]);
            return true;
        }

        private static bool ConvertString(JToken obj, out string str)
        {
            str = null;
            if (obj.Type != JTokenType.String)
                return false;
            str = (string) obj;
            return true;
        }

        private static bool ConvertValidString(JToken obj, out string str, IEnumerable<string> validValues )
        {
            if (!ConvertString(obj,out str))
                return false;
            return validValues.Contains(str);
        }

        private static bool ConvertEasing(JToken obj, out IEasing easing)
        {
            easing = null;
            string name;
            if (obj.Type != JTokenType.Object)
                return false;
            if (obj["name"] == null)
                return false;
            if (!ConvertString(obj["name"], out name))
                return false;
            if (!Easings.IsEasingNameValid(name))
                return false;
            easing = Easings.GetEasingForName(name);
            return true;
        }


        private  Exception GetException(PropertyDefinition propDef, object obj)
        {
            throw new JsonReaderException("Property '"+propDef.Name+"' for effect '"+_attributes.Name+"'  should have value type '" + propDef.PropertyType +"' but had value '"+ obj + "'.");
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof (EffectProperties).IsAssignableFrom(objectType);
        }
    }
}