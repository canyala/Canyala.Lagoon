﻿//-------------------------------------------------------------------------------
//
//  MIT License
//
//  Copyright (c) 2012-2022 Canyala Innovation (Martin Fredriksson)
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

using System.Collections;

namespace Canyala.Lagoon.Core.Functional;

/// <summary>
/// Static class to be used to generate an IComparer with custom behaviour
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
    /// <typeparam name="TValue">Type of objects to compare</typeparam>
    /// <param name="func">The compare function to use.</param>
    /// <returns>New instance of the GenericComparer class using the function given.</returns>
    public static IComparer<TValue?> With<TValue>(Func<TValue?, TValue?, int> func)
    {
        return new GenericComparer<TValue>(func);
    }

    /// <summary>
    /// Returns a instance of the IComparer interface where
    /// the Compare() method calls the function given.
    /// </summary>
    /// <typeparam name="TValue">Type of objects to compare</typeparam>
    /// <param name="func">The compare function to use.</param>
    /// <returns>New instance of the GenericComparer class using the function given.</returns>
    public static IComparer WithIComparer<TValue>(Func<TValue?, TValue?, int> func)
    {
        return new GenericComparer<TValue>(func);
    }
}
