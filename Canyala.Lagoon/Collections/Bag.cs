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

using Canyala.Lagoon.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Canyala.Lagoon.Collections
{
    public class Bag<T> : IEnumerable<KeyValuePair<T, int>>
    {
        private Dictionary<T, int> _bag = new Dictionary<T, int>();

        public Bag(IEqualityComparer<T> comparer)
        {
            _bag = new Dictionary<T, int>(comparer);
        }

        public Bag(IEnumerable<T> seq)
        { seq.Do(Add); }

        public void Add(T element)
        {
            if (!_bag.ContainsKey(element))
                _bag.Add(element, 1);
            else
                _bag[element]++;
        }

        public void Remove(T element)
        {
            if (_bag.ContainsKey(element))
                _bag.Remove(element);
        }

        public void Clear()
        {
            _bag.Clear();
        }

        public int this[T element]
        { get { return ElementCount(element); } }

        public int ElementCount(T element)
        {
            int count = 0;
            _bag.TryGetValue(element, out count);
            return count;
        }

        public int Count { get { return _bag.Count; } }

        public bool Contains(T element)
        { return ElementCount(element) > 0; }

        public IEnumerator<KeyValuePair<T, int>> GetEnumerator()
        { return _bag.GetEnumerator(); }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        { return _bag.GetEnumerator(); }

        public IEnumerable<T> Keys
        { get { return _bag.Keys; } }
    }
}
