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

namespace Canyala.Lagoon.Extensions;

/// <summary>
/// 
/// </summary>
public static class StackExtensions
{
    public static bool IsEmpty<T>(this Stack<T> stack)
    { return stack.Count == 0; }

    /// <summary>
    /// Replaces the topmost item.
    /// </summary>
    /// <typeparam name="T">Type of item.</typeparam>
    /// <param name="stack">Stack instance.</param>
    /// <param name="item">New item.</param>
    /// <returns>Old item.</returns>
    public static T? PeekPoke<T>(this Stack<T?> stack, T? newItem)
    {
        T? oldItem = stack.Count > 0 ? stack.Pop() : default;
        stack.Push(newItem);
        return oldItem;
    }

    /// <summary>
    /// Replaces the topmost item.
    /// </summary>
    /// <typeparam name="T">Type of item.</typeparam>
    /// <param name="stack">Stack instance.</param>
    /// <param name="newItem">A new item.</param>
    /// <returns>New item</returns>
    public static T? Poke<T>(this Stack<T?> stack, T? newItem)
    {
        if (stack.Count > 0) stack.Pop();
        stack.Push(newItem);
        return newItem;
    }
}
