//-------------------------------------------------------------------------------
//
//  MIT License
//
//  Copyright (c) 2012-2022 Canyala Innovation
//
//  Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
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

using Canyala.Lagoon.Extensions;

namespace Canyala.Lagoon.Serialization;

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
        private readonly Element[] _elements;

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
        private readonly string _content;
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
        private readonly string _tag;
        private readonly Element _element;

        internal SemanticElement(string tag, Element element)
        {
            _tag = tag;
            _element = element;
        }

        public override string ToString()
        { return String.Concat("<", _tag, ">", _element, "</", _tag, ">"); }
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

