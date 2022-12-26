//-------------------------------------------------------------------------------
//
//  MIT License
//
//  Copyright (c) 2012-2022 Canyala Innovation
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

using Canyala.Lagoon.Extensions;
using Canyala.Lagoon.Text;

namespace Canyala.Lagoon.Database;

public class Csv
{
    public static IEnumerable<string[]> ReadLines(IEnumerable<string> lines, char commaCharacter = ',')
        { foreach (var line in lines) yield return line.Split(commaCharacter); }

    public static IEnumerable<string[]> ReadText(string text, char commaCharacter = ',')
        { return ReadLines(Analyzer.Lines(text), commaCharacter); }

    public static IEnumerable<string[]> ReadFile(string path, char commaCharacter = ',')
        { return ReadLines(File.ReadLines(path), commaCharacter); }

    public static void WriteFile(string path, IEnumerable<string[]> lines, char commaCharacter = ',')
        { File.WriteAllLines(path, lines.Select(line => line.Join(commaCharacter))); }
}
