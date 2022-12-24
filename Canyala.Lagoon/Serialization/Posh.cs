//
// Copyright (c) 2013 Canyala Innovation AB
//
// All rights reserved.
//

using Canyala.Lagoon.Extensions;
using Canyala.Lagoon.Functional;
using Canyala.Lagoon.Reflection;
using Canyala.Lagoon.Text;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Canyala.Lagoon.Serialization
{
    /// <summary>
    /// A POSH Serializer/Deserializer (Plain Old Semantic Html)
    /// </summary>
    public static class Posh
    {
        /// <summary>
        /// Represents any html element.
        /// </summary>
        public abstract class Element
        {
        }

        /// <summary>
        /// Represesents a list of html elements.
        /// </summary>
        public class ElementList : Element, IEnumerable<Element>
        {
            private Element[] _elements;

            internal ElementList(params Element[] elements)
            { _elements = elements; }

            internal ElementList(IEnumerable<Element> elements)
            { _elements = elements.ToArray(); }

            public override string ToString()
            { return String.Concat(_elements.Select(element => element.ToString())); }

            public IEnumerator<Element> GetEnumerator()
            { foreach (var element in _elements) yield return element; }

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            { return _elements.GetEnumerator(); }
        }

        /// <summary>
        /// Represents html text which is also considered to be an element.
        /// </summary>
        public class Text : Element
        {
            private string _content;

            internal Text(string content)
            { _content = content; }

            public override string ToString()
            { return _content; }
        }

        /// <summary>
        /// Represents semantic html elements
        /// </summary>
        public abstract class SemanticElement : Element
        {
            private string _tag;
            private Element _content;

            internal SemanticElement(string tag, Element content)
            {
                _tag = tag;
                _content = content;
            }

            public override string ToString()
            { return String.Concat("<", _tag, ">", _content, "</", _tag, ">"); }
        }

        /// <summary>
        /// Represents the h1 element
        /// </summary>
        public class Heading1 : SemanticElement
        {
            internal Heading1(Element content) : base("h1", content) { }
        }

        public class Heading2 : SemanticElement
        {
            internal Heading2(Element content) : base("h2", content) { }
        }

        public class Heading3 : SemanticElement
        {
            internal Heading3(Element content) : base("h3", content) { }
        }

        public class Heading4 : SemanticElement
        {
            internal Heading4(Element content) : base("h4", content) { }
        }

        public class Heading5 : SemanticElement
        {
            internal Heading5(Element content) : base("h5", content) { }
        }

        public class Heading6 : SemanticElement
        {
            internal Heading6(Element content) : base("h6", content) { }
        }

        public class Paragraph : SemanticElement
        {
            internal Paragraph(Element content) : base("p", content) { }
        }

        public class Anchor : SemanticElement
        {
            internal Anchor(Element content) : base("a", content) { }
        }

        public class Section : SemanticElement
        {
            internal Section(Element content) : base("section", content) { }
        }

        public class Article : SemanticElement
        {
            internal Article(Element content) : base("article", content) { }
        }

        public class Meter : SemanticElement
        {
            internal Meter(Element content) : base("meter", content) { }
        }

        public class ListItem : SemanticElement
        {
            internal ListItem(Element content) : base("li", content) { }
        }

        public class OrderedList : SemanticElement
        {
            internal OrderedList(ElementList content) : base("ol", content) { }
        }

        public class UnorderedList : SemanticElement
        {
            internal UnorderedList(ElementList content) : base("ul", content) { }
        }

        public class DefinitionType : SemanticElement
        {
            internal DefinitionType(Element content) : base("dt", content) { }
        }

        public class DefinitionValue : SemanticElement
        {
            internal DefinitionValue(Element content) : base("dv", content) { }
        }

        public class DefinitionPair : Element
        {
            private DefinitionType _type;
            private DefinitionValue _value;

            internal DefinitionPair(DefinitionType type, DefinitionValue value)
            {
                _type = type;
                _value = value;
            }

            public override string ToString()
            {
                return String.Concat(_type, _value);
            }
        }

        public class DefinitionList : SemanticElement
        {
            internal DefinitionList(params DefinitionPair[] pairs) : base("dl", New.ElementList(pairs)) { }
            internal DefinitionList(IEnumerable<DefinitionPair> pairs) : base("dl", New.ElementList(pairs.ToArray())) { }
        }

        /// <summary>
        /// POSH element factory methods.
        /// </summary>
        public static class New
        {
            static public Element Text(string content)
            { return new Text(content); }

            static public Element Heading1(string content)
            { return new Heading1(New.Text(content)); }

            static public Element Heading2(string content)
            { return new Heading2(New.Text(content)); }

            static public Element Heading3(string content)
            { return new Heading3(New.Text(content)); }

            static public Element Heading4(string content)
            { return new Heading4(New.Text(content)); }

            static public Element Heading5(string content)
            { return new Heading5(New.Text(content)); }

            static public Element Heading6(string content)
            { return new Heading6(New.Text(content)); }

            static public Element Paragraph(string content)
            { return new Paragraph(New.Text(content)); }

            static public Element Article(string content)
            { return new Article(New.Text(content)); }

            static public Element Section(string content)
            { return new Paragraph(New.Text(content)); }

            static public Element Meter(string content)
            { return new Meter(New.Text(content)); }

            static public Element ListItem(string content)
            { return new ListItem(New.Text(content)); }

            static public DefinitionType DefinitionType(string content)
            { return new DefinitionType(New.Text(content)); }

            static public DefinitionValue DefinitionValue(string content)
            { return new DefinitionValue(New.Text(content)); }

            static public DefinitionPair DefinitionPair(DefinitionType type, DefinitionValue value)
            { return new DefinitionPair(type, value); }

            static public DefinitionPair DefinitionPair(string type, string value)
            { return New.DefinitionPair(New.DefinitionType(type), New.DefinitionValue(value)); }

            static public ElementList ElementList(params Element[] elements)
            { return new ElementList(elements); }

            static public ElementList ElementList(IEnumerable<Element> elements)
            { return new ElementList(elements.ToArray()); }

            static public ElementList ElementList(IEnumerable<string> strings)
            { return new ElementList(strings.Select(s => New.Text(s))); }

            static public ElementList ElementList(params string[] strings)
            { return new ElementList(strings.Select(s => New.Text(s))); }

            static public Element OrderedList(params string[] strings)
            { return new OrderedList(New.ElementList(strings.Select(s => New.ListItem(s)))); }

            static public Element OrderedList(IEnumerable<string> strings)
            { return new OrderedList(New.ElementList(strings.Select(s => New.ListItem(s)))); }

            static public Element UnorderedList(params string[] strings)
            { return new UnorderedList(New.ElementList(strings.Select(s => New.ListItem(s)))); }

            static public Element UnorderedList(IEnumerable<string> strings)
            { return new UnorderedList(New.ElementList(strings.Select(s => New.ListItem(s)))); }

            static public Element DefinitionList(params Tuple<string, string>[] pairs)
            { return new DefinitionList(pairs.Select(pair => New.DefinitionPair(pair.Item1, pair.Item2))); }

            static public Element DefinitionList(IEnumerable<Tuple<string, string>> pairs)
            { return new DefinitionList(pairs.Select(pair => New.DefinitionPair(pair.Item1, pair.Item2))); }

            static public Element DefinitionList(IDictionary<string, string> pairs)
            { return new DefinitionList(pairs.Select(pair => New.DefinitionPair(pair.Key, pair.Value))); }

            static public Element DefinitionList(params KeyValuePair<string, string>[] pairs)
            { return new DefinitionList(pairs.Select(pair => New.DefinitionPair(pair.Key, pair.Value))); }

            static public Element DefinitionList(IEnumerable<KeyValuePair<string, string>> pairs)
            { return new DefinitionList(pairs.Select(pair => New.DefinitionPair(pair.Key, pair.Value))); }
        }
    }
}

/*
 
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
            { return new String(dateTimeOffset.ToString("yyyy-MM-ddThh:mm:ss.fffffffzzz", CultureInfo.InvariantCulture)); }

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
            static public Value Object(Type declaredType, object clrObject)
            {
                var actualType = clrObject.GetType();

                if (!declaredType.IsAssignableFrom(actualType))
                    throw new InvalidCastException();

                IEnumerable<Json.NameValue> nameValuePairs = null;

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
                var jsonTypeName = Json.New.String(actualType.AssemblyQualifiedName);
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
            static public Value Value(Type declaredType, object clrObject)
            {
                if (clrObject == null)
                    return Json.New.Null;

                var actualType = clrObject.GetType();

                if (actualType.IsEnum)
                    return Json.New.String(clrObject.ToString());

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
                    return Json.New.String(((Type)clrObject).AssemblyQualifiedName);

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
        public static string Serialize<T>(T clrObject)
        { return Json.New.Value(typeof(T), clrObject).ToString(); }

        /// <summary>
        /// Deserialize an object from json.
        /// </summary>
        /// <typeparam name="TResult">The expected return type.</typeparam>
        /// <param name="text">A string in json format.</param>
        /// <returns>A deserialized object of type <code>TResult</code>.</returns>
        public static TResult Deserialize<TResult>(string text)
        { return (TResult)Json.Value.Parse(text.AsSubString()).ConvertTo(typeof(TResult)); }

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

            public abstract object ConvertTo(Type type);
        }

        /// <summary>
        /// Provides a representationm for json true.
        /// </summary>
        public sealed class True : Value
        {
            public bool StaticValue { get { return true; } }

            internal True()
            { }

            public override string ToString()
            { return "true"; }

            public override object ConvertTo(Type type)
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
            public bool StaticValue { get { return false; } }

            internal False()
            { }

            public override string ToString()
            { return "false"; }

            public override object ConvertTo(Type type)
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
            public object StaticValue { get { return null; } }

            internal Null()
            { }

            public override string ToString()
            { return "null"; }

            public override object ConvertTo(Type type)
            {
                if (type.IsArray && type.HasElementType)
                    return System.Array.CreateInstance(type.GetElementType(), 0);

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

            public override object ConvertTo(Type type)
            {
                if (type == typeof(string))
                    return StaticValue;

                if (type == typeof(Guid))
                    return new Guid(StaticValue);

                if (type == typeof(DateTime))
                    return DateTime.ParseExact(StaticValue, "o", CultureInfo.InvariantCulture);

                if (type == typeof(DateTimeOffset))
                    return DateTimeOffset.ParseExact(StaticValue, "yyyy-MM-ddThh:mm:ss.fffffffzzz", CultureInfo.InvariantCulture);

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
            private SubString _value;
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

            public override object ConvertTo(Type type)
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
                Type type = null;
                NameValue[] properties = null;
                if (NameValuePairs.Length == 1 && NameValuePairs[0].Value is Object)
                {
                    var assemblyQualifiedTypeName = NameValuePairs[0].Name.StaticValue;

                    if (IsAssemblyQualifiedTypeName(assemblyQualifiedTypeName))
                    {
                        properties = ((Object)NameValuePairs[0].Value).NameValuePairs;
                        type = Type.GetType(assemblyQualifiedTypeName);
                    }
                }

                if (type == null)
                {
                    properties = NameValuePairs;
                    type = baseType;
                }

                var constructors = type
                    .GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

                var arguments = new object[properties.Length];

                foreach (var constructor in constructors)
                {
                    var parameters = constructor.GetParameters();
                    if (parameters.Length != properties.Length)
                        continue;

                    try
                    {
                        foreach (var parameter in parameters)
                        {
                            var namedValue = properties.GetValueByName(parameter.Name, true);
                            if (namedValue == null) continue;

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
                    var instance = defaultConstructor.Invoke(null);

                    foreach (var property in properties)
                    {
                        var propertyReference = type.GetProperty(property.Name.StaticValue);
                        propertyReference.SetValue(instance, property.Value.ConvertTo(propertyReference.PropertyType));
                    }

                    return instance;
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

            public override object ConvertTo(Type type)
            {
                if (!(type.IsArray && type.HasElementType))
                    throw new InvalidCastException("A Json.Array can not be cast to a {0}.".Args(type.Name));

                var elementType = type.GetElementType();
                var rank = type.GetArrayRank();

                if (rank == 1)
                {
                    var array = System.Array.CreateInstance(elementType, Values.Length);
                    for (int i = 0; i < array.Length; i++) array.SetValue(Values[i].ConvertTo(elementType), i);
                    return array;
                }
                else
                {
                    var lengths = (int[])Values[0].ConvertTo(typeof(int[]));
                    var elements = (System.Array)Values[1].ConvertTo(elementType.MakeArrayType());
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
}
 
*/