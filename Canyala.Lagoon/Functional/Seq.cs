//
// Copyright (c) 2013 Canyala Innovation AB
//
// All rights reserved.
//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Canyala.Lagoon.Functional
{
    /// <summary>
    /// Provides a functional factory
    /// </summary>
    public static class Seq
    {
        public static IEnumerable<T> Of<T>()
        {
            yield break;
        }

        public static IEnumerable<T> Of<T>(T t1)
        {
            yield return t1;
        }

        public static IEnumerable<T> Of<T>(T t1, T t2)
        {
            yield return t1;
            yield return t2;
        }

        public static IEnumerable<T> Of<T>(T t1, T t2, T t3)
        {
            yield return t1;
            yield return t2;
            yield return t3;
        }

        public static IEnumerable<T> Of<T>(T t1, T t2, T t3, T t4)
        {
            yield return t1;
            yield return t2;
            yield return t3;
            yield return t4;
        }

        public static IEnumerable<T> Of<T>(T t1, T t2, T t3, T t4, T t5)
        {
            yield return t1;
            yield return t2;
            yield return t3;
            yield return t4;
            yield return t5;
        }

        public static IEnumerable<T> Of<T>(T t1, T t2, T t3, T t4, T t5, T t6)
        {
            yield return t1;
            yield return t2;
            yield return t3;
            yield return t4;
            yield return t5;
            yield return t6;
        }

        public static IEnumerable<T> Of<T>(T t1, T t2, T t3, T t4, T t5, T t6, T t7)
        {
            yield return t1;
            yield return t2;
            yield return t3;
            yield return t4;
            yield return t5;
            yield return t6;
            yield return t7;

        }

        public static IEnumerable<T> Of<T>(T t1, T t2, T t3, T t4, T t5, T t6, T t7, T t8)
        {
            yield return t1;
            yield return t2;
            yield return t3;
            yield return t4;
            yield return t5;
            yield return t6;
            yield return t7;
            yield return t8;
        }

        public static IEnumerable<T> Of<T>(T t1, T t2, T t3, T t4, T t5, T t6, T t7, T t8, T t9)
        {
            yield return t1;
            yield return t2;
            yield return t3;
            yield return t4;
            yield return t5;
            yield return t6;
            yield return t7;
            yield return t8;
            yield return t9;
        }

        public static IEnumerable<T> Of<T>(T t1, T t2, T t3, T t4, T t5, T t6, T t7, T t8, T t9, T t10)
        {
            yield return t1;
            yield return t2;
            yield return t3;
            yield return t4;
            yield return t5;
            yield return t6;
            yield return t7;
            yield return t8;
            yield return t9;
            yield return t10;
        }

        public static T[] Array<T>(params T[] arguments)
        {
            return arguments;
        }

        public static T[] Array<T>(int count, T @default)
        {
            T[] array = new T[count];
            for (int i = 0; i < count; i++)
                array[i] = @default;
            return array;
        }

        public static IEnumerable<T> Allways<T>(T p)
        {
            while (true) { yield return p; }
        }

        public static IEnumerable<T> Empty<T>()
        {
            yield break;
        }

        public static IEnumerable<T> Generate<T>(int count, Func<T> generator)
        {
            while (count-- > 0)
                yield return generator();
        }

        public static IEnumerable<int> OfInts(int count)
        {
            for (int i = 0; i < count; i++)
                yield return i;
        }

        public static IEnumerable<int> OfInts(int from, int to)
        {
            if (from < to)
                for (int i = from; i <= to; i++) yield return i;
            else
                for (int i = from; i >= to; i--) yield return i;
        }

        public static IEnumerable<T> Concat<T>(T first, IEnumerable<T> seq)
        {
            yield return first;
            foreach (var item in seq)
                yield return item;
        }

        public static IEnumerable<T> Concat<T>(IEnumerable<T> seq, T last)
        {
            foreach (var item in seq)
                yield return item;

            yield return last;
        }

        public static IEnumerable<T> Concat<T>(IEnumerable<T> firsts, IEnumerable<T> lasts)
        {
            foreach (var first in firsts)
                yield return first;

            foreach (var last in lasts)
                yield return last;
        }

        public static IEnumerable<T> Concat<T>(IEnumerable<IEnumerable<T>> sequences)
        {
            foreach (var sequence in sequences)
                foreach (var item in sequence)
                    yield return item;
        }

        public static IEnumerable<TResult> Map<T1, T2, TResult>(IEnumerable<T1> seqOne, IEnumerable<T2> seqTwo, Func<T1, T2, TResult> function)
        {
            var enumOne = seqOne.GetEnumerator();
            var enumTwo = seqTwo.GetEnumerator();

            while (enumOne.MoveNext() && enumTwo.MoveNext())
                yield return function(enumOne.Current, enumTwo.Current);
        }

        public static void Do<T>(IEnumerable<T> seq, Action<T> action)
        {
            foreach (var item in seq)
                action(item);
        }

        public static void Do<TOne, TTwo>(IEnumerable<TOne> seqOne, IEnumerable<TTwo> seqTwo, Action<TOne, TTwo> action)
        {
            var enumOne = seqOne.GetEnumerator();
            var enumTwo = seqTwo.GetEnumerator();

            while (enumOne.MoveNext() && enumTwo.MoveNext())
                action(enumOne.Current, enumTwo.Current);
        }

        /// <summary>
        /// There is often a problem removing items from a collection since you mess up the iterator once one item is removed.
        /// This method solves this when working with a List with a predicate
        /// </summary>
        /// <typeparam name="T">the type of the sequence</typeparam>
        /// <param name="sequence">the original sequence</param>
        /// <param name="predicate">the method determining if the item should be removed or not</param>
        public static void Remove<T>(this List<T> sequence, Func<T, bool> predicate)
        {
            var itemsToRemove = sequence.Where(predicate);
            sequence.Remove(itemsToRemove);
        }

        /// <summary>
        /// There is often a problem removing items from a collection since you mess up the iterator once one item is removed.
        /// This method solves this when working with List
        /// </summary>
        /// <typeparam name="T">the type of the sequence</typeparam>
        /// <param name="sequence">the original sequence</param>
        /// <param name="itemsToRemove">the items in the original sequence to remove</param>
        public static void Remove<T>(this List<T> sequence, IEnumerable<T> itemsToRemove)
        {
            foreach (var item in itemsToRemove)
                sequence.Remove(item);
        }

        public static bool DoWhile<TOne, TTwo>(IEnumerable<TOne> seqOne, IEnumerable<TTwo> seqTwo, Func<TOne, TTwo, bool> predicateAction)
        {
            var enumOne = seqOne.GetEnumerator();
            var enumTwo = seqTwo.GetEnumerator();

            while (true)
            {
                var enumOneState = enumOne.MoveNext();
                var enumTwoState = enumTwo.MoveNext();

                if (enumOneState != enumTwoState)
                    return false;

                if (!enumOneState && !enumTwoState)
                    return true;

                if (!predicateAction(enumOne.Current, enumTwo.Current))
                    return false;
            }
        }

        public static bool DoUntil<TOne, TTwo>(IEnumerable<TOne> seqOne, IEnumerable<TTwo> seqTwo, Func<TOne, TTwo, bool> predicateAction)
        {
            var enumOne = seqOne.GetEnumerator();
            var enumTwo = seqTwo.GetEnumerator();

            while (true)
            {
                var oneHasItems = enumOne.MoveNext();
                var twoHasItems = enumTwo.MoveNext();

                if (oneHasItems != twoHasItems)
                    return false;

                if (!oneHasItems && !twoHasItems)
                    return true;

                if (predicateAction(enumOne.Current, enumTwo.Current))
                    return false;
            }
        }

        public static IEnumerable<T> Make<T>(int numberOfItems) where T : new()
        {
            for (int i = 0; i < numberOfItems; i++)
                yield return new T();
        }

        public static T Make<T>() where T : new()
        {
            return Make<T>(1).Single();
        }

        public static bool AreEqual<T>(IEnumerable<T> first, IEnumerable<T> second, Func<T, T, bool> comparer)
        {
            if (first == null || second == null)
                return false;

            var firstEnumerator = first.GetEnumerator();
            var secondEnumerator = second.GetEnumerator();
            bool firstHasMoreElements;
            bool secondHasMoreElements;

            while (true)
            {
                firstHasMoreElements = firstEnumerator.MoveNext();
                secondHasMoreElements = secondEnumerator.MoveNext();

                if (!(firstHasMoreElements && secondHasMoreElements))
                    break;

                if (comparer(firstEnumerator.Current, secondEnumerator.Current) == false)
                    return false;
            }

            return !firstHasMoreElements && !secondHasMoreElements;
        }

        public static int BinarySearch<T1, T2>(T1[] array, Func<T1, T2> accessor, T2 value, BinarySearchResult result)
            where T2 : IComparable
        {
            if (array.Length == 0)
                return -1;

            var stack = new Stack<KeyValuePair<int, T2>>();
            BuildBinarySearchStack(array, accessor, value, 0, array.Length, stack);

            Func<T2, T2, bool> comparer = null;

            switch (result)
            {
                case BinarySearchResult.EqualOrSmaller:
                    comparer = (item1, item2) => item1.CompareTo(item2) < 1;
                    break;
                case BinarySearchResult.Equals:
                    comparer = (item1, item2) => item1.CompareTo(item2) == 0;
                    break;
                case BinarySearchResult.EqualOrGreater:
                    comparer = (item1, item2) => item1.CompareTo(item2) > -1;
                    break;
            }

            var trimmedStack = stack.SkipWhile(x => comparer(x.Value, value) == false);

            if (trimmedStack.Any())
            {
                var pair = trimmedStack.First();
                return pair.Key;              // returns the index found
            }

            return -1;
        }

        public static int BinarySearch<T1, T2>(IList<T1> array, Func<T1, T2> accessor, T2 value, BinarySearchResult result)
            where T2 : IComparable
        {
            if (array.Count == 0)
                return -1;

            var stack = new Stack<KeyValuePair<int, T2>>();
            BuildBinarySearchStack(array, accessor, value, 0, array.Count, stack);

            Func<T2, T2, bool> comparer = null;

            switch (result)
            {
                case BinarySearchResult.EqualOrSmaller:
                    comparer = (item1, item2) => item1.CompareTo(item2) < 1;
                    break;
                case BinarySearchResult.Equals:
                    comparer = (item1, item2) => item1.CompareTo(item2) == 0;
                    break;
                case BinarySearchResult.EqualOrGreater:
                    comparer = (item1, item2) => item1.CompareTo(item2) > -1;
                    break;
            }

            var trimmedStack = stack.SkipWhile(x => comparer(x.Value, value) == false);

            if (trimmedStack.Any())
            {
                var pair = trimmedStack.First();
                return pair.Key;              // returns the index found
            }

            return -1;
        }

        #region private helpers

        private static void BuildBinarySearchStack<T1, T2>(T1[] array, Func<T1, T2> accessor, T2 searchKey, int startIndex, int length, Stack<KeyValuePair<int, T2>> searchStack)
            where T2 : IComparable
        {
            if (length == 0)
                return;

            int pos = startIndex + length / 2;

            var key = accessor(array[pos]);
            var searchNode = new KeyValuePair<int, T2>(pos, key);
            searchStack.Push(searchNode);

            if (searchKey.CompareTo(key) > 0)
                BuildBinarySearchStack(array, accessor, searchKey, pos + 1, length - pos - 1 + startIndex, searchStack);
            else if (searchKey.CompareTo(key) < 0)
                BuildBinarySearchStack(array, accessor, searchKey, startIndex, pos - startIndex, searchStack);
            else
                return;
        }

        private static void BuildBinarySearchStack<T1, T2>(IList<T1> list, Func<T1, T2> accessor, T2 searchKey, int startIndex, int length, Stack<KeyValuePair<int, T2>> searchStack)
            where T2 : IComparable
        {
            if (length == 0)
                return;

            int pos = startIndex + length / 2;

            var key = accessor(list[pos]);
            var searchNode = new KeyValuePair<int, T2>(pos, key);
            searchStack.Push(searchNode);

            if (searchKey.CompareTo(key) > 0)
                BuildBinarySearchStack(list, accessor, searchKey, pos + 1, length - pos - 1 + startIndex, searchStack);
            else if (searchKey.CompareTo(key) < 0)
                BuildBinarySearchStack(list, accessor, searchKey, startIndex, pos - startIndex, searchStack);
            else
                return;
        }

        #endregion
    }

    /// <summary>
    /// Represent what type match to return from a binary seach of a list or array.
    /// </summary>
    public enum BinarySearchResult
    {
        /// <summary>
        /// Return the element that is equal to the search key or if not found the biggest 
        /// existing value that is smaller that the search key.
        /// </summary>
        EqualOrSmaller,

        /// <summary>
        /// Return the element that is equal to the search key.
        /// </summary>
        Equals,

        /// <summary>
        /// Return the element that is equal to the search key or if not found the smallest 
        /// existing value that is bigger that the search key.
        /// </summary>
        EqualOrGreater
    }
}
