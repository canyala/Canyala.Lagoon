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

namespace Canyala.Lagoon.Collections;

public class SubArray<T> : IEnumerable<T>
    where T : notnull
{
    private readonly ArraySegment<T> _segment;

    private SubArray(T[] parentArray, int offset, int count)
    { _segment = new ArraySegment<T>(parentArray, offset, count); }

    public static SubArray<T> Create(T[] parent, int offset, int count)
    { return new SubArray<T>(parent, offset, count); }

    public T this[int index]
    {
        get
        {
            if (index >= _segment.Count)
                throw new ArgumentOutOfRangeException(nameof(index));

            if (_segment.Array is null)
                throw new ArgumentNullException(nameof(index));

            return _segment.Array[index + _segment.Offset];
        }
    }

    public int Length { get { return _segment.Count; } }

    public IEnumerator<T> GetEnumerator()
    { return new Enumerator(this); }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    { return GetEnumerator(); }

    class Enumerator : IEnumerator<T>
    {
        private readonly SubArray<T> _array;
        private int _index;

        internal Enumerator(SubArray<T> array)
        {
            _array = array;
            _index = 0;
        }

        public T Current
        { get { return _array[_index]; } }

        object System.Collections.IEnumerator.Current
        { get { return Current; } }

        public bool MoveNext()
        { return ++_index < _array.Length; }

        public void Reset()
        { _index = 0; }

        public void Dispose()
        { }
    }
}
