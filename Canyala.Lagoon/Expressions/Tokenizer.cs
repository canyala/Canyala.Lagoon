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

using System.Globalization;
using System.Text.RegularExpressions;

namespace Canyala.Lagoon.Expressions
{
    internal class Tokenizer
    {
        private readonly Regex regEx;
        private readonly MatchCollection matches;
        private int currentMatch;

        public Tokenizer(string expression, NumberFormatInfo numberFormatInfo)
        {
            string pattern = string.Format(@"(\d+{0}\d+)|(\d+)|(\?\w+)|(\w+)|([+\-*/])|(\()|(\))|([^\s]+)", RegExEscape(numberFormatInfo.NumberDecimalSeparator));
            regEx = new Regex(pattern, RegexOptions.Singleline);
            matches = regEx.Matches(expression);
            Initialize();
        }

        public Tokenizer(string expression) : this(expression, CultureInfo.InvariantCulture.NumberFormat)
        {
        }

        private static string RegExEscape(string s)
        {
            if (s == ".")
                return @"\.";
            else
                return s;
        }

        public void Initialize()
        {
            currentMatch = 0;
        }

        public void Expect(Predicate<string> predicate)
        {
            if (currentMatch >= matches.Count || predicate(matches[currentMatch++].Value))
                throw new ArgumentException(string.Format("Illegal dynamic evaluate expression"));
        }

        public bool Allow(Predicate<string> predicate)
        {
            if (currentMatch < matches.Count)
                return predicate(matches[currentMatch].Value);
            else
                return false;
        }

        public bool Accept(Predicate<string> predicate)
        {
            if (currentMatch >= matches.Count) return false;

            if (predicate(matches[currentMatch].Value))
            {
                currentMatch++;
                return true;
            }

            return false;
        }

        public string Read()
        {
            if (currentMatch >= matches.Count) return string.Empty;
            return matches[currentMatch++].Value;
        }
    }
}
