﻿//-------------------------------------------------------------------------------
//
//  MIT License
//
//  Copyright (c) 2012-2022 Canyala Innovation (Martin Fredriksson)
//
//  Permission is hereby granted, free of charge, to any person obtaining a copy
//  of this software and associated documentation files (the "Software"), to deal
//  in the Software without restriction, including without limitation the rights
//  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//  copies of the Software, and to permit persons to whom the Software is
//  furnished to do so, subject to the following conditions:
//
//  The above copyright notice and this permission notice shall be included in all
//  copies or substantial portions of the Software.
//
//  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//  SOFTWARE.
//
//------------------------------------------------------------------------------- 

using System.Reflection;
using System.Text;

using Canyala.Lagoon.Core.Functional;
using Canyala.Lagoon.Core.Reflection;

namespace Canyala.Lagoon.Core.Extensions;

public static class ReflectionExtensions
{
    public static string ReadableName(this Type type)
    {
        var builder = new StringBuilder();

        if (type.IsGenericType)
        {
            builder.Append(type.Name.AsSpan(0, type.Name.IndexOf('`')));

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

    public static object? GetValue(this PropertyInfo propertyInfo, object instance)
    {
        return propertyInfo.GetValue(instance, null);
    }

    public static IEnumerable<SemanticProperty> GetSemanticProperties(this Type type)
    {
        return type
            .GetProperties()
            .Select(property => SemanticProperty.FromPropertyInfo(property));
    }

    public static object? GetPropertyValue(this object instance, SemanticProperty property)
    {
        return instance
            .GetType()
            .GetProperty(property.Name, property.Type)?
            .GetValue(instance);
    }

    public static object? GetPropertyValue(this object instance, string propertyName)
    {
        return instance
            .GetType()
            .GetRuntimeProperty(propertyName)?
            .GetValue(instance);
    }

    public static object? SetPropertyValue(this object instance, SemanticProperty property, object? value)
    {
        instance
            .GetType()
            .GetProperty(property.Name, property.Type)?
            .SetValue(instance, value);

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
        where TSource : notnull
        where TTarget : new()
    {
        foreach (var item in seq)
        {
            TTarget targetItem = new();
            targetItem.SemanticCopy(item);
            collection.Add(targetItem);
        }

        return collection;
    }

    public static bool SemanticEquals(this object? x, object? y)
    {
        if (x is null || y is null)
            return false;

        return !x
            .GetType()
            .GetSemanticProperties()
            .Intersect(y.GetType().GetSemanticProperties())
            .Any(property => Equals(x.GetPropertyValue(property), y.GetPropertyValue(property)) == false);
    }

    public static bool SemanticEquals<TOne, TTwo>(this IEnumerable<TOne?> x, IEnumerable<TTwo?> y)
    {
        return Seq.DoWhile(x, y, (a, b) => a.SemanticEquals(b));
    }
}
