//-------------------------------------------------------------------------------
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

using System.Globalization;
using System.Reflection;
using System.Text;

using Canyala.Lagoon.Core.Extensions;
using Canyala.Lagoon.Core.Functional;
using Canyala.Lagoon.Core.Text;

namespace Canyala.Lagoon.Core.Serialization;

/// <summary>
/// Provides json (Java Script Object Notation) serialization for value types and polymorphic composite value types.
/// </summary>
public static class Json
{
    /// <summary>
    /// Json value factory methods.
    /// </summary>
    public static class New
    {
        #region - Internal singletons -
        private static readonly True _true = new();
        private static readonly False _false = new();
        private static readonly Null _null = new();
        #endregion

        /// <summary>
        /// Creates a Json True object.
        /// </summary>
        static public Value True
        { get { return _true; } }

        /// <summary>
        /// Creates a Json False object.
        /// </summary>
        static public Value False
        { get { return _false; } }

        /// <summary>
        /// Creates a Json Null object.
        /// </summary>
        static public Null Null
        { get { return _null; } }

        /// <summary>
        /// Creates a Json String object.
        /// </summary>
        /// <param name="value">A clr string value.</param>
        /// <returns>A Json String.</returns>
        static public String String(string value)
        { return new String(value); }

        /// <summary>
        /// Creates a Json String object.
        /// </summary>
        /// <param name="value">A clr guid value.</param>
        /// <returns>A Json String.</returns>
        static public String String(Guid guid)
        { return new String(guid.ToString()); }

        /// <summary>
        /// Creates a Json String object.
        /// </summary>
        /// <param name="value">A clr datetime value.</param>
        /// <returns>A Json String.</returns>
        static public String String(DateTime dateTime)
        { return new String(dateTime.ToString("o", CultureInfo.InvariantCulture)); }

        /// <summary>
        /// Creates a Json Number object
        /// </summary>
        /// <param name="value">A clr time span value.</param>
        /// <returns>A Json String object.</returns>
        static public String String(TimeSpan value)
        { return new String(value.ToString()); }

        /// <summary>
        /// Creates a Json String object.
        /// </summary>
        /// <param name="value">A clr datetimeoffset value.</param>
        /// <returns>A Json String.</returns>
        static public String String(DateTimeOffset dateTimeOffset)
        { return new String(dateTimeOffset.ToString("yyyy-MM-ddTHH:mm:ss.fffffffzzz", CultureInfo.InvariantCulture)); }

        /// <summary>
        /// Creates a Json Number object
        /// </summary>
        /// <param name="value">A clr double value.</param>
        /// <returns>A Json Number object.</returns>
        static public Number Number(double value)
        { return new Number(value.ToString(CultureInfo.InvariantCulture)); }

        /// <summary>
        /// Creates a Json Number object
        /// </summary>
        /// <param name="value">A clr float value.</param>
        /// <returns>A Json Number object.</returns>
        static public Number Number(float value)
        { return new Number(value.ToString(CultureInfo.InvariantCulture)); }

        /// <summary>
        /// Creates a Json Number object
        /// </summary>
        /// <param name="value">A clr long value.</param>
        /// <returns>A Json Number object.</returns>
        static public Number Number(long value)
        { return new Number(value.ToString(CultureInfo.InvariantCulture)); }

        /// <summary>
        /// Creates a Json Number object
        /// </summary>
        /// <param name="value">A clr int value.</param>
        /// <returns>A Json Number object.</returns>
        static public Number Number(int value)
        { return new Number(value.ToString(CultureInfo.InvariantCulture)); }

        /// <summary>
        /// Creates a Json Name/Value pair.
        /// </summary>
        /// <param name="name">A Json String as the Name part of the pair.</param>
        /// <param name="value">A Json Value as the Value part of the pair.</param>
        /// <returns></returns>
        static public NameValue NameValue(String name, Value value)
        { return new NameValue(name, value); }

        /// <summary>
        /// Creates a Json Object.
        /// </summary>
        /// <param name="nameValuePairs">An argument list of Json NameValue pair's.</param>
        /// <returns>A Json Object.</returns>
        static public Object Object(params NameValue[] nameValuePairs)
        { return new Object(nameValuePairs); }

        /// <summary>
        /// Creates a Json Object.
        /// </summary>
        /// <param name="nameValuePairs">A sequence of Json NameValue pair's.</param>
        /// <returns>A Json Object.</returns>
        static public Object Object(IEnumerable<NameValue> nameValuePairs)
        { return new Object(nameValuePairs.ToArray()); }

        /// <summary>
        /// Creates a Json Object.
        /// </summary>
        /// <param name="declaredType">Declared type.</param>
        /// <param name="clrObject">A class object instance.</param>
        /// <returns>A Json Object</returns>
        static public Value Object(Type? declaredType, object? clrObject)
        {
            if (clrObject is null)
                throw new ArgumentNullException(nameof(clrObject));

            var actualType = clrObject.GetType();

            if (declaredType is null)
                throw new ArgumentNullException(nameof(declaredType));

            if (!declaredType.IsAssignableFrom(actualType))
                throw new InvalidCastException();

            IEnumerable<Json.NameValue>? nameValuePairs = null;

            var publicFields = actualType
                .GetFields(BindingFlags.Instance | BindingFlags.Public);

            if (!publicFields.Any())
            {
                var publicProperties = actualType.GetProperties(BindingFlags.Instance | BindingFlags.Public);

                if (!publicProperties.Any())
                    return Json.New.Null;

                nameValuePairs = publicProperties
                    .Select(property => Json.New.NameValue(Json.New.String(property.Name), Json.New.Value(property.PropertyType, property.GetValue(clrObject))));
            }
            else
                nameValuePairs = publicFields
                    .Select(field => Json.New.NameValue(Json.New.String(field.Name), Json.New.Value(field.FieldType, field.GetValue(clrObject))));

            if (declaredType == actualType)
                return Json.New.Object(nameValuePairs);

            var jsonObject = Json.New.Object(nameValuePairs);
            var jsonTypeName = Json.New.String(actualType.AssemblyQualifiedName ?? string.Empty);
            var nameValuePair = Json.New.NameValue(jsonTypeName, jsonObject);

            return Json.New.Object(nameValuePair);
        }

        /// <summary>
        /// Creates a Json Array.
        /// </summary>
        /// <param name="values">An argument list of Json Value's</param>
        /// <returns>A Json Array.</returns>
        static public Array Array(params Value[] values)
        { return new Array(values); }

        /// <summary>
        /// Creates a Json Array.
        /// </summary>
        /// <param name="values">An sequence of Json Value's</param>
        /// <returns>A Json Array.</returns>
        static public Array Array(IEnumerable<Value> values)
        { return new Array(values.ToArray()); }

        /// <summary>
        /// Creates a Json Array.
        /// </summary>
        /// <param name="array">A clr array</param>
        /// <returns>A Json Array</returns>
        static public Array Array(System.Array array)
        {
            var type = array.GetType();

            if (array.Rank == 1)
                return Json.New.Array(array.Map(element => Json.New.Value(type.GetElementType(), element)));

            var lengths = new int[array.Rank];
            for (int dimension = 0; dimension < array.Rank; dimension++)
                lengths[dimension] = array.GetLength(dimension);

            var dimensions = Json.New.Array(lengths.Map(length => Json.New.Value(typeof(int), length)));
            var elements = Json.New.Array(array.Map(element => Json.New.Value(type.GetElementType(), element)));

            return Json.New.Array(dimensions, elements);
        }

        /// <summary>
        /// Creates a Json Value.
        /// </summary>
        /// <param name="clrObject">A clr object to create a Json Value from.</param>
        /// <returns>A Json Value</returns>
        static public Value Value(Type? declaredType, object? clrObject)
        {
            if (clrObject == null)
                return Json.New.Null;

            var actualType = clrObject.GetType();

            if (actualType.IsEnum)
                return Json.New.String(clrObject.ToString() ?? string.Empty);

            if (actualType.IsArray && actualType.HasElementType)
                return Json.New.Array((System.Array)clrObject);

            if (actualType == typeof(string))
                return Json.New.String((string)clrObject);

            if (actualType == typeof(Guid))
                return Json.New.String((Guid)clrObject);

            if (actualType == typeof(DateTime))
                return Json.New.String((DateTime)clrObject);

            if (actualType == typeof(DateTimeOffset))
                return Json.New.String((DateTimeOffset)clrObject);

            if (actualType == typeof(Type).GetType())
                return Json.New.String(((Type)clrObject).AssemblyQualifiedName ?? string.Empty);

            if (actualType == typeof(TimeSpan))
                return Json.New.String((TimeSpan)clrObject);

            if (actualType == typeof(bool))
                return ((bool)clrObject) == true ? Json.New.True : Json.New.False;

            if (actualType == typeof(double))
                return Json.New.Number((double)clrObject);

            if (actualType == typeof(float))
                return Json.New.Number((float)clrObject);

            if (actualType == typeof(long))
                return Json.New.Number((long)clrObject);

            if (actualType == typeof(int))
                return Json.New.Number((int)clrObject);

            return Json.New.Object(declaredType, clrObject);
        }
    }

    /// <summary>
    /// Provides access to Value by Name for NameValue arrays.
    /// </summary>
    /// <param name="nameValuePairs">The array of NameValue's</param>
    /// <param name="name">The Name to use for the lookup.</param>
    /// <param name="ignoreCase">True if case insensitive comparison should be used.</param>
    /// <returns>The Value that corresponds to Name.</returns>
    public static Value GetValueByName(this NameValue[] nameValuePairs, string name, bool ignoreCase)
    {
        foreach (var nameValue in nameValuePairs)
            if (ignoreCase)
            {
                if (System.String.Compare(nameValue.Name.StaticValue, name, CultureInfo.InvariantCulture, CompareOptions.IgnoreCase) == 0)
                    return nameValue.Value;
            }
            else if (nameValue.Name.StaticValue == name)
                return nameValue.Value;

        throw new KeyNotFoundException(name);
    }

    /// <summary>
    /// Serialize an object into json.
    /// Polymorphic serialization is supported by wrapping objects in a json type object
    /// when needed.
    /// </summary>
    /// <typeparam name="T">The declared type of <code>clrObject</code></typeparam>
    /// <param name="clrObject">The object to serialize.</param>
    /// <returns>A string in json format.</returns>
    public static string? Serialize<T>(T clrObject)
    { return New.Value(typeof(T), clrObject).ToString(); }

    /// <summary>
    /// Deserialize an object from json.
    /// </summary>
    /// <typeparam name="TResult">The expected return type.</typeparam>
    /// <param name="text">A string in json format.</param>
    /// <returns>A deserialized object of type <code>TResult</code>.</returns>
    public static TResult? Deserialize<TResult>(string? text)
    { 
        if (text is null)
            throw new ArgumentNullException(nameof(text));

        return (TResult?) Json.Value.Parse(text.AsSubString()).ConvertTo(typeof(TResult?)); 
    }

    /// <summary>
    /// Provides a representation for json values.
    /// </summary>
    public abstract class Value
    {
        public static Value Parse(SubString text)
        {
            if (text == "null")
                return Json.New.Null;

            if (text == "true")
                return Json.New.True;

            if (text == "false")
                return Json.New.False;

            if (text.First == '{')
                return Object.Parse(text);

            if (text.First == '[')
                return Array.Parse(text);

            if (text.First == '"')
                return String.Parse(text);

            return Number.Parse(text.Trim());
        }

        public abstract object? ConvertTo(Type type);
    }

    /// <summary>
    /// Provides a representationm for json true.
    /// </summary>
    public sealed class True : Value
    {
        public static bool StaticValue { get { return true; } }

        internal True()
        { }

        public override string ToString()
        { return "true"; }

        public override object? ConvertTo(Type type)
        {
            if (type == typeof(bool))
                return StaticValue;

            throw new InvalidCastException("Json.True >> {0}".Args(type.Name));
        }
    }

    /// <summary>
    /// Provides a representation for json false.
    /// </summary>
    public sealed class False : Value
    {
        public static bool StaticValue { get { return false; } }

        internal False()
        { }

        public override string ToString()
        { return "false"; }

        public override object? ConvertTo(Type type)
        {
            if (type == typeof(bool))
                return StaticValue;

            throw new InvalidCastException("Json.False >> {0}".Args(type.Name));
        }
    }

    /// <summary>
    /// Provides a representation for json nulls.
    /// </summary>
    public sealed class Null : Value
    {
        public static object? StaticValue { get { return null; } }

        internal Null()
        { }

        public override string ToString()
        { return "null"; }

        public override object? ConvertTo(Type type)
        {
            if (type.IsArray && type.HasElementType)
                return System.Array.CreateInstance(type.GetElementType() ?? typeof(Object), 0);

            var constructorInfo = type
                .GetConstructor(Seq.Array<Type>());

            if (constructorInfo != null)
                return constructorInfo.Invoke(null);

            return StaticValue;
        }
    }

    /// <summary>
    /// Provides a representation for json strings.
    /// </summary>
    public sealed class String : Value
    {
        private readonly SubString _value;
        public string StaticValue
        { get { return _value.Trim(1, -1).ToString().DecodeEscape(); } }

        private String(SubString value)
        { _value = value; }

        internal String(string str)
        { _value = str.AsCodeString().AsSubString(); }

        public override string ToString()
        { return _value.ToString(); }

        public static new String Parse(SubString text)
        {
            if (text.First != '"' || text.Last != '"')
                throw new FormatException(text.ToString());

            return new Json.String(text);
        }

        public override object? ConvertTo(Type type)
        {
            if (type == typeof(string))
                return StaticValue;

            if (type == typeof(Guid))
                return new Guid(StaticValue);

            if (type == typeof(DateTime))
            {
                var dateTime = DateTime.ParseExact(StaticValue, "o", CultureInfo.InvariantCulture);

                // ParseExact converts Utc to corresponding Local, convert it back
                if (StaticValue.EndsWith('Z') && dateTime.Kind == DateTimeKind.Local)
                    dateTime = dateTime.ToUniversalTime(); // Convert to DateTimeKind.Utc

                return dateTime;
            }

            if (type == typeof(DateTimeOffset))
                return DateTimeOffset.ParseExact(StaticValue, "yyyy-MM-ddTHH:mm:ss.fffffffzzz", CultureInfo.InvariantCulture);

            if (type == typeof(TimeSpan))
                return TimeSpan.Parse(StaticValue);

            if (type == typeof(Type))
                return Type.GetType(StaticValue);

            if (type.IsEnum)
                return Enum.Parse(type, StaticValue, false);

            throw new InvalidCastException("Json.String >> {0}".Args(type.Name));
        }
    }

    /// <summary>
    /// Provides a representation for json numbers.
    /// </summary>
    public sealed class Number : Value
    {
        private readonly SubString _value;
        public string StaticValue
        { get { return _value.ToString(); } }

        internal Number(string value)
        { _value = value.AsSubString(); }

        private Number(SubString subString)
        { _value = subString; }

        public override string ToString()
        { return StaticValue; }

        public static new Number Parse(SubString text)
        {
            return new Json.Number(text);
        }

        public override object? ConvertTo(Type type)
        {
            if (type == typeof(double))
                return Convert.ToDouble(StaticValue, CultureInfo.InvariantCulture);

            if (type == typeof(float))
                return Convert.ToSingle(StaticValue, CultureInfo.InvariantCulture);

            if (type == typeof(long))
                return Convert.ToInt64(StaticValue, CultureInfo.InvariantCulture);

            if (type == typeof(int))
                return Convert.ToInt32(StaticValue, CultureInfo.InvariantCulture);

            throw new InvalidCastException("Json.Number >> {0}".Args(type.Name));
        }
    }

    /// <summary>
    /// Provides a representation for json name/value pairs.
    /// </summary>
    public sealed class NameValue
    {
        public readonly String Name;
        public readonly Value Value;

        internal NameValue(String name, Value value)
        {
            Name = name;
            Value = value;
        }

        public override string ToString()
        {
            var builder = new StringBuilder();

            builder
                .Append(Name.ToString())
                .Append(':')
                .Append(Value.ToString());

            return builder.ToString();
        }

        public static NameValue Parse(SubString text)
        {
            var parts = text.Split(':').Select(part => part.Trim()).ToArray();
            return Json.New.NameValue(String.Parse(parts[0]), Value.Parse(parts[1]));
        }
    }

    /// <summary>
    /// Provides a representation for json objects.
    /// </summary>
    public sealed class Object : Value
    {
        public readonly NameValue[] NameValuePairs;

        internal Object(NameValue[] nameValuePairs)
        { NameValuePairs = nameValuePairs; }

        public override string ToString()
        {
            var builder = new StringBuilder();

            builder.Append('{')
                .Append(NameValuePairs.Select(nameValue => nameValue.ToString()).Join(','))
                .Append('}');

            return builder.ToString();
        }

        public static new Object Parse(SubString text)
        {
            if (text.First != '{' || text.Last != '}')
                throw new FormatException(text.ToString());

            var nameValuePairs = text
                .Trim(1, -1)
                .Split(',')
                .Select(part => NameValue.Parse(part));

            return Json.New.Object(nameValuePairs);
        }

        private static bool IsAssemblyQualifiedTypeName(string text)
        {
            if (!text.Contains("PublicKeyToken="))
                return false;

            if (!text.Contains("Version="))
                return false;

            if (!text.Contains("Culture="))
                return false;

            return true;
        }

        public override object ConvertTo(Type baseType)
        {
            Type? type = null;
            NameValue[]? properties = null;

            if (NameValuePairs.Length == 1 && NameValuePairs[0].Value is Json.Object @object)
            {
                var assemblyQualifiedTypeName = NameValuePairs[0].Name.StaticValue;

                if (IsAssemblyQualifiedTypeName(assemblyQualifiedTypeName))
                {
                    type = Type.GetType(assemblyQualifiedTypeName);
                    properties = @object.NameValuePairs;                    
                }
            }

            type ??= baseType;
            properties ??= NameValuePairs;

            var constructors = type
                .GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

            object?[] arguments = new object[properties.Length];

            foreach (var constructor in constructors)
            {
                var parameters = constructor.GetParameters();
                if (parameters.Length != properties.Length)
                    continue;

                try
                {
                    foreach (var parameter in parameters)
                    {
                        if (parameter.Name == null)
                            continue;

                        Value namedValue = properties.GetValueByName(parameter.Name, true);
                        arguments[parameter.Position] = namedValue.ConvertTo(parameter.ParameterType);
                    }
                }
                catch (KeyNotFoundException ex)
                {
                    throw new InvalidOperationException("Json: ctor {0}({1}) could not be found. '{2}' is incorrect.".Args(type.Name, properties.Select(p => p.Name.StaticValue).Join(','), ex.Message));
                }

                return constructor.Invoke(arguments);
            }

            if (constructors.Length == 1 && constructors[0].GetParameters().Any() == false)
            {
                // Let's try using the default constructor and properties.
                var defaultConstructor = type.GetConstructor(Seq.Array<Type>());

                if (defaultConstructor != null)
                {
                    var instance = defaultConstructor.Invoke(null);

                    foreach (var property in properties)
                    {
                        var propertyReference = type.GetProperty(property.Name.StaticValue);

                        if (propertyReference is null)
                            throw new InvalidOperationException($"Json: {type.Name}.{property.Name.StaticValue} property not found.");

                        propertyReference.SetValue(instance, property.Value.ConvertTo(propertyReference.PropertyType));
                    }

                    return instance;
                }
            }

            throw new InvalidOperationException("Json: ctor {0}({1}) could not be found.".Args(type.Name, properties.Select(p => p.Name.StaticValue).Join(',')));
        }
    }

    /// <summary>
    /// Provides a representation for json arrays.
    /// </summary>
    public sealed class Array : Value
    {
        public readonly Value[] Values;

        internal Array(Value[] values)
        { Values = values; }

        public override string ToString()
        {
            var builder = new StringBuilder();

            builder.Append('[')
                .Append(Values.Select(value => value.ToString()).Join(','))
                .Append(']');

            return builder.ToString();
        }

        public static new Value Parse(SubString text)
        {
            if (text.First != '[' || text.Last != ']')
                throw new FormatException(text.ToString());

            if (text == "[]")
                return Json.New.Null;

            return Json.New.Array(text.Trim(1, -1).Split(',').Select(part => Value.Parse(part)));
        }

        public override object? ConvertTo(Type type)
        {
            if (!(type.IsArray && type.HasElementType))
                throw new InvalidCastException($"A Json.Array can not be cast to a {type.Name}.");

            var rank = type.GetArrayRank();

            var elementType = type.GetElementType();

            if (elementType is null)
                throw new InvalidProgramException($"A Json.Array can not be cast to a {type.Name} array, {nameof(elementType)} is null.");

            if (rank == 1)
            {
                var array = System.Array.CreateInstance(elementType, Values.Length);
                for (int i = 0; i < array.Length; i++) array.SetValue(Values[i].ConvertTo(elementType), i);
                return array;
            }
            else
            {
                var lengths = (Values[0].ConvertTo(typeof(int[])) as int[]) ?? System.Array.Empty<int>();
                var elements = (Values[1].ConvertTo(elementType.MakeArrayType()) as System.Array) ?? System.Array.CreateInstance(elementType, 0);
                var array = System.Array.CreateInstance(elementType, lengths);
                var indices = new int[rank];

                foreach (var value in elements)
                {
                    array.SetValue(value, indices);

                    for (int i = rank - 1; i >= 0; i--)
                    {
                        indices[i] = indices[i] + 1;
                        if (indices[i] < lengths[i]) break;
                        indices[i] = 0;
                    }
                }

                return array;
            }
        }
    }
}
