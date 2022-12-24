//
// Copyright (c) 2013 Canyala Innovation AB
//
// All rights reserved.
//

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Canyala.Lagoon.Functional
{
    public class GenericComparer<T> : Comparer<T>, IComparer
    {
        internal GenericComparer(Func<T, T, int> compareFunc)
        {
            _Compare = compareFunc;
        }

        protected Func<T, T, int> _Compare = (x, y) => Comparer<T>.Default.Compare(x, y);

        public override int Compare(T x, T y)
        {
            return _Compare(x, y);
        }
    }
}
