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
    }
}