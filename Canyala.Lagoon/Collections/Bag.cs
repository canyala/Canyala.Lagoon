//
// Copyright (c) 2013 Canyala Innovation AB
//
// All rights reserved.
//

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
