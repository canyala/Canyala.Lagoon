/*
 
  MIT License

  Copyright (c) 2022 Canyala Innovation

  Permission is hereby granted, free of charge, to any person obtaining a copy
  of this software and associated documentation files (the "Software"), to deal
  in the Software without restriction, including without limitation the rights
  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
  copies of the Software, and to permit persons to whom the Software is
  furnished to do so, subject to the following conditions:

  The above copyright notice and this permission notice shall be included in all
  copies or substantial portions of the Software.

  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
  SOFTWARE.

*/

using Canyala.Lagoon.Functional;
using Canyala.Lagoon.Text;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace Canyala.Lagoon.Extensions
{
    public static class StringExtensions
    {
        static public string AsString(this IEnumerable<char> seq)
        {
            var builder = new StringBuilder();
            foreach (var c in seq) builder.Append(c);
            return builder.ToString();
        }

        static public IEnumerable<char> AsEnumerable(this string s)
        {
            return s.ToCharArray();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="self"></param>
        /// <param name="other"></param>
        /// <returns></returns>
        static public string CommonStartsWith(this string self, string other)
        {
            var builder = new StringBuilder();
            var otherEnum = other.AsEnumerable().GetEnumerator();
            var selfEnum = self.AsEnumerable().GetEnumerator();

            while (selfEnum.MoveNext() && otherEnum.MoveNext())
            {
                if (selfEnum.Current == otherEnum.Current)
                {
                    builder.Append(selfEnum.Current);
                    continue;
                }

                break;
            }

            return builder.ToString();
        }

        static public bool TryGetEnumPrefix(this string self, out string prefix)
        {
            prefix = string.Empty;
            var enumerator = self.AsEnumerable().GetEnumerator();
            var builder = new StringBuilder();

            while (enumerator.MoveNext())
                if (Char.IsLetter(enumerator.Current)) builder.Append(enumerator.Current);
                else if (Char.IsDigit(enumerator.Current))
                {
                    while (enumerator.MoveNext())
                        if (!Char.IsDigit(enumerator.Current)) return false;
                    prefix = builder.ToString();
                    return true;
                }
                else return false;

            return false;
        }

        public static SubString AsSubString(this string text)
        { return new SubString(text); }

        public static string Limit(this string text, int length, string ellipsis = "...")
        {
            if (text.Length <= length)
                return text;

            return String.Concat(text.Substring(0, length - ellipsis.Length), ellipsis);
        }

        public static string AsCodeString(this string text)
        {
            var builder = new StringBuilder();

            builder
                .Append('"')
                .Append(text.EncodeEscape())
                .Append('"');

            return builder.ToString();
        }

        public static string EncodeEscape(this string text)
        {
            var builder = new StringBuilder();

            foreach (var c in text)
                switch (c)
                {
                    case '\"':
                        builder.Append("\\\"");
                        break;
                    case '\\':
                        builder.Append("\\\\");
                        break;
                    case '\b':
                        builder.Append("\\b");
                        break;
                    case '\f':
                        builder.Append("\\f");
                        break;
                    case '\n':
                        builder.Append("\\n");
                        break;
                    case '\r':
                        builder.Append("\\r");
                        break;
                    case '\t':
                        builder.Append("\\t");
                        break;
                    default:
                        builder.Append(c);
                        break;
                }

            return builder.ToString();
        }

        public static string DecodeEscape(this string text)
        {
            var builder = new StringBuilder();
            var checkEscape = false;

            foreach (var c in text)
            {
                if (c == '\\')
                {
                    if (checkEscape)
                    {
                        builder.Append('\\');
                        checkEscape = false;
                        continue;
                    }

                    checkEscape = true;
                    continue;
                }

                if (checkEscape)
                {
                    checkEscape = false;

                    switch (c)
                    {
                        case '"':
                            builder.Append('"');
                            break;
                        case 'b':
                            builder.Append('\b');
                            break;
                        case 'f':
                            builder.Append('\f');
                            break;
                        case 'n':
                            builder.Append('\n');
                            break;
                        case 'r':
                            builder.Append('\r');
                            break;
                        case 't':
                            builder.Append('\t');
                            break;
                        default:
                            throw new FormatException("Unrecognized escape character \\{0}".Args(c));
                    }
                }
                else
                    builder.Append(c);
            }

            return builder.ToString();
        }

        public static string EncodeKey(this string key)
        {
            var encoded = new StringBuilder();

            foreach (var c in key)
            {
                switch (c)
                {
                    case '/':
                        encoded.Append("=1");
                        break;
                    case '\\':
                        encoded.Append("=2");
                        break;
                    case '#':
                        encoded.Append("=3");
                        break;
                    case '?':
                        encoded.Append("=4");
                        break;
                    case '=':
                        encoded.Append("==");
                        break;
                    default:
                        encoded.Append(c);
                        break;
                }
            }

            return encoded.ToString();
        }

        public static string DecodeKey(this string key)
        {
            var builder = new StringBuilder();
            var checkEscape = false;

            foreach (var c in key)
            {
                if (c == '=')
                {
                    if (checkEscape)
                    {
                        builder.Append('=');
                        checkEscape = false;
                        continue;
                    }

                    checkEscape = true;
                    continue;
                }

                if (checkEscape)
                {
                    checkEscape = false;

                    switch (c)
                    {
                        case '1':
                            builder.Append('/');
                            break;
                        case '2':
                            builder.Append('\\');
                            break;
                        case '3':
                            builder.Append('#');
                            break;
                        case '4':
                            builder.Append('?');
                            break;
                        default:
                            throw new FormatException("Unrecognized escape character \\{0}".Args(c));
                    }
                }
                else
                    builder.Append(c);
            }

            return builder.ToString();
        }

        public static string Args(this string format, params object[] arguments)
        {
            return String.Format(format, arguments);
        }

        public static bool HasValue(this string text)
        {
            return !string.IsNullOrEmpty(text);
        }

        /// <summary>
        /// Extension for strings that is a shortcut to create formatted strings with a 
        /// format provider.
        /// </summary>
        /// <param name="format">The string format expression.</param>
        /// <param name="formatProvider">A format provider to use when formatting text.</param>
        /// <param name="arguments">The arguments to format.</param>
        /// <returns>A formatted string with the arguments.</returns>
        /// <remarks>
        /// This extension is a short form for the <see cref="String.Format"/>
        /// method. It allows you to write, for example:
        /// <code>
        /// "{0}, {1}, {2}".Args(CultureInfo.CurrentCulture, 1, 2, 3)   // => "1, 2, 3"
        /// </code>
        /// Instead of:
        /// <code>
        /// String.Format(CultureInfo.CurrentCulture, "{0}, {1}, {2}", 1, 2, 3);
        /// </code>
        /// </remarks>
        public static string Args(this string format, IFormatProvider formatProvider, params object[] arguments)
        {
            return string.Format(formatProvider, format, arguments);
        }

        public static string[] Split(this string s, string split)
        {
            return s.Split(Seq.Array(split), StringSplitOptions.None);
        }

        public static string Join(this IEnumerable<string> seqOfStrings, char separator)
        {
            var builder = new StringBuilder();

            foreach (var stringItem in seqOfStrings)
            {
                if (builder.Length > 0) builder.Append(separator);
                builder.Append(stringItem);
            }

            return builder.ToString();
        }

        public static string Join(this IEnumerable<string> seqOfStrings, string separator)
        {
            var builder = new StringBuilder();

            foreach (var stringItem in seqOfStrings)
            {
                if (builder.Length > 0) builder.Append(separator);
                builder.Append(stringItem);
            }

            return builder.ToString();
        }

        public static double AsDouble(this string text)
        {
            double value = 0.0;
            double.TryParse(text, out value);
            return value;
        }

        public static bool IsDouble(this string text)
        {
            double value = 0.0;
            return double.TryParse(text, out value);
        }

        public static long AsLong(this string text)
        {
            long value = 0L;
            long.TryParse(text, out value);
            return value;
        }

        public static bool IsLong(this string text)
        {
            long value = 0L;
            return long.TryParse(text, out value);
        }

        public static int AsInt(this string text)
        {
            int value = 0;
            int.TryParse(text, out value);
            return value;
        }

        public static bool IsInt(this string text)
        {
            int value = 0;
            return int.TryParse(text, out value);
        }

        public static bool Any(this string text, Predicate<char> predicate)
        {
            foreach (var c in text)
                if (predicate(c))
                    return true;

            return false;
        }

        public static T AsEnum<T>(this string text)
        {
            return (T)Enum.Parse(typeof(T), text, true);
        }

        public static bool IsEmpty(this string text)
        { return string.IsNullOrEmpty(text); }

        public static string ReplacePrefix(this string text, string prefix, string replaceWith)
        {
            if (!text.IsEmpty() && text.StartsWith(prefix))
                return replaceWith + text.Substring(prefix.Length);

            return text;
        }

        public static string ReplaceSuffix(this string text, string suffix, string replaceWith)
        {
            if (!text.IsEmpty() && text.EndsWith(suffix))
                return text.Substring(0, text.Length - suffix.Length) + replaceWith;

            return text;
        }

        public static bool StartsWithAny(this string text, params string[] prefixes)
        {
            if (!text.IsEmpty())
                return prefixes.Any(prefix => text.StartsWith(prefix));

            return false;
        }

        public static bool EndsWithAny(this string text, params string[] suffixes)
        {
            if (!text.IsEmpty())
                return suffixes.Any(suffix => text.EndsWith(suffix));

            return false;
        }

        public static string TrimAny(this string text, params string[] subStrings)
        {
            string ignore;
            return TrimAny(text, out ignore, subStrings);
        }

        public static string TrimAny(this string text, out string match, params string[] subStrings)
        {
            match = null;
            foreach (var s in subStrings)
            {
                if (text.StartsWith(s) && text.EndsWith(s))
                {
                    match = s;
                    int start = s.Length;
                    int length = text.Length - (start * 2);
                    return text.Substring(start, length);
                }
            }

            return text;
        }

        public static string TrimStartAny(this string text, params string[] subStrings)
        {
            string ignore;
            return TrimStartAny(text, out ignore, subStrings);
        }

        public static string TrimStartAny(this string text, out string match, params string[] subStrings)
        {
            match = null;
            foreach (var s in subStrings)
            {
                if (text.StartsWith(s))
                {
                    match = s;
                    int start = s.Length;
                    return text.Substring(start);
                }
            }

            return text;
        }

        public static string TrimEndAny(this string text, params string[] subStrings)
        {
            string ignore;
            return TrimEndAny(text, out ignore, subStrings);
        }

        public static string TrimEndAny(this string text, out string match, params string[] subStrings)
        {
            match = null;
            foreach (var s in subStrings)
            {
                if (text.EndsWith(s))
                {
                    match = s;
                    int length = text.Length - s.Length;
                    return text.Substring(0, length);
                }
            }

            return text;
        }

        public static bool IsQuotedByAny(this string text, params string[] quotes)
        {
            foreach (var s in quotes)
            {
                if (text.StartsWith(s) && text.EndsWith(s))
                    return true;
            }

            return false;
        }

        public static IEnumerable<char> AsChars(this IEnumerable<string> lines)
        {
            foreach (var line in lines)
                foreach (var c in line.AsEnumerable())
                    yield return c;
        }

        public static IEnumerable<string> AsLines(this IEnumerable<char> text)
        {
            var builder = new StringBuilder();
            char[] eolChars = new[] { '\r', '\n' };
            char prev = '\0';

            foreach (char c in text)
            {
                if (prev == '\n' || prev == '\r')
                {
                    yield return builder.ToString();
                    builder.Clear();
                    if (prev == '\r' && c == '\n')
                    {
                        prev = '\0';
                        continue;
                    }
                }

                prev = c;

                if (c != '\r' && c != '\n')
                    builder.Append(c);
            }

            yield return builder.ToString();
        }

        #region URI reserved characters
        private static readonly Dictionary<char, string> _reservedCharacters = new Dictionary<char, string>
        {
            { ' ', "+" },
            { '!', "%21" },
            { '#', "%23" },
            { '$', "%24" },
            { '&', "%26" },
            { '\'', "%27" },
            { '(', "%28" },
            { ')', "%29" },
            { '*', "%2A" },
            { '+', "%2B" },
            { ',', "%2C" },
            { '/', "%2F" },
            { ':', "%3A" },
            { ';', "%3B" },
            { '=', "%3D" },
            { '?', "%3F" },
            { '@', "%40" },
            { '[', "%5B" },
            { ']', "%5D" }
        };
        #endregion

        public static string AsPercentEncoded(this string text)
        {
            var encoded = new StringBuilder();

            foreach (char c in text)
            {
                byte[] bytes = Encoding.UTF8.GetBytes(Seq.Array(c));

                if (bytes.Length > 1)
                {
                    foreach (byte b in bytes)
                        encoded.AppendFormat("%{0}", b.ToString("X2"));
                    continue;
                }

                if (_reservedCharacters.ContainsKey(c))
                    encoded.Append(_reservedCharacters[c]);
                else
                    encoded.Append(c);
            }

            return encoded.ToString();
        }

        public static string AsPercentDecoded(this string text)
        {
            StringBuilder result = new StringBuilder();
            List<byte> decodeBuffer = null;

            for (int i = 0; i < text.Length; i++)
            {
                char c = text[i];

                if (c == '%')
                {
                    if (decodeBuffer == null)
                        decodeBuffer = new List<byte>();
                    string hex = text.Substring(i + 1, 2);
                    byte b = byte.Parse(hex, NumberStyles.AllowHexSpecifier);
                    decodeBuffer.Add(b);
                    i += 2;
                    continue;
                }

                if (decodeBuffer != null)
                {
                    byte[] bytes = decodeBuffer.ToArray();
                    string decoded = Encoding.UTF8.GetString(bytes, 0, bytes.Length);
                    result.Append(decoded);
                    decodeBuffer = null;
                }

                if (c == '+')
                    c = ' ';

                result.Append(c);
            }

            if (decodeBuffer != null)
            {
                byte[] bytes = decodeBuffer.ToArray();
                string decoded = Encoding.UTF8.GetString(bytes, 0, bytes.Length);
                result.Append(decoded);
                decodeBuffer = null;
            }

            return result.ToString();
        }

        public static string AsXmlEncoded(this string text)
        {
            var builder = new StringBuilder();

            foreach (char c in text)
            {
                switch (c)
                {
                    case '"':
                        builder.Append("&quot;");
                        break;

                    case '&':
                        builder.Append("&amp;");
                        break;

                    case '\'':
                        builder.Append("&apos;");
                        break;

                    case '<':
                        builder.Append("&lt;");
                        break;

                    case '>':
                        builder.Append("&gt;");
                        break;

                    default:
                        builder.Append(c);
                        break;
                }
            }

            return builder.ToString();
        }

        public static string AsXmlDecoded(this string text)
        {
            var builder = new StringBuilder();
            string entity = string.Empty;

            foreach (char c in text)
            {
                if (entity.Length > 0)
                {
                    entity += c;

                    if (c == ';')
                    {
                        switch (entity)
                        {
                            case "&quot;":
                                builder.Append('"');
                                break;

                            case "&amp;":
                                builder.Append('&');
                                break;

                            case "&apos;":
                                builder.Append('\'');
                                break;

                            case "&lt;":
                                builder.Append('<');
                                break;

                            case "&gt;":
                                builder.Append('>');
                                break;
                        }
                        entity = string.Empty;
                    }
                    continue;
                }

                if (c == '&')
                {
                    entity = "&";
                    continue;
                }

                builder.Append(c);
            }

            return builder.ToString();
        }
    }
}
