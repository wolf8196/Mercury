using System;

namespace Mercury.Utils
{
    public static class GenericObjectExtensions
    {
        public static T ThrowIfNull<T>(this T obj, string paramName)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(paramName);
            }

            return obj;
        }

        public static string ThrowIfNullOrEmpty(this string str, string paramName)
        {
            if (string.IsNullOrEmpty(str))
            {
                throw new ArgumentException("String is null or empty.", paramName);
            }

            return str;
        }
    }
}