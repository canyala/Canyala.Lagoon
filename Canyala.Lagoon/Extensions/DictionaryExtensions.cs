//
// Copyright (c) 2013 Canyala Innovation AB
//
// All rights reserved.
//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Canyala.Lagoon.Extensions
{
    public static class DictionaryExtensions
    {
        public static IDictionary<TKey, TValue> Clone<TKey, TValue>(this IDictionary<TKey, TValue> table)
        {
            return new Dictionary<TKey, TValue>(table);
        }

        public static TValue GetSafe<TKey, TValue>(this IDictionary<TKey, TValue> table, TKey key)
        {
            TValue value;
            if (table.TryGetValue(key, out value))
                return value;

            return default(TValue);
        }
    }
}
