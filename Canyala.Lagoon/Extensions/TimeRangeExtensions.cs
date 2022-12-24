//
// Copyright (c) 2013 Canyala Innovation AB
//
// All rights reserved.
//

using Canyala.Lagoon.Functional;
using Canyala.Lagoon.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Canyala.Lagoon.Extensions
{
    public static class TimeRangeExtensions
    {
        public static IEnumerable<TimeRange> Reduce(this IEnumerable<TimeRange> ranges)
        {
            List<TimeRange> input = new List<TimeRange>(ranges);
            List<TimeRange> result = new List<TimeRange>();

            if (input.Any())
            {
                TimeRange current = TimeRange.Empty;
                input.Sort();

                foreach (var range in input)
                {
                    if (current.IsEmpty)
                        current = range;
                    else
                    {
                        if (current.Intersects(range))
                            current = current.Union(range);
                        else
                        {
                            result.Add(current);
                            current = range;
                        }
                    }
                }

                result.Add(current);
            }

            return result;
        }
    }
}
