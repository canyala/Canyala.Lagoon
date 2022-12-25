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


using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Canyala.Lagoon.Extensions;
using Canyala.Lagoon.Text;
using Canyala.Lagoon.Collections;
using Canyala.Lagoon.Functional;
using Canyala.Lagoon.Models;

namespace Canyala.Lagoon.Files
{
    /// <summary>
    /// Provides an interface for text table models.
    /// </summary>
    public interface ITable
    {
        /// <summary>
        /// The number of rows in the table.
        /// </summary>
        int RowCount { get; }
        /// <summary>
        /// The number of columns in the table.
        /// </summary>
        int ColumnCount { get; }
        /// <summary>
        /// The number of cells in the table.
        /// </summary>
        int CellCount { get; }
        /// <summary>
        /// The cells in the table a sequence.
        /// </summary>
        IEnumerable<string> Cells { get; }
        /// <summary>
        /// The rows in the table as a sequence.
        /// </summary>
        IEnumerable<IEnumerable<string>> Rows { get; }
        /// <summary>
        /// The columns in the table as a sequence.
        /// </summary>
        IEnumerable<IEnumerable<string>> Columns { get; }
        /// <summary>
        /// The cells in a specific column as a sequence.
        /// </summary>
        /// <param name="index">The index of specific column.</param>
        /// <returns>A column as a sequence of cells.</returns>
        IEnumerable<string> Column(int index);
        /// <summary>
        /// The cells in a specific row as a sequence.
        /// </summary>
        /// <param name="index">The index of a specific row.</param>
        /// <returns>A row as a sequence of cells.</returns>
        IEnumerable<string> Row(int index);
        /// <summary>
        /// The content of a specific cell.
        /// </summary>
        /// <param name="rowIndex">The row index of a specific cell.</param>
        /// <param name="columnIndex">The column index of a specific cell.</param>
        /// <returns>A cell as a string.</returns>
        string Cell(int rowIndex, int columnIndex);
        /// <summary>
        /// The format of a specific column.
        /// </summary>
        /// <param name="index">The column index of a specific column.</param>
        /// <returns>The formats of the specific column as a sequence of strings.</returns>
        IEnumerable<string> ColumnFormats(int index);
        /// <summary>
        /// The name (heading) of a specific column.
        /// </summary>
        /// <param name="index">The column index of a specific column.</param>
        /// <returns>The name of the specific column as a string.</returns>
        string ColumnName(int index);
        /// <summary>
        /// No argument
        /// </summary>
        /// <returns>All column names as strings</returns>
        IEnumerable<string> ColumnNames { get; }
        /// <summary>
        /// The type of a specific column.
        /// </summary>
        /// <param name="index">The column index of specific column.</param>
        /// <returns>The content type of the specific column as a System.Type</returns>
        Type ColumnType(int index);
        /// <summary>
        /// The row separator char.
        /// </summary>
        char ColumnSeparator { get; }
    }

    /// <summary>
    /// Provides a model for in memory text tables.
    /// </summary>
    public class FileTable : ITable
    {        
        private List<string[]> _lines;
        private List<string> _columnNames;
        private List<FormattedType> _columnFormattedTypes;
        private int _columnCount;

        public int RowCount { get { return _lines.Count; } }
        public int ColumnCount { get { return _columnCount; } }
        public int CellCount { get { return RowCount * ColumnCount; } }

        public IEnumerable<string> Cells
        {
            get
            {
                foreach (var line in _lines)
                    foreach (var cell in line)
                        yield return cell;
            }
        }

        public IEnumerable<IEnumerable<string>> Rows
        {
            get 
            {
                for (int i=0; i<RowCount; i++)
                    yield return Row(i);
            }
        }

        public IEnumerable<IEnumerable<string>> Columns
        {
            get 
            {
                for (int i = 0; i < ColumnCount; i++)
                    yield return Column(i);
            }
        }

        public IEnumerable<string> Column(int index)
        {
            foreach (var line in _lines)
            {
                if (index < line.Length) yield return line[index];
            }
        }

        public IEnumerable<string> Row(int index)
        {
            foreach (var cell in _lines[index])
                yield return cell;
        }

        public string Cell(int row, int column)
        {
            return _lines[row][column];
        }

        public IEnumerable<string> ColumnFormats(int index)
        {
            return _columnFormattedTypes[index].Formats;
        }

        public string ColumnName(int index)
        {
            return _columnNames[index];
        }

        public IEnumerable<string> ColumnNames
        {
            get { return _columnNames; }
        }

        public Type ColumnType(int index)
        {
            return _columnFormattedTypes[index].Type;
        }

        public char ColumnSeparator { get; private set; }

        private FileTable(IEnumerable<string> lines, char columnSeparator)
        {
            ColumnSeparator = columnSeparator;

            _columnFormattedTypes = new List<FormattedType>();
            _lines = new List<string[]>(lines.Select(line => line.Split(columnSeparator)));
            _columnCount = _lines.Max(line => line.Length);            

            if (Analyzer.LikelyFormattedType(_lines[0]).Type == typeof(string))
            {
                _columnNames = new List<string>(_lines.TakeAt(0).Select(item => Analyzer.AsPropertyName(item)));

                var enumed = new Bag<string>(StringComparer.OrdinalIgnoreCase);
                foreach (var name in _columnNames)
                {
                    string enumPrefix;
                    if (name.TryGetEnumPrefix(out enumPrefix))
                        enumed.Add(enumPrefix);
                }
            }
            else
                _columnNames = new List<string>(0.UpTo(_columnCount).Select(i => String.Format("Column{0}", i)));

            Columns.Do(column => _columnFormattedTypes.Add(Analyzer.LikelyFormattedType(column)));
        }

        static public ITable FromLines(IEnumerable<string> lines, char columnSeparator)
            { return new FileTable(lines, columnSeparator); }

        static public ITable FromLines(IEnumerable<string> lines)
            { return new FileTable(lines, Analyzer.LikelyColumnSeparator(lines)); }

        static private readonly string[] LineSeparators = { Environment.NewLine };

        static public ITable FromText(string text, char columnSeparator)
            { return new FileTable(text.Split(LineSeparators, StringSplitOptions.None), columnSeparator); }

        static public ITable FromFile(string fileName, char columnSeparator)
            { return new FileTable(File.ReadLines(fileName), columnSeparator); }

        static public ITable FromFile(string fileName)
            { return FileTable.FromLines(File.ReadLines(fileName)); }

        static public ITable FromText(string text)
            { return FromText(text, Analyzer.LikelyColumnSeparator(text)); }
    }
}
