/*
 
  MIT License

  Copyright (c) 2022 Canyala Innovation

  Permission is hereby granted, free of charge, to any person obtaining a copy
  of this software and associated documentation files (the "Software"), to deal
  in the Software without restriction, including without limitation the rights
  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
  copies of the Software, and to permit persons to whom the Software is
  furnished to do so, subject to the following conditions:

  The above copyright notice and this permission notice shall be included in all
  copies or substantial portions of the Software.

  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
  SOFTWARE.

*/

using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Canyala.Lagoon.Functional;
using Canyala.Lagoon.Contracts;
using Canyala.Lagoon.Extensions;
      
namespace Canyala.Lagoon.Test
{
    [TestClass]
    public class BinarySearchTest
    {
        private static int[] testDataEven = new int[] { 1, 4, 7, 8, 9, 12, 14, 18, 20, 23, 25, 26, 27, 30 };
        private static int[] testDataOdd = new int[] { 1, 4, 7, 8, 9, 12, 14, 18, 20, 23, 25, 26, 27 };

        #region private helpers
        private int Find(int[] array, int value)
            { return Find(array.ToList(), value); }

        private int Find(IList<int> list, int value)
            { return list.IndexOf(value); }

        private int FindSmallerOrEqual(int[] array, int value)
        {
            for (int i = 0; i < array.Length; i++)
                if (array[i] > value)
                    return i == 0 ? -1 : i - 1;

            return array.Length - 1;
        }

        private int FindSmallerOrEqual(IList<int> array, int value)
        {
            for (int i = 0; i < array.Count; i++)
                if (array[i] > value)
                    return i == 0 ? -1 : i - 1;

            return array.Count - 1;
        }

        private int FindGreaterOrEqual(int[] array, int value)
        {
            for (int i = array.Length -1; i >= 0; i--)
                if (array[i] < value)
                    return i == array.Length - 1 ? -1 : i + 1;

            return 0;
        }

        private int FindGreaterOrEqual(IList<int> array, int value)
        {
            for (int i = array.Count - 1; i >= 0; i--)
                if (array[i] < value)
                    return i == array.Count - 1 ? -1 : i + 1;

            return 0;
        }

        #endregion

        [TestMethod]
        public void TestEmpty()
        {
            Assert.AreEqual(-1, Seq.BinarySearch(new int[0], x => x, 3, BinarySearchResult.Equals));
            Assert.AreEqual(-1, Seq.BinarySearch(new List<int>(), x => x, 3, BinarySearchResult.Equals));
        }

        [TestMethod]
        public void TestArrayEqualWithEvenNumberOfItems()
        {
            int[] toSearchFor = new int[] { 10, 8, 18, 4, 1, 27, 30, 0 };

            foreach (var item in toSearchFor)
            {
                int expected = Find(testDataEven, item);
                int actual = Seq.BinarySearch(testDataEven, x => x , item, BinarySearchResult.Equals);

                Assert.AreEqual(expected, actual);
            }
        }

        [TestMethod]
        public void TestArrayEqualWithOddNumberOfItems()
        {
            int[] toSearchFor = new int[] { 10, 8, 18, 4, 1, 27, 26, 0 };

            foreach (var item in toSearchFor)
            {
                int expected = Find(testDataOdd, item);
                int actual = Seq.BinarySearch(testDataEven, x => x, item, BinarySearchResult.Equals);

                Assert.AreEqual(expected, actual);
            }
        }

        [TestMethod]
        public void TestArrayEqualSmallerWithEvenNumberOfItems()
        {
            int[] toSearchFor = new int[] { 35, 10, 8, 18, 4, 1, 27, 30, 0 };

            foreach (var item in toSearchFor)
            {
                int expected = FindSmallerOrEqual(testDataEven, item);
                int actual = Seq.BinarySearch(testDataEven, x => x, item, BinarySearchResult.EqualOrSmaller);

                Assert.AreEqual(expected, actual);
            }
        }

        [TestMethod]
        public void TestArrayEqualSmallerWithOddNumberOfItems()
        {
            int[] toSearchFor = new int[] { 35, 10, 8, 18, 4, 1, 27, 0 };

            foreach (var item in toSearchFor)
            {
                int expected = FindSmallerOrEqual(testDataOdd, item);
                int actual = Seq.BinarySearch(testDataOdd, x => x, item, BinarySearchResult.EqualOrSmaller);

                Assert.AreEqual(expected, actual);
            }
        }

        [TestMethod]
        public void TestArrayEqualGreaterWithEvenNumberOfItems()
        {
            int[] toSearchFor = new int[] { 35, 10, 8, 18, 4, 1, 27, 30, 0 };

            foreach (var item in toSearchFor)
            {
                int expected = FindGreaterOrEqual(testDataEven, item);
                int actual = Seq.BinarySearch(testDataEven, x => x, item, BinarySearchResult.EqualOrGreater);

                Assert.AreEqual(expected, actual);
            }
        }

        [TestMethod]
        public void TestArrayEqualGreaterWithOddNumberOfItems()
        {
            int[] toSearchFor = new int[] { 35, 10, 8, 18, 4, 1, 27, 0 };

            foreach (var item in toSearchFor)
            {
                int expected = FindGreaterOrEqual(testDataOdd, item);
                int actual = Seq.BinarySearch(testDataOdd, x => x, item, BinarySearchResult.EqualOrGreater);

                Assert.AreEqual(expected, actual);
            }
        }

        [TestMethod]
        public void TestListEqualWithEvenNumberOfItems()
        {
            int[] toSearchFor = new int[] { 10, 8, 18, 4, 1, 27, 30, 0 };

            foreach (var item in toSearchFor)
            {
                int expected = Find(testDataEven.ToList(), item);
                int actual = Seq.BinarySearch(testDataEven.ToList(), x => x, item, BinarySearchResult.Equals);

                Assert.AreEqual(expected, actual);
            }
        }

        [TestMethod]
        public void TestListEqualWithOddNumberOfItems()
        {
            int[] toSearchFor = new int[] { 10, 8, 18, 4, 1, 27, 26, 0 };

            foreach (var item in toSearchFor)
            {
                int expected = Find(testDataOdd.ToList(), item);
                int actual = Seq.BinarySearch(testDataEven.ToList(), x => x, item, BinarySearchResult.Equals);

                Assert.AreEqual(expected, actual);
            }
        }

        [TestMethod]
        public void TestListEqualSmallerWithEvenNumberOfItems()
        {
            int[] toSearchFor = new int[] { 35, 10, 8, 18, 4, 1, 27, 30, 0 };

            foreach (var item in toSearchFor)
            {
                int expected = FindSmallerOrEqual(testDataEven.ToList(), item);
                int actual = Seq.BinarySearch(testDataEven.ToList(), x => x, item, BinarySearchResult.EqualOrSmaller);

                Assert.AreEqual(expected, actual);
            }
        }

        [TestMethod]
        public void TestListEqualSmallerWithOddNumberOfItems()
        {
            int[] toSearchFor = new int[] { 35, 10, 8, 18, 4, 1, 27, 0 };

            foreach (var item in toSearchFor)
            {
                int expected = FindSmallerOrEqual(testDataOdd.ToList(), item);
                int actual = Seq.BinarySearch(testDataOdd.ToList(), x => x, item, BinarySearchResult.EqualOrSmaller);

                Assert.AreEqual(expected, actual);
            }
        }

        [TestMethod]
        public void TestListEqualGreaterWithEvenNumberOfItems()
        {
            int[] toSearchFor = new int[] { 35, 10, 8, 18, 4, 1, 27, 30, 0 };

            foreach (var item in toSearchFor)
            {
                int expected = FindGreaterOrEqual(testDataEven.ToList(), item);
                int actual = Seq.BinarySearch(testDataEven.ToList(), x => x, item, BinarySearchResult.EqualOrGreater);

                Assert.AreEqual(expected, actual);
            }
        }

        [TestMethod]
        public void TestListEqualGreaterWithOddNumberOfItems()
        {
            int[] toSearchFor = new int[] { 35, 10, 8, 18, 4, 1, 27, 0 };

            foreach (var item in toSearchFor)
            {
                int expected = FindGreaterOrEqual(testDataOdd.ToList(), item);
                int actual = Seq.BinarySearch(testDataOdd.ToList(), x => x, item, BinarySearchResult.EqualOrGreater);

                Assert.AreEqual(expected, actual);
            }
        }
    }
}
