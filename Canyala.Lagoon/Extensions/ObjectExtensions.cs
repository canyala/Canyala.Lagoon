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
    public static class ObjectExtensions
    {
        public static bool HasValue(this object obj)
        { return obj != null; }
    }
}
