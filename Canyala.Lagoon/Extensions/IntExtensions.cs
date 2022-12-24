//
// Copyright (c) 2013 Canyala Innovation AB
//
// All rights reserved.
//

using Canyala.Lagoon.Functional;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Canyala.Lagoon.Extensions
{
    public static class IntExtensions
    {
        public static IEnumerable<T> Generate<T>(this int count, Func<T> generator)
        { return Seq.Generate(count, generator); }
    }
}
