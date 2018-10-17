using System;
using Newtonsoft.Json;

namespace Clamp.Common.Extensions
{
    public static class JsonExtensions
    {
        public static string ToJson(this object obj)
        {
            if (obj == null)
            {
                return string.Empty;
            }
            return JsonConvert.SerializeObject(obj);
        }

        public static T ToDeserialize<T>(this string str)
        {
            if (str == null)
            {
                return default(T);
            }
            return JsonConvert.DeserializeObject<T>(str);
        }

        public static object ToDeserialize(this string str, Type type)
        {
            if (str == null)
            {
                return null;
            }
            return JsonConvert.DeserializeObject(str, type);
        }
    }
}