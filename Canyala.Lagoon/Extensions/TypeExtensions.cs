using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Canyala.Lagoon.Extensions
{
    public static class TypeExtensions
    {
        public static T MinValue<T>(this Type type)
        {
            if (type == typeof(DateTime))
                return (T)(object)DateTime.MinValue;

            if (type == typeof(double))
                return (T)(object)double.MinValue;

            if (type == typeof(float))
                return (T)(object)float.MinValue;

            if (type == typeof(long))
                return (T)(object)Int32.MinValue;

            if (type == typeof(int))
                return (T)(object)Int32.MinValue;

            return default(T);
        }

        public static T MaxValue<T>(this Type type)
        {
            if (type == typeof(DateTime))
                return (T)(object)DateTime.MaxValue;

            if (type == typeof(double))
                return (T)(object)double.MaxValue;

            if (type == typeof(float))
                return (T)(object)float.MaxValue;

            if (type == typeof(long))
                return (T)(object)Int32.MaxValue;

            if (type == typeof(int))
                return (T)(object)Int32.MaxValue;

            return default(T);
        }

    }
}
