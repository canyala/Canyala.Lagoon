//
// Copyright (c) 2013 Canyala Innovation AB
//
// All rights reserved.
//

using Canyala.Lagoon.Functional;
using Canyala.Lagoon.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Canyala.Lagoon.Extensions
{
    public static class SequenceExtensions
    {
        public static IEnumerable<T> Rest<T>(this IEnumerable<T> seq)
        { return seq.Skip(1); }

        public static IEnumerable<String> SkipEmpty(this IEnumerable<String> seq)
        { return seq.Where(s => s.Length > 0); }

        public static IEnumerable<SubString> SkipEmpty(this IEnumerable<SubString> seq)
        { return seq.Where(s => s.Length > 0); }

        public static IEnumerable<T> Do<T>(this IEnumerable<T> seq, Action<T> action)
        {
            Seq.Do(seq, action);
            return seq;
        }

        public static IEnumerable<T> TakeFromCurrent<T>(this IEnumerator<T> enumerator, int numberOfItems)
        {
            if (numberOfItems == 0)
                yield break;

            while (numberOfItems > 0)
            {
                yield return enumerator.Current;

                if (--numberOfItems > 0)
                    if (!enumerator.MoveNext())
                        yield break;
            }
        }

        public static IEnumerable<IEnumerable<T>> Split<T>(this IEnumerable<T> seq, int partitionSize)
        {
            var enumerator = seq.GetEnumerator();

            while (enumerator.MoveNext())
            {
                yield return enumerator.TakeFromCurrent(partitionSize).ToList();
            }
        }

        public static IEnumerable<T1> FilterBy<T1, T2>(this IEnumerable<T1> first, IEnumerable<T2> filterCollection, Func<T1, T2, bool> filter)
        {
            foreach (var item1 in first)
                foreach (var item2 in filterCollection)
                    if (filter(item1, item2))
                        yield return item1;
        }

        /// <summary>
        /// Find which elements in first sequence that is not present in the seconde secuence
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public static IEnumerable<T> Complement<T>(this IEnumerable<T> first, IEnumerable<T> second)
        {
            var intersect = first.Intersect(second);
            foreach (var item in first.Where(element => !intersect.Any(i => element.Equals(i))))
                yield return item;
        }

        /// <summary>
        /// There is often a problem removing items from a collection since you mess up the iterator once one item is removed.
        /// This method solves this when working with IEnumerable
        /// </summary>
        /// <typeparam name="T">the type of the sequence</typeparam>
        /// <param name="sequence">the original sequence</param>
        /// <param name="itemsToRemove">the items in the original sequence to remove</param>
        public static IEnumerable<T> Remove<T>(this IEnumerable<T> sequence, IEnumerable<T> itemsToRemove)
        {
            return sequence.Complement(itemsToRemove);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public static IEnumerable<char> UpTo(this char start, char end)
        {
            for (var i = start; i <= end; i++)
                yield return i;
        }

        /// <summary>
        /// Creates a seqence of rising integers.
        /// </summary>
        /// <param name="start">the first int in the sequence.</param>
        /// <param name="end">the last int in the sequence.</param>
        /// <returns>An enumeration of the integers from start upto end</returns>
        /// <remarks>
        /// The end value must be greater than the start value.
        /// 
        /// Examples:
        /// 1.UpTo(5)        => 1 2 3 4 5
        /// (-5).UpTo(5)     => -5 -4 -3 -2 -1 0 1 2 3 4 5
        /// </remarks>
        public static IEnumerable<int> UpTo(this int start, int end)
        {
            for (var i = start; i <= end; i++)
                yield return i;
        }

        /// <summary>
        /// Creates a seqence of rising integers.
        /// </summary>
        /// <param name="start">the first int in the sequence.</param>
        /// <param name="end">the last int in the sequence.</param>
        /// <param name="step">the step length to use.</param>
        /// <returns>An enumeration of the integers from start upto end with steps.</returns>
        /// <remarks>
        /// The step parameter must be strictly positive (not zero or negative) and
        /// the end value must be greater than the start value.
        /// 
        /// Examples:
        /// 1.UpTo(5, 1)        => 1 2 3 4 5
        /// 1.UpTo(6, 2)        => 1 3 5
        /// (-5).UpTo(5, 2)     => -5 -3 -1 1 3 5
        /// </remarks>
        public static IEnumerable<int> UpTo(this int start, int end, int step)
        {
            for (int i = start; i <= end; i += step)
                yield return i;
        }

        /// <summary>
        /// Creates a seqence of rising long integers.
        /// </summary>
        /// <param name="start">the first long in the sequence.</param>
        /// <param name="end">the last long in the sequence.</param>
        /// <returns>An enumeration of the long values from start up to end.</returns>
        /// <remarks>
        /// The end value must be greater than the start value.
        /// 
        /// Examples:
        /// 1.UpTo(5)        => 1 2 3 4 5
        /// (-5).UpTo(5)     => -5 -4 -3 -2 -1 0 1 2 3 4 5
        /// </remarks>
        public static IEnumerable<long> UpTo(this long start, long end)
        {
            for (long i = start; i <= end; i++)
                yield return i;
        }

        /// <summary>
        /// Creates a seqence of rising long integers.
        /// </summary>
        /// <param name="start">the first long in the sequence.</param>
        /// <param name="end">the last long in the sequence.</param>
        /// <param name="step">the step length to use.</param>
        /// <returns>An enumeration of the long values from start up to end with steps.</returns>
        /// <remarks>
        /// The step parameter must be strictly positive (not zero or negative) and
        /// the end value must be greater than the start value.
        /// 
        /// Examples:
        /// 1.UpTo(5, 1)        => 1 2 3 4 5
        /// 1.UpTo(6, 2)        => 1 3 5
        /// (-5).UpTo(5, 2)     => -5 -3 -1 1 3 5
        /// </remarks>
        public static IEnumerable<long> UpTo(this long start, long end, long step)
        {
            for (long i = start; i <= end; i += step)
                yield return i;
        }

        /// <summary>
        /// Creates a sequence of DateTime's
        /// </summary>
        /// <param name="start">Start</param>
        /// <param name="end">End</param>
        /// <param name="step">Step</param>
        /// <returns>Sequence</returns>
        public static IEnumerable<DateTime> UpTo(this DateTime start, DateTime end, TimeSpan step)
        {
            for (var i = start; i <= end; i += step)
                yield return i;
        }

        /// <summary>
        /// A sequence of falling integers.
        /// </summary>
        /// <param name="start">the first value.</param>
        /// <param name="end">the last value.</param>
        /// <returns>An enumeration of the integers from start down to end.</returns>
        /// <remarks>
        /// The end value must be smaller than the start value.
        /// 
        /// Examples:
        /// 5.DownTo(1)          => 5 4 3 2 1
        /// (-1).DownTo(-5)     => -1 -2 -3 -4 -5
        /// </remarks>
        public static IEnumerable<int> DownTo(this int start, int end)
        {
            for (int i = start; i >= end; i--)
                yield return i;
        }

        /// <summary>
        /// A sequence of falling integers.
        /// </summary>
        /// <param name="start">the first value.</param>
        /// <param name="end">the last value.</param>
        /// <param name="step">the step length.</param>
        /// <returns>An enumeration of the integers from start down to end with steps.</returns>
        /// <remarks>
        /// The step parameter must be strictly positive (not zero or negative) and
        /// the end value must be smaller than the start value.
        /// 
        /// Examples:
        /// 5.DownTo(1, 1)          => 5 4 3 2 1
        /// 5.DownTo(1, 2)          => 5 3 1
        /// (-1).DownTo(-10, 3)     => -1 -4 -7 -10
        /// </remarks>
        public static IEnumerable<int> DownTo(this int start, int end, int step)
        {
            for (int i = start; i >= end; i -= step)
                yield return i;
        }

        /// <summary>
        /// A sequence of falling long values.
        /// </summary>
        /// <param name="start">the first value.</param>
        /// <param name="end">the last value.</param>
        /// <returns>An enumeration of the long values from start down to end.</returns>
        /// <remarks>
        /// The end value must be smaller than the start value.
        /// 
        /// Examples:
        /// 5.DownTo(1)          => 5 4 3 2 1
        /// (-1).DownTo(-5)      => -1 -2 -3 -4 -5 
        /// </remarks>
        public static IEnumerable<long> DownTo(this long start, long end)
        {
            for (long i = start; i >= end; i--)
                yield return i;
        }

        /// <summary>
        /// A sequence of falling long values.
        /// </summary>
        /// <param name="start">the first value.</param>
        /// <param name="end">the last value.</param>
        /// <param name="step">the step length.</param>
        /// <returns>An enumeration of the long values from start down to end with steps.</returns>
        /// <remarks>
        /// The step parameter must be strictly positive (not zero or negative) and
        /// the end value must be smaller than the start value.
        /// 
        /// Examples:
        /// 5.DownTo(1, 1)          => 5 4 3 2 1
        /// 5.DownTo(1, 2)          => 5 3 1
        /// (-1).DownTo(-10, 3)     => -1 -4 -7 -10
        /// </remarks>
        public static IEnumerable<long> DownTo(this long start, long end, long step)
        {
            for (long i = start; i >= end; i -= step)
                yield return i;
        }

        /// <summary>
        /// Returns a sequence of all pairwise combinations of two sequences (creating 
        /// essentially the cartesian product of the two sequences).
        /// </summary>
        /// <typeparam name="T1">The type of the elements of the first sequence.</typeparam>
        /// <typeparam name="T2">The type of the elements of the second sequence.</typeparam>
        /// <param name="first">The first sequence</param>
        /// <param name="second">The second sequence</param>
        /// <returns>A sequence of Tuples of all combinations of pairs of elements from first and second sequences.</returns>
        /// <remarks>
        /// <example>
        /// <code>
        /// var list1 = new List<int> { 1, 2 };
        /// var list2 = new List<string> { "one", "two", "three" };
        /// var combinations = list1.Combine(list2)                 // => (1, "one") (1, "two") (1, "three") (2, "one") (2, "two") (2, "three")
        /// </code>
        /// </example>
        /// </remarks>
        public static IEnumerable<Tuple<T1, T2>> CartesianProduct<T1, T2>(this IEnumerable<T1> first, IEnumerable<T2> second)
        {
            foreach (var i1 in first)
                foreach (var i2 in second)
                    yield return Tuple.Create(i1, i2);
        }

        /// <summary>
        /// Returns a sequence of all triplets combinations of three sequences (creating 
        /// essetially the cartesian product of the sequences).
        /// </summary>
        /// <typeparam name="T1">The type of the elements of the first sequence.</typeparam>
        /// <typeparam name="T2">The type of the elements of the second sequence.</typeparam>
        /// <typeparam name="T3">The type of the elements of the third sequence.</typeparam>
        /// <param name="first">The first sequence</param>
        /// <param name="second">The second sequence</param>
        /// <param name="third">The third sequence</param>
        /// <returns>A sequence of Tuples of all combinations of tripplets of elements from the sequences.</returns>
        /// <remarks>
        /// <example>
        /// Using Combine() to generate cartesian product of 3 sequences.
        /// <code>
        /// var list1 = new List<int> { 1, 2 };
        /// var list2 = new List<string> { "one", "two", "three" };
        /// var list3 = new List<bool> { true, false };
        /// var combinations = list1.Combine(list2, list3)                 
        /// // result is:
        /// (1, "one", true) 
        /// (1, "one", false) 
        /// (1, "two", true) 
        /// (1, "two", false) 
        /// (1, "three", true) 
        /// (1, "three", false) 
        /// (2, "one", true)
        /// (2, "one", false)
        /// (2, "two", true)
        /// (2, "two", false) 
        /// (2, "three", true)
        /// (2, "three", false)
        /// </code>
        /// </example>
        /// </remarks>
        public static IEnumerable<Tuple<T1, T2, T3>> CartesianProduct<T1, T2, T3>(this IEnumerable<T1> first, IEnumerable<T2> second, IEnumerable<T3> third)
        {
            foreach (var i1 in first)
                foreach (var i2 in second)
                    foreach (var i3 in third)
                        yield return Tuple.Create(i1, i2, i3);
        }

        /// <summary>
        /// Returns a sequence of all quadtruple combinations of three sequences (creating 
        /// essetially the cartesian product of the sequences).
        /// </summary>
        /// <typeparam name="T1">The type of the elements of the first sequence.</typeparam>
        /// <typeparam name="T2">The type of the elements of the second sequence.</typeparam>
        /// <typeparam name="T3">The type of the elements of the third sequence.</typeparam>
        /// <typeparam name="T4">The type of the elements of the fourth sequence.</typeparam>
        /// <param name="first">The first sequence</param>
        /// <param name="second">The second sequence</param>
        /// <param name="third">The third sequence</param>
        /// <param name="fourth">The fourth sequence</param>
        /// <returns>A sequence of Tuples of all combinations of quadtruplets of elements from the sequences.</returns>
        public static IEnumerable<Tuple<T1, T2, T3, T4>> CartesianProduct<T1, T2, T3, T4>(this IEnumerable<T1> first, IEnumerable<T2> second, IEnumerable<T3> third, IEnumerable<T4> fourth)
        {
            foreach (var i1 in first)
                foreach (var i2 in second)
                    foreach (var i3 in third)
                        foreach (var i4 in fourth)
                            yield return Tuple.Create(i1, i2, i3, i4);
        }

        /// <summary>
        /// Creates a sequence of pairs of the successive elements from two sequences.
        /// </summary>
        /// <typeparam name="T1">The type of the elements of the first sequence.</typeparam>
        /// <typeparam name="T2">The type of the elements of the second sequence.</typeparam>
        /// <param name="first">The first sequence</param>
        /// <param name="second">The second sequence</param>
        /// <param name="strict">Flag that when true stops the pairing at the end of the shortest sequence.</param>
        /// <returns>A sequence of Tuples of pairs of successive elements from the sequences</returns>
        /// <remarks>
        /// In strict mode the pairing stops at the end of the shortest sequence whereas
        /// in non strict mode it does not. In non strict mode the elements from the shorter sequence
        /// is filled in with the default(T) value for the type of the shorter sequence.
        /// <code>
        /// var list1 = new List&lt;string&gt; { "foo", "bar" };
        /// var list2 = new List&lt;string&gt; { "one", "two", "three" };
        /// list1.Pairs(list2, true);                                   // ("foo", "one") ("bar", "two")
        /// list1.Pairs(list2, false);                                  // ("foo", "one") ("bar", "two") (null, "three")
        /// </code>
        /// </remarks>
        public static IEnumerable<Tuple<T1, T2>> PairWith<T1, T2>(this IEnumerable<T1> first, IEnumerable<T2> second, bool strict)
        {
            var firstEnumerator = first.GetEnumerator();
            var secondEnumerator = second.GetEnumerator();

            if (strict)
            {
                while (firstEnumerator.MoveNext() && secondEnumerator.MoveNext())
                {
                    yield return Tuple.Create(firstEnumerator.Current, secondEnumerator.Current);
                }
            }
            else
            {
                bool stop;
                do
                {
                    bool firstHasItems = firstEnumerator.MoveNext();
                    bool secondHasItems = secondEnumerator.MoveNext();
                    stop = !(firstHasItems || secondHasItems);

                    if (firstHasItems && secondHasItems)
                        yield return Tuple.Create(firstEnumerator.Current, secondEnumerator.Current);
                    else if (firstHasItems)
                        yield return Tuple.Create(firstEnumerator.Current, default(T2));
                    else if (secondHasItems)
                        yield return Tuple.Create(default(T1), secondEnumerator.Current);
                } while (!stop);
            }
        }

        /// <summary>
        /// Creates a sequence of tripplets of the successive elements from three sequences.
        /// </summary>
        /// <typeparam name="T1">The type of the elements of the first sequence</typeparam>
        /// <typeparam name="T2">The type of the elements of the second sequence</typeparam>
        /// <typeparam name="T3">The type of the elements of the third sequence</typeparam>
        /// <param name="first">The first sequence</param>
        /// <param name="second">The second sequence</param>
        /// <param name="third">The third sequence</param>
        /// <param name="strict">Flag that when true stops the generation of tripplets at the end of the shortest sequence.</param>
        /// <returns>A sequence of Tuples of tripplets of successive elements from the sequences</returns>
        /// /// <remarks>
        /// In strict mode the generation of tripplets stops at the end of the shortest sequence whereas
        /// in non strict it mode does not. In non strict mode the elements from the shorter sequences
        /// is filled in with the default(T) value for the type of the those sequences.
        /// <code>
        /// var list1 = new List<string> { "foo", "bar" };
        /// var list2 = new List<string> { "one", "two", "three" };
        /// var list3 = new List<string> { 1, 2, 3, 4 };
        /// list1.Pairs(list2, list3, true);                                   // ("foo", "one", 1) ("bar", "two", 2)
        /// list1.Pairs(list2, list3, false);                                  // ("foo", "one", 1) ("bar", "two", 2) (null, "three", 3) (null, null, 4)
        /// </code>
        /// </remarks>
        public static IEnumerable<Tuple<T1, T2, T3>> PairWith<T1, T2, T3>(this IEnumerable<T1> first, IEnumerable<T2> second, IEnumerable<T3> third, bool strict)
        {
            var firstEnumerator = first.GetEnumerator();
            var secondEnumerator = second.GetEnumerator();
            var thirdEnumerator = third.GetEnumerator();

            if (strict)
            {
                while (firstEnumerator.MoveNext() && secondEnumerator.MoveNext() && thirdEnumerator.MoveNext())
                {
                    yield return Tuple.Create(firstEnumerator.Current, secondEnumerator.Current, thirdEnumerator.Current);
                }
            }
            else
            {
                bool stop;
                do
                {
                    bool firstHasItems = firstEnumerator.MoveNext();
                    bool secondHasItems = secondEnumerator.MoveNext();
                    bool thirdHasItems = thirdEnumerator.MoveNext();
                    stop = !(firstHasItems || secondHasItems || thirdHasItems);

                    if (firstHasItems && secondHasItems && thirdHasItems)
                        yield return Tuple.Create(firstEnumerator.Current, secondEnumerator.Current, thirdEnumerator.Current);
                    else if (firstHasItems && secondHasItems)
                        yield return Tuple.Create(firstEnumerator.Current, secondEnumerator.Current, default(T3));
                    else if (firstHasItems && thirdHasItems)
                        yield return Tuple.Create(firstEnumerator.Current, default(T2), thirdEnumerator.Current);
                    else if (secondHasItems && thirdHasItems)
                        yield return Tuple.Create(default(T1), secondEnumerator.Current, thirdEnumerator.Current);
                    else if (firstHasItems)
                        yield return Tuple.Create(firstEnumerator.Current, default(T2), default(T3));
                    else if (secondHasItems)
                        yield return Tuple.Create(default(T1), secondEnumerator.Current, default(T3));
                    else if (thirdHasItems)
                        yield return Tuple.Create(default(T1), default(T2), thirdEnumerator.Current);
                } while (!stop);
            }
        }

        /// <summary>
        /// Calculates an aggregated hash code from the hashcodes of the elements of the sequence
        /// up to a maximum number of elements.
        /// </summary>
        /// <typeparam name="T">type of the elements of the sequence</typeparam>
        /// <param name="seq">the sequence</param>
        /// <param name="maxElementsToUse">maximum elements to use</param>
        /// <returns>aggregated hashcode</returns>
        public static int GetHashCode<T>(this IEnumerable<T> seq, int maxElementsToUse)
        {
            var elements = seq.Take(maxElementsToUse);
            int hash = 0;
            elements.Do(item => hash = 27 * hash + item.GetHashCode());
            return hash;
        }

        public static bool IsEmpty<T>(this IEnumerable<T> seq)
        { return seq.Count() == 0; }

        public static bool AreSame<T>(this IEnumerable<T> seq)
        {
            if (seq.Any())
            {
                var first = seq.First();
                return seq.All(item => item.Equals(first));
            }
            else
                return true;
        }
    }
}
