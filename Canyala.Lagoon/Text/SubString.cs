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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Canyala.Lagoon.Text
{
    /// <summary>
    /// Provides an immutable substring implementation that avoids
    /// memory allocations by using struct and char array mapping
    /// to improve performance and allow parallellized use.
    /// </summary>
    public struct SubString
    {
        private readonly char[] _array;
        private readonly int _start, _end;

        public int Length { get { return _end - _start; } }

        public SubString(char[] array, int start, int end)
        {
            _array = array;
            _start = start;
            _end = end;
        }

        public SubString(string text)
            : this(text.ToCharArray(), 0, text.Length)
        { }

        public SubString(SubString substr, int start, int end)
            : this(substr._array, substr._start + start, substr._start + end)
        { }

        public SubString(SubString substr, int start = 0)
            : this(substr._array, substr._start + start, substr._end)
        { }

        public override string ToString()
        {
            return new String(_array, _start, _end - _start);
        }

        public char this[int index]
        { get { return _array[_start + index]; } }

        public SubString this[int start, int end]
        { get { return Trim(start, end); } }

        public SubString Trim(int start, int end)
        {
            if (end > 0)
                return new SubString(_array, _start + start, _start + end);

            return new SubString(_array, _start + start, _end + end);
        }

        public SubString TrimStart()
        {
            var start = _start;

            while (start < _end && Char.IsWhiteSpace(_array[start]))
                start++;

            return new SubString(_array, start, _end);
        }

        public SubString Trim()
        {
            if (_start == _end)
                return this;

            var start = _start;
            var pos = _end - 1;

            while (start < _end && Char.IsWhiteSpace(_array[start]))
                start++;

            while (pos > start && Char.IsWhiteSpace(_array[pos]))
                pos--;

            return new SubString(_array, start, pos + 1);
        }

        private bool SkipBody(ref int pos, char start, char end)
        {
            if (_array[pos] == start)
            {
                pos++;
                if (_array[pos] != end)
                {
                    while (_array[pos] != end)
                    {
                        if (!SkipBodies(ref pos))
                            pos++;
                    }
                }

                pos++;
                return true;
            }

            return false;
        }

        private bool SkipStringConstant(ref int pos)
        {
            if (_array[pos] == '"')
            {
                pos++;
                if (_array[pos] != '"')
                {
                    while (_array[pos] != '"')
                    {
                        if (_array[pos] == '\\') pos++;
                        pos++;
                    }
                }
                pos++;
                return true;
            }

            return false;
        }

        private bool SkipBodies(ref int pos)
        {
            if (!SkipBody(ref pos, '[', ']'))
                if (!SkipBody(ref pos, '{', '}'))
                    if (!SkipBody(ref pos, '(', ')'))
                        if (!SkipBody(ref pos, '<', '>'))
                            if (!SkipStringConstant(ref pos))
                                return false;

            return true;
        }

        public IEnumerable<SubString> Split(char splitChar, bool keepEmpty = false)
        {
            if (keepEmpty)
                return SplitKeepEmpty(splitChar);

            return SplitSkipEmpty(splitChar);
        }

        private IEnumerable<SubString> SplitKeepEmpty(char splitChar)
        {
            int pos = _start, from = _start;

            while (pos < _end)
            {
                if (!SkipBodies(ref pos))
                {
                    if (_array[pos] == splitChar)
                    {
                        yield return new SubString(_array, from, pos);
                        from = pos + 1;
                    }

                    pos++;
                }
            }

            yield return new SubString(_array, from, pos);
        }

        private IEnumerable<SubString> SplitSkipEmpty(char splitChar)
        {
            int pos = _start, from = _start;

            while (pos < _end)
            {
                if (!SkipBodies(ref pos))
                {
                    if (_array[pos] == splitChar)
                    {
                        if (pos > from)
                            yield return new SubString(_array, from, pos);

                        from = pos + 1;
                    }

                    pos++;
                }
            }

            if (pos > from)
                yield return new SubString(_array, from, pos);
        }

        public char FromStart(int index)
        { return _array[_start + index]; }

        public char FromEnd(int index)
        { return _array[_end - index - 1]; }

        public char First
        { get { return _array[_start]; } }

        public char Last
        { get { return _array[_end - 1]; } }

        public SubString FirstHidden
        { get { return new SubString(this, -_start, _start); } }

        public SubString LastLine
        {
            get
            {
                int pos = _end - 1;
                while (pos-- >= _start && _array[pos] != '\r' && _array[pos] != '\n') ;
                return new SubString(this, -pos, _start);
            }
        }

        public bool StartsWith(string s, bool caseSensitive = true)
        {
            if (s.Length > Length) return false;

            if (caseSensitive)
            {
                for (int i = 0; i < s.Length; i++)
                {
                    if (s[i] != _array[_start + i])
                        return false;
                }
            }
            else
            {
                for (int i = 0; i < s.Length; i++)
                {
                    if (char.ToLower(s[i]) != char.ToLower(_array[_start + i]))
                        return false;
                }
            }

            return true;
        }

        public static bool operator ==(SubString lhs, SubString rhs)
        {
            var length = lhs.Length;

            if (lhs._array == rhs._array && lhs._start == rhs._start && lhs._end == rhs._end)
                return true;

            if (length == rhs.Length)
                for (int i = 0; i < length; i++)
                    if (lhs._array[i] != rhs._array[i])
                        return false;

            return true;
        }

        public static bool operator !=(SubString lhs, SubString rhs)
        { return !(lhs == rhs); }

        public static bool operator ==(SubString lhs, string rhs)
        {
            var length = lhs.Length;

            if (length != rhs.Length)
                return false;

            for (int i = 0; i < length; i++)
                if (lhs._array[lhs._start + i] != rhs[i])
                    return false;

            return true;
        }

        public static bool operator !=(SubString lhs, string rhs)
        { return !(lhs == rhs); }

        public override bool Equals(object obj)
        {
            if (obj is SubString)
            {
                var other = (SubString)obj;
                return this == other;
            }

            if (obj is String)
            {
                var other = (String)obj;
                return this == other;
            }

            return false;
        }

        public override int GetHashCode()
        { return ToString().GetHashCode(); }

        public static implicit operator string(SubString subStr)
        { return subStr.ToString(); }
    }
}
