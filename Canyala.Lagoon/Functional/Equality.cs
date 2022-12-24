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
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Canyala.Lagoon.Functional
{
    /// <summary>
    /// Static class to be used to generate a IEqualityComparer with custom behaviour
    /// for LINQ expressions.
    /// </summary>
    /// <remarks>
    /// Using the Equality class you can simplify the situation where you need to supply 
    /// an IEqualityComparer with a Linq expression.
    /// 
    /// var products = GetProducts().Distinct(Equality.With((x,y) => x.productId == y.productId));
    /// </remarks>
    public static class Equality
    {
        /// <summary>
        /// Returns a new instance of an object implementing IEqualityComparer(T) where
        /// the Equals() method evalutates the function given.
        /// </summary>
        /// <typeparam name="T">Type of objects to be compared.</typeparam>
        /// <param name="func">A function that performs the equality comparison.</param>
        /// <returns>A new instance of the GenericEqualityComparer class that uses
        /// the equality function for equality comparisons.</returns>
        public static IEqualityComparer<T> With<T>(Func<T, T, bool> func) { return new GenericEqualityComparer<T>(func); }

        /// <summary>
        /// Returns a new instance of an object implementing IEqualityComparer(T) where
        /// the Equals() method evalutates the function given and the GetHashcode() method uses
        /// the hashcode function given.
        /// </summary>
        /// <typeparam name="T">Type of objects to be compared.</typeparam>
        /// <param name="eqfunc">A function that performs the equality comparison.</param>
        /// <param name="hashfunc">A function that performs the hashcode calculation.</param>
        /// <returns>A new instance of the GenericEqualityComparer class that uses
        /// the equality and hashcode functions.</returns>
        public static IEqualityComparer<T> With<T>(Func<T, T, bool> eqfunc, Func<T, int> hashfunc)
        {
            return new GenericEqualityComparer<T>(eqfunc, hashfunc);
        }
    }
}
