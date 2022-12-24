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

namespace Canyala.Lagoon.Extensions
{
    public static class ArrayExtensions
    {
        public static IEnumerable<T> Select<T>(this Array array, Predicate<T> predicate) where T : class
        {
            foreach (var @object in array)
                if (predicate(@object as T))
                    yield return @object as T;
        }

        public static void Poke(this byte[] buffer, int offset, byte[] data)
        { for (int i = 0; i < data.Length; i++) buffer[offset + i] = data[i]; }

        public static IEnumerable<T> Map<T>(this IEnumerable array, Func<object, T> map)
        { foreach (var item in array) yield return map(item); }

        public static string FromEncoding(this byte[] self, Encoding encoding)
        { return encoding.GetString(self, 0, self.Length); }

        public static string FromUTF8(this byte[] self)
        { return FromEncoding(self, Encoding.UTF8); }

        /// <summary>
        /// Selects indexed elements of an array.
        /// </summary>
        /// <typeparam name="T">The type of elements.</typeparam>
        /// <param name="array">The array.</param>
        /// <param name="args">Parameter array indicating which indices to select from array.</param>
        /// <returns>An array of T with the selected elements</returns>
        public static IEnumerable<T> Select<T>(this T[] array, params int[] args)
        { return args.Select(i => array[i]); }

        /// <summary>
        /// Allows observation of a sequence.
        /// </summary>
        /// <typeparam name="T">The type of the sequence.</typeparam>
        /// <param name="seq">The sequence.</param>
        /// <param name="observer">An action implementing the observation.</param>
        /// <returns></returns>
        public static IEnumerable<T> AsObserved<T>(this IEnumerable<T> seq, Action<T> observation)
        {
            foreach (var item in seq)
            {
                observation(item);
                yield return item;
            }
        }

        /// <summary>
        /// Allows monitoring of a sequence.
        /// </summary>
        /// <typeparam name="T">The type of the sequence.</typeparam>
        /// <param name="seq">The sequence.</param>
        /// <param name="observer">An predicate implementing the monitoration.</param>
        /// <returns></returns>
        public static IEnumerable<T> AsMonitored<T>(this IEnumerable<T> seq, Predicate<T> monitor)
        {
            foreach (var item in seq)
            {
                if (monitor(item))
                    yield return item;
                else
                    yield break;
            }
        }

        public static IEnumerable<T> Reverse<T>(this T[] seq)
        {
            for (int i = seq.Length - 1; i >= 0; i--)
                yield return seq[i];
        }
    }
}
