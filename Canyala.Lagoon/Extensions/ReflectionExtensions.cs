//
// Copyright (c) 2013 Canyala Innovation AB
//
// All rights reserved.
//

using Canyala.Lagoon.Functional;
using Canyala.Lagoon.Reflection;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Canyala.Lagoon.Extensions
{
    public static class ReflectionExtensions
    {
        public static string ReadableName(this Type type)
        {
            var builder = new StringBuilder();

            if (type.IsGenericType)
            {
                builder.Append(type.Name.Substring(0, type.Name.IndexOf('`')));

                builder.Append("Of");
                builder.Append(type.GetGenericArguments().Select(arg => arg.ReadableName()).Join("And"));
            }
            else
            {
                builder.Append(type.Name);
            }

            return builder.ToString();
        }

        public static IEnumerable<T> GetCustomAttributes<T>(this PropertyInfo propertyInfo)
        {
            return propertyInfo.GetCustomAttributes(typeof(T), false).Cast<T>();
        }

        public static void SetValue(this PropertyInfo propertyInfo, object instance, object value)
        {
            propertyInfo.SetValue(instance, value, null);
        }

        public static object GetValue(this PropertyInfo propertyInfo, object instance)
        {
            return propertyInfo.GetValue(instance, null);
        }

        public static IEnumerable<SemanticProperty> GetSemanticProperties(this Type type)
        {
            return type.GetProperties().Select(property => SemanticProperty.FromPropertyInfo(property));
        }

        public static object GetPropertyValue(this object instance, SemanticProperty property)
        {
            return instance.GetType().GetProperty(property.Name, property.Type).GetValue(instance);
        }

        public static object GetPropertyValue(this object instance, string propertyName)
        {
            return instance.GetType().GetRuntimeProperty(propertyName).GetValue(instance);
        }

        public static object SetPropertyValue(this object instance, SemanticProperty property, object value)
        {
            instance.GetType().GetProperty(property.Name, property.Type).SetValue(instance, value);
            return instance;
        }

        public static object SemanticCopy(this object target, object source)
        {
            target
                .GetType()
                .GetSemanticProperties()
                .Intersect(source.GetType().GetSemanticProperties())
                .Do(property => target.SetPropertyValue(property, source.GetPropertyValue(property)));

            return target;
        }

        public static ICollection<TTarget> SemanticCopy<TTarget, TSource>(this ICollection<TTarget> collection, IEnumerable<TSource> seq)
            where TTarget : new()
        {
            foreach (var item in seq)
            {
                TTarget targetItem = new TTarget();
                targetItem.SemanticCopy(item);
                collection.Add(targetItem);
            }

            return collection;
        }

        public static bool SemanticEquals(this object x, object y)
        {
            return !x
                .GetType()
                .GetSemanticProperties()
                .Intersect(y.GetType().GetSemanticProperties())
                .Any(property => Equals(x.GetPropertyValue(property), y.GetPropertyValue(property)) == false);
        }

        public static bool SemanticEquals<TOne, TTwo>(this IEnumerable<TOne> x, IEnumerable<TTwo> y)
        {
            return Seq.DoWhile(x, y, (a, b) => a.SemanticEquals(b));
        }
    }
}
