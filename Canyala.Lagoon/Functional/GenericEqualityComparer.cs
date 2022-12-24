//
// Copyright (c) 2013 Canyala Innovation AB
//
// All rights reserved.
//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Canyala.Lagoon.Functional
{
    /// <summary>
    /// Generic equality comparer implementation which is returned by the 
    /// Equality.With() static (<see cref="Equality"/>) method to be used with linq expressions where
    /// you need to supply an IEqualityComparer.
    /// </summary>
    /// <typeparam name="T">Type of objects being compared.</typeparam>
    internal class GenericEqualityComparer<T> : EqualityComparer<T>
    {
        internal GenericEqualityComparer(Func<T, T, bool> equalsFunc)
        {
            _Equals = equalsFunc;
        }

        internal GenericEqualityComparer(Func<T, T, bool> equalsFunc, Func<T, int> hashcodeFunc)
        {
            _Equals = equalsFunc;
            _Hashcode = hashcodeFunc;
        }

        protected Func<T, T, bool> _Equals = (x, y) => x.Equals(y);
        protected Func<T, int> _Hashcode = x => x.GetHashCode();

        public override bool Equals(T x, T y)
        {
            return _Equals(x, y);
        }

        public override int GetHashCode(T obj)
        {
            return _Hashcode(obj);
        }
    }
}
