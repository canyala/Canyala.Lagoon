//
// Copyright (c) 2013 Canyala Innovation AB
//
// All rights reserved.
//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Canyala.Lagoon.Extensions
{
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
        public static T PeekPoke<T>(this Stack<T> stack, T newItem)
        {
            T oldItem = stack.Count > 0 ? stack.Pop() : default(T);
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
        public static T Poke<T>(this Stack<T> stack, T newItem)
        {
            if (stack.Count > 0) stack.Pop();
            stack.Push(newItem);
            return newItem;
        }
    }
}
