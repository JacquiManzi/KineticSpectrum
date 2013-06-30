using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Net;

namespace WebApplication1.JSConverters
{
    public class Serializer
    {
        static Serializer()
        {
            Ser = new JsonSerializer();
            foreach (var jsonConverter in Converters)
            {
                Ser.Converters.Add(jsonConverter);
            }
            Ser.ContractResolver = new CamelCasePropertyNamesContractResolver();
        }

        public static readonly IList<JsonConverter> Converters = new List<JsonConverter>
                                                                     {
                                                                         new LightAddressConverter(),
                                                                         new GroupConverter(),
                                                                         new Vector3Converter(),
                                                                         new ColorEffectConverter(),
                                                                         new ColorConverter(),
                                                                         new PatternConverter()
                                                                     }.AsReadOnly();

        public static readonly JsonSerializer Ser;

        public static Stream ToStream(object obj)
        {
            var ms = new MemoryStream();
            var sw = new StreamWriter(ms);
            Ser.Serialize(sw, obj );
            sw.Flush();
            ms.Position = 0;
            return ms;
        }
        public static string ToString(object obj)
        {
            var stream = ToStream(obj);
            return new StreamReader(stream).ReadToEnd();
        }

        public static T FromString<T>(string @string)
        {
            return Ser.Deserialize<T>(new JsonTextReader(new StringReader(@string)));
        }

        public static T FromStream<T>(Stream stream)
        {
            return Ser.Deserialize<T>(new JsonTextReader(new StreamReader(stream)));
        }

        public static T FromPost<T>(Stream stream)
        {
            string str = new StreamReader(stream).ReadToEnd();
            str = WebUtility.UrlDecode(str.Substring(2));
            return FromString<T>(str);
        }
    }
}