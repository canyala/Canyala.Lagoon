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

namespace Canyala.Lagoon.Extensions;

public static class TableExtensions
{
    /// <summary>
    /// Maps a table into a sequence of rows.
    /// </summary>
    /// <param name="table">The table</param>
    /// <returns>A sequence of rows.</returns>
    public static IEnumerable<string[]> AsRows(this string[,] table)
    {
        for (int row = 0; row < table.GetLength(0); row++)
            yield return table.AsColumns(row).ToArray();
    }

    /// <summary>
    /// Maps a table row into a sequence of columns.
    /// </summary>
    /// <param name="table">The table</param>
    /// <param name="row"></param>
    /// <returns>A sequnce of columns in a row</returns>
    public static IEnumerable<string> AsColumns(this string[,] table, int row)
    {
        for (int column = 0; column < table.GetLength(1); column++)
            yield return table[row, column];
    }
}
