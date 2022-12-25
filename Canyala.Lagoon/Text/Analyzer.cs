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
using System.Text;

using Canyala.Lagoon.Collections;
using Canyala.Lagoon.Extensions;
using Canyala.Lagoon.Functional;

namespace Canyala.Lagoon.Text;

/// <summary>
/// Provides text analysis algorithms
/// </summary>
public static class Analyzer
{
    static public string AsPropertyName(string text)
    {
        var builder = new StringBuilder();
        foreach (char character in text) if (Char.IsLetterOrDigit(character)) builder.Append(character);
        var propertyName = builder.ToString();

        propertyName = propertyName.Replace("Amt", "Amount");
        propertyName = propertyName.Replace("Cnt", "Count");
        propertyName = propertyName.Replace("Chrg", "Charge");
        propertyName = propertyName.Replace("Svc", "Service");
        propertyName = propertyName.Replace("Trkg", "Tracking");
        propertyName = propertyName.Replace("Acct", "Account");
        propertyName = propertyName.Replace("Dept", "Department");
        if (!propertyName.Contains("Invoice")) propertyName = propertyName.Replace("Inv", "Invoice");
        if (!propertyName.Contains("Customer")) propertyName = propertyName.Replace("Cust", "Customer");

        return propertyName;
    }

    static public FormattedType? LikelyFormattedType(IEnumerable<string> samples)
    {
        var trimmedSamples = samples
            .Select(sample => sample.Trim())
            .ToList();

        return RecognizedTypes
            .Select(type => TypeQualifierMap[type](trimmedSamples))
            .OrderByDescending(weighted => weighted.Weight)
            .First()
            .FormattedType;
    }

    static public char LikelyColumnSeparator(IEnumerable<string> lines)
    {
        return LikelyColumnSeparator(CharFromLines(lines));
    }

    static private IEnumerable<char> CharFromLines(IEnumerable<string> lines)
    {
        foreach (var line in lines)
            foreach (var @char in line.AsEnumerable())
                yield return @char;
    }

    static public char LikelyColumnSeparator(IEnumerable<char> text)
    {
        return RecognizedSeparators
            .Select(item => new { sep = item, n = text.Count(test => test == item) })
            .OrderByDescending(tuple => tuple.n)
            .First()
            .sep;
    }

    #region Internal Implementation

    private class WeightedFormattedType
    {
        public int Weight { get; set; }
        public FormattedType? FormattedType { get; set; }
    }

    static Analyzer()
    {
        Seq.Do(RecognizedTypes, TypeQualifiers, (type, qualifier) => TypeQualifierMap.Add(type, qualifier));
    }

    //
    // Important! RecognizedTypes, TypeQualifiers and TypePriorities must correlate!
    //

    delegate WeightedFormattedType TypeQualifierDelegate(IEnumerable<string> samples);

    // NOT USED : static private readonly int[] TypePriorities = { 5, 1, 4, 2, 3 };
    static private readonly Type[] RecognizedTypes = { typeof(Boolean), typeof(String), typeof(DateTime), typeof(Double), typeof(Int64) };
    static private readonly TypeQualifierDelegate[] TypeQualifiers = { IsQualifiedAsBoolean, IsQualifiedAsText, IsQualifiedAsDateTime, IsQualifiedAsReal, IsQualifiedAsInteger };
    static private readonly Dictionary<Type, TypeQualifierDelegate> TypeQualifierMap = new();
    static private readonly char[] RecognizedSeparators = { ';', ',', '\t', ':' };

    static private readonly Tuple<bool, string> TrueEmpty = new(true, String.Empty);
    static private readonly Tuple<bool, string> FalseEmpty = new(false, String.Empty);

    static private Tuple<bool, string> IsQualifiedAsText(string sample)
    {
        if (Char.IsLetter(sample[0]) || Char.IsPunctuation(sample[0]))
            return TrueEmpty;

        return FalseEmpty;
    }

    static private Tuple<bool, string> IsQualifiedAsBoolean(string sample)
    {
        if (sample == "0") return TrueEmpty;
        if (sample == "1") return TrueEmpty;
        if (sample == "-1") return TrueEmpty;
        if (sample == "true") return TrueEmpty;
        if (sample == "True") return TrueEmpty;
        if (sample == "TRUE") return TrueEmpty;
        if (sample == "false") return TrueEmpty;
        if (sample == "False") return TrueEmpty;
        if (sample == "FALSE") return TrueEmpty;

        return FalseEmpty;
    }

    static private Tuple<bool, string> IsQualifiedAsInteger(string sample)
    {
        char first = sample[0];
        if (!Char.IsDigit(first) && first != '-')
            return FalseEmpty;

        if (sample.Any(c => !Char.IsDigit(c)))
            return FalseEmpty;

        return TrueEmpty;
    }

    static private Tuple<bool, string> IsQualifiedAsReal(string sample)
    {
        if (Double.TryParse(sample, NumberStyles.Float, CultureInfo.InvariantCulture, out _))
            return TrueEmpty;
        if (Double.TryParse(sample, NumberStyles.Float, CultureInfo.CurrentUICulture, out _))
            return TrueEmpty;
        if (Double.TryParse(sample, NumberStyles.Float, CultureInfo.CurrentCulture, out _))
            return TrueEmpty;
        return FalseEmpty;
    }

    static private Tuple<bool, string> IsQualifiedAsDateTime(string sample)
    {
        string[] exactFormats = { "yyyyMMdd", "HHmmss", "d", "D", "f", "F", "g", "G", "m", "M", "o", "O", "r", "R", "s", "t", "T", "u", "U", "y", "Y" };

        foreach (string format in exactFormats)
            if (DateTime.TryParseExact(sample, format, CultureInfo.InvariantCulture, DateTimeStyles.NoCurrentDateDefault, out _))
                return new Tuple<bool, string>(true, format);

        return FalseEmpty;
    }

    static private WeightedFormattedType IsQualifiedAsInteger(IEnumerable<string> samples)
    {
        return CalculateWeightByPredicate(samples, typeof(Int32), 2, IsQualifiedAsInteger);
    }

    static private WeightedFormattedType IsQualifiedAsReal(IEnumerable<string> samples)
    {
        return CalculateWeightByPredicate(samples, typeof(double), 1, IsQualifiedAsReal);
    }

    static private WeightedFormattedType IsQualifiedAsDateTime(IEnumerable<string> samples)
    {
        return CalculateWeightByPredicate(samples, typeof(DateTime), 4, IsQualifiedAsDateTime);
    }

    static private WeightedFormattedType IsQualifiedAsBoolean(IEnumerable<string> samples)
    {
        return CalculateWeightByPredicate(samples, typeof(bool), 3, IsQualifiedAsBoolean);
    }

    static private WeightedFormattedType IsQualifiedAsText(IEnumerable<string> samples)
    {
        return CalculateWeightByPredicate(samples, typeof(string), 0, IsQualifiedAsText);
    }

    static private WeightedFormattedType CalculateWeightByPredicate(IEnumerable<string> samples, Type type, int priority, Func<string, Tuple<bool, string>> predicate)
    {
        if (samples.All(sample => String.IsNullOrEmpty(sample)))
            if (type == typeof(string))
                return new WeightedFormattedType { Weight = 1, FormattedType = new FormattedType { Type = type, Formats = new string[] { "" } } };
            else
                return new WeightedFormattedType { Weight = 0, FormattedType = new FormattedType { Type = type, Formats = new string[] { "" } } };

        var countOfValid = 0;
        var formats = new Bag<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var sample in samples)
        {
            if (String.IsNullOrEmpty(sample)) continue;
            var tuple = predicate(sample);
            if (!tuple.Item1) continue;

            if (!String.IsNullOrEmpty(tuple.Item2)) formats.Add(tuple.Item2);
            countOfValid++;
        }

        var orderedFormats = formats
            .OrderByDescending(format => format.Value)
            .Select(format => format.Key)
            .ToList();

        return new WeightedFormattedType { Weight = Weight(countOfValid, priority), FormattedType = new FormattedType { Type = type, Formats = orderedFormats } };
    }

    static private int Weight(int count, int priority)
    {
        if (count > 0) return count << 16 | priority;
        return 0;
    }

    #endregion

    static public int RowCount(IEnumerable<string> lines)
    { return RowCount(CharFromLines(lines)); }

    static public int RowCount(IEnumerable<char> text)
    { return text.Count(c => c == '\n'); }

    static public IEnumerable<string> Split(string text, Predicate<char> splitPredicate, bool includeSplit = false)
    {
        int from = 0, to = 0;

        while (from < text.Length)
        {
            while (to < text.Length && splitPredicate(text[to]))
                to++;

            if (includeSplit)
                yield return text.Substring(from, to - from + 1);
            else
                yield return text[from..to];

            from = to + 1;

            while (from < text.Length && !splitPredicate(text[from]))
                from++;

            to = from;
        }
    }

    static public IEnumerable<string> Words(string text)
    { return Split(text, c => Char.IsLetterOrDigit(c)); }

    static public IEnumerable<string> Sentences(string text)
    { foreach (var sentence in Split(text, c => !Char.IsPunctuation(c), true)) yield return sentence.Trim(); }

    static public IEnumerable<string> Permutations(string word)
    {
        for (int length = word.Length; length > 0; length--)
            for (int start = 0; start <= word.Length - length; start++)
                yield return word.Substring(start, length);
    }

    public static IEnumerable<string> Lines(string text, string? newLine = null)
    {
        int startIndex = 0;
        newLine ??= Environment.NewLine;
        int endIndex = text.IndexOf(newLine, startIndex);

        while (endIndex >= 0)
        {
            yield return text[startIndex..endIndex];
            startIndex = endIndex + newLine.Length;

            endIndex = text.IndexOf(newLine, startIndex);
        }

        yield return text[startIndex..];
    }
}
