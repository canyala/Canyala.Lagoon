/*
 
  MIT License

  Copyright (c) 2012-2022 Canyala Innovation

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

namespace Canyala.Lagoon.Collections;

public class Bag<TKey> : IEnumerable<KeyValuePair<TKey, int>>
    where TKey : notnull
{
    private readonly Dictionary<TKey, int> _bag = new();

    public Bag(IEqualityComparer<TKey> comparer)
    {
        _bag = new Dictionary<TKey, int>(comparer);
    }

    public Bag(IEnumerable<TKey> seq)
    { seq.Do(Add); }

    public void Add(TKey element)
    {
        if (!_bag.ContainsKey(element))
            _bag.Add(element, 1);
        else
            _bag[element]++;
    }

    public void Remove(TKey element)
    {
        _bag.Remove(element);
    }

    public void Clear()
    {
        _bag.Clear();
    }

    public int this[TKey element]
    { get { return ElementCount(element); } }

    public int ElementCount(TKey element)
    {
        _bag.TryGetValue(element, out int count);
        return count;
    }

    public int Count { get { return _bag.Count; } }

    public bool Contains(TKey element)
    { return ElementCount(element) > 0; }

    public IEnumerator<KeyValuePair<TKey, int>> GetEnumerator()
    { return _bag.GetEnumerator(); }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    { return _bag.GetEnumerator(); }

    public IEnumerable<TKey> Keys
    { get { return _bag.Keys; } }
}
