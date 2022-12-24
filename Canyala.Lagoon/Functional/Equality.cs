//
// Copyright (c) 2013 Canyala Innovation AB
//
// All rights reserved.
//

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
