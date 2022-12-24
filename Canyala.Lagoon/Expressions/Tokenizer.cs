//
// Copyright (c) 2013 Canyala Innovation AB
//
// All rights reserved.
//

using System;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace Canyala.Lagoon.Expressions
{
    internal class Tokenizer
    {
        private Regex regEx = null;
        private MatchCollection matches;
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

        private string RegExEscape(string s)
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
