//
// Copyright (c) 2013 Canyala Innovation AB
//
// All rights reserved.
//

using System.Collections.Generic;
using System.Linq;

namespace Canyala.Lagoon.Extensions
{
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
}
