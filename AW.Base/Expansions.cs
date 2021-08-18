using AW.Serializer;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace AW
{
    public static class Expansions
    {
        #region Type

        public static object Object(this Type type, object[] @params = null)
        {
            return SerializerHelper.GetObject(type, @params);
        }


        public static T Object<T>(this Type type, object[] @params = null)
        {
            return (T)SerializerHelper.GetObject(type, @params);
        }

        #endregion


        #region String

        public static int Int(this string value)
        {
            return int.Parse(value);
        }


        public static bool TryInt(this string value, out int result)
        {
            return int.TryParse(value, out result);
        }


        public static long Long(this string value)
        {
            return long.Parse(value);
        }


        public static bool TryLong(this string value, out long result)
        {
            return long.TryParse(value, out result);
        }


        public static double Double(this string value)
        {
            return double.Parse(value.Replace(',', '.'));
        }


        public static bool TryDouble(this string value, out double result)
        {
            return double.TryParse(value.Replace(',', '.'), out result);
        }


        public static DateTime Date(this string value, string format = null, IFormatProvider provider = null)
        {
            if (string.IsNullOrEmpty(format))
                return DateTime.Parse(value);

            return DateTime.ParseExact(value, format, provider ?? CultureInfo.InvariantCulture);
        }


        public static Type Type(this string value)
        {
            return SerializerHelper.GetType(value);
        }


        public static bool IsNull(this string value)
        {
            return string.IsNullOrEmpty(value);
        }


        public static string UniqueFrom(this string value, IEnumerable<string> values)
        {
            var index = 1;
            var newValue = value;

            while (values.Any(v => v.Equals(newValue, StringComparison.InvariantCultureIgnoreCase)))
            {
                newValue = $"{value}#{index}";
                index++;
            }

            return newValue;
        }


        public static bool AnyFrom(this string value, IEnumerable<string> values)
            => values.Any(v => v.Equals(value, StringComparison.InvariantCultureIgnoreCase));

        #endregion


        #region Enum

        public static IEnumerable<T> GetValues<T>(this T @enum) where T : Enum
        {
            return Enum.GetValues(@enum.GetType()).Cast<T>();
        }


        public static string GetName<T>(this T @enum) where T : Enum
        {
            return Enum.GetName(@enum.GetType(), @enum);
        }

        #endregion


        #region IEnumerable

        public static IEnumerable<IEnumerable<T>> Groups<T>(this IEnumerable<T> source, int groupCount)
        {
            var count = source.Count();
            var e = (int)Math.Floor(count / (double)groupCount);
            var ae = count - e * groupCount;

            var skip = 0;
            for (var i = 0; i < groupCount; ++i)
            {
                var c = i < ae ? e + 1 : e;
                yield return source.Skip(skip).Take(c);

                skip += c;
            }
        }

        #endregion


        #region Serializer

        public static string Serialize(this object obj)
        {
            var serializer = new AWSerializer();
            return serializer.Serialize(obj);
        }


        public static T Deserialize<T>(this string data) where T : class
        {
            var serializer = new AWSerializer();
            return serializer.Deserialize<T>(data);
        }

        #endregion
    }
}
