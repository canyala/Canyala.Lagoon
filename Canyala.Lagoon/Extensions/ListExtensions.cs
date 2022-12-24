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
    public static class ListExtensions
    {
        /// <summary>
        /// Take an item from a list.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list">This list.</param>
        /// <param name="index">The index of the item to take.</param>
        /// <returns>The item as a 'T'.</returns>
        static public T TakeAt<T>(this List<T> list, int index)
        {
            T item = list[index];
            list.RemoveAt(index);
            return item;
        }
    }
}
