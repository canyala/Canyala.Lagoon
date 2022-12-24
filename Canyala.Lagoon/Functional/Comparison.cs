//
// Copyright (c) 2013 Canyala Innovation AB
//
// All rights reserved.
//

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Canyala.Lagoon.Functional
{
    /// <summary>
    /// Static class to be used to generate a IComparer with custom behaviour
    /// for LINQ expressions.
    /// </summary>
    /// <remarks>
    /// Using the Comparison class you can simplify the situation where you need to supply 
    /// an IComparer with a Linq expression.
    /// 
    /// var products = GetProducts().OrderBy(x => x.ProductId,  Comparison.With((x,y) => String.Compare(x.Name, y.Name)));
    /// </remarks>
    public static class Comparison
    {
        /// <summary>
        /// Returns a generic instance of the IComparer interface where
        /// the Compare() method calls the function given.
        /// </summary>
        /// <typeparam name="T">Type of objects to compare</typeparam>
        /// <param name="func">The compare function to use.</param>
        /// <returns>New instance of the GenericComparer class using the function given.</returns>
        public static IComparer<T> With<T>(Func<T, T, int> func)
        {
            return new GenericComparer<T>(func);
        }

        /// <summary>
        /// Returns a instance of the IComparer interface where
        /// the Compare() method calls the function given.
        /// </summary>
        /// <typeparam name="T">Type of objects to compare</typeparam>
        /// <param name="func">The compare function to use.</param>
        /// <returns>New instance of the GenericComparer class using the function given.</returns>
        public static IComparer WithIComparer<T>(Func<T, T, int> func)
        {
            return new GenericComparer<T>(func);
        }
    }
}
