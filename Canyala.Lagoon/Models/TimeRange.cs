//-------------------------------------------------------------------------------
//
//  MIT License
//
//  Copyright (c) 2012-2022 Canyala Innovation
//
//  Permission is hereby granted, free of charge, to any person obtaining a copy
//  of this software and associated documentation files (the "Software"), to deal
//  in the Software without restriction, including without limitation the rights
//  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//  copies of the Software, and to permit persons to whom the Software is
//  furnished to do so, subject to the following conditions:
//
//  The above copyright notice and this permission notice shall be included in all
//  copies or substantial portions of the Software.
//
//  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//  SOFTWARE.
//
//------------------------------------------------------------------------------- 

using Canyala.Lagoon.Extensions;
using Canyala.Lagoon.Functional;

namespace Canyala.Lagoon.Models;

/// <summary>
/// Represents a range in time with a start point and and end point.
/// </summary>
public class TimeRange : IComparable<TimeRange>
{
    /// <summary>
    /// Returns an empty time range representing a range with no content.
    /// </summary>
    public static TimeRange Empty { get { return TimeRange.Create(DateTime.MaxValue, DateTime.MinValue); } }

    /// <summary>
    /// The start time of the range.
    /// </summary>
    public DateTime From { get; private set; }

    /// <summary>
    /// The end time of the range.
    /// </summary>
    public DateTime To { get; private set; }

    /// <summary>
    /// Tests if a time range is empty (From > To)
    /// </summary>
    public bool IsEmpty { get { return From > To; } }

    /// <summary>
    /// Constructs a new time range from a start and end point in time.
    /// </summary>
    /// <param name="from">start point in time for the range</param>
    /// <param name="to">end point in time for the range</param>
    /// <remarks>
    /// Setting From = DateTime.MinValue sets up a range where From is unspecified, in the
    /// same way setting To = DateTime.MaxValue sets up a range where To is unspecified.
    /// 
    /// So, TimeRange.Create(DateTime.MinValue, DateTime.Now) creates a range covering
    /// "everything up to now".
    /// </remarks>
    internal TimeRange(DateTime from, DateTime to)
    {
        From = from;
        To = to;
    }

    /// <summary>
    /// Converts the time range into a TimeSpan object.
    /// </summary>
    public TimeSpan AsTimeSpan { get { return To - From; } }

    /// <summary>
    /// Calculates the union of a set of time ranges, which is the total range of
    /// time from the earliest From value of any of the time ranges to the latest 
    /// To value of any of the time ranges.
    /// </summary>
    /// <param name="ranges">Set of other ranges to take union with.</param>
    /// <returns>The union of all the ranges.</returns>
    public TimeRange Union(params TimeRange[] ranges)
    {
        var maxTo = ranges.Max(item => item.To);
        var minFrom = ranges.Min(item => item.From);

        return Create(From < minFrom ? From : minFrom, To > maxTo ? To : maxTo);
    }

    /// <summary>
    /// Calculates the intersection of a set of time ranges, which is the smallest 
    /// common range of time covered by all the ranges specified.
    /// </summary>
    /// <param name="ranges">Set of other ranges to take intersection with.</param>
    /// <returns>Range covered by all the input ranges</returns>
    public TimeRange Intersection(params TimeRange[] ranges)
    {
        var maxFrom = Seq.Concat(From, ranges.Select(item => item.From)).Max();
        var minTo = Seq.Concat(To, ranges.Select(item => item.To)).Min();

        return Create(maxFrom, minTo);
    }

    /// <summary>
    /// Tests if a set of time ranges has a non empty intersection.
    /// </summary>
    /// <param name="ranges">Set of ranges to test for intersection.</param>
    /// <returns>true if a non empty intersection exists else false.</returns>
    public bool Intersects(params TimeRange[] ranges)
    { return !Intersection(ranges).IsEmpty; }

    /// <summary>
    /// Tests if this range is a sub range of another range, i.e
    /// if this range is completely covered by the specified range.
    /// </summary>
    /// <param name="range">Range to to test if this range is a sub range of.</param>
    /// <returns>true if this range is a subrange of the range else false.</returns>
    public bool IsSubRangeOf(TimeRange range)
    { return range.IsEmpty ? false : range.From <= From && range.To >= To; }

    /// <summary>
    /// Calculates the difference between this range and another range, i.e
    /// the parts of this range that does not intersect the specified range.
    /// </summary>
    /// <param name="range">The range to subtract from this range.</param>
    /// <returns>The specified range subtracted from this range.</returns>
    public IEnumerable<TimeRange> Difference(TimeRange range)
    {
        var from = From;
        var to = To;

        if (Intersects(range) == false)
            return Seq.Empty<TimeRange>();

        if (From < range.From && To > range.To)
            return Seq.Of(Create(From, range.From), Create(range.To, To));

        var intersection = Intersection(range);

        if (From < intersection.From)
        {
            from = From;
            to = intersection.From;
        }
        else
        {
            from = intersection.To;
            to = To;
        }

        if (to == from)
        {
            return Seq.Empty<TimeRange>();
        }

        return Seq.Of(Create(from, to));
    }

    /// <summary>
    /// Returns the complement of a set of ranges with respect to this range.
    /// </summary>
    /// <param name="ranges">ranges to take complement with</param>
    /// <returns>The sub ranges contained in this range that is not covered by any of the given ranges.</returns>
    public IEnumerable<TimeRange> Complement(params TimeRange[] ranges)
    {
        var intersections = new List<TimeRange>(Seq.Concat<TimeRange>(ranges.Select(r => Seq.Of(Intersection(r)))));

        var reduced = intersections.Reduce();

        if (reduced.Count() > 1)
        {
            List<TimeRange> result = new List<TimeRange>();
            DateTime time = From;
            foreach (var range in reduced)
            {
                if (time < range.From)
                    result.Add(TimeRange.Create(time, range.From));
                time = range.To;
            }

            if (time < To)
                result.Add(TimeRange.Create(time, To));

            return result;
        }
        else
            return Seq.Empty<TimeRange>();
    }

    /// <summary>
    /// Tests if this TimeRange is equal to another, i.e compares if
    /// From and To are equal respectively.
    /// </summary>
    /// <param name="obj">Other TimeRange to compare with</param>
    /// <returns>true if equal else false</returns>
    public override bool Equals(object? obj)
    {
        if (obj is TimeRange other)
            return From.Equals(other.From) && To.Equals(other.To);

        return false;
    }

    public override int GetHashCode()
    { return From.GetHashCode() ^ To.GetHashCode(); }

    public override string ToString()
    { return "{0} - {1}".Args(From, To); }

    /// <summary>
    /// Constructs a new TimeRange object.
    /// </summary>
    /// <param name="from">The start point in time of the range.</param>
    /// <param name="to">The end point in time of the range.</param>
    /// <returns>The new time range object.</returns>
    public static TimeRange Create(DateTime from, DateTime to)
    { return new TimeRange(from, to); }

    public int CompareTo(TimeRange? other)
    {
        if (other is null)
            throw new ArgumentNullException(nameof(other));

        if (From < other.From)
            return -1;
        else if (From > other.From)
            return 1;
        else
        {
            if (To < other.To)
                return 1;
            else if (To > other.To)
                return -1;
            else
                return 0;
        }
    }
}
