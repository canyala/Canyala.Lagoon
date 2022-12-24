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
    public static class DateTimeExtensions
    {
        public static long[] AsTicks(this DateTime[] times)
        {
            var values = new long[times.Length];

            for (int i = 0; i < times.Length; i++)
                values[i] = times[i].Ticks;

            return values;
        }
    }
}
