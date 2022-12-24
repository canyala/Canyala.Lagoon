//
// Copyright (c) 2013 Canyala Innovation AB
//
// All rights reserved.
//

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Canyala.Lagoon.Extensions
{
    public static class StorageExtensions
    {
        public static Int64[] ReadInt64s(this BinaryReader reader, int count)
        {
            var array = new Int64[count];
            for (int i = 0; i < count; i++) array[i] = reader.ReadInt64();
            return array;
        }

        public static long Write(this BinaryWriter writer, Int64[] array)
        {
            foreach (var item in array) writer.Write(item);
            return writer.BaseStream.Position;
        }

        public static long Seek(this BinaryReader reader, long offset)
        { return reader.BaseStream.Seek(offset, SeekOrigin.Begin); }

        public static long Seek(this BinaryWriter writer, long offset)
        { return writer.BaseStream.Seek(offset, SeekOrigin.Begin); }

        /// <summary>
        /// Converts a string to a byte array using provided encoding.
        /// </summary>
        /// <param name="s">The string.</param>
        /// <param name="encoding">The encoding to use.</param>
        /// <param name="embeddedLength"><code>true</code> if length should be embedded in the byte array, <code>false</code> if length given by byte array length.</param>
        /// <returns></returns>
        static public byte[] AsBytes(this string s, Encoding encoding, bool embeddedLength = true)
        {
            if (embeddedLength)
            {
                var stream = new MemoryStream();
                var writer = new BinaryWriter(stream, encoding);
                writer.Write(s);

                return stream.ToArray();
            }

            return encoding.GetBytes(s);
        }

        /// <summary>
        /// Converts a string to a byte array using UTF8 encoding.
        /// </summary>
        /// <param name="s">The string.</param>
        /// <param name="embeddedLength"><code>true</code> if length should be embedded in the byte array, <code>false</code> if length given by byte array length.</param>
        /// <returns></returns>
        static public byte[] AsBytes(this string s, bool embeddedLength = true)
        { return s.AsBytes(Encoding.UTF8, embeddedLength); }

        /// <summary>
        /// Converts a byte array to string using provided encoding.
        /// </summary>
        /// <param name="bytes">The byte array</param>
        /// <param name="encoding">Encoding to use.</param>
        /// <param name="embeddedLength">Set to <code>true</code> if byte array includes length, <code>false</code> if length is given by array length.</param>
        /// <returns>A string.</returns>
        static public string AsString(this byte[] bytes, Encoding encoding, bool embeddedLength = true)
        {
            if (embeddedLength)
            {
                var stream = new MemoryStream(bytes);
                var reader = new BinaryReader(stream, encoding);
                return reader.ReadString();
            }

            return encoding.GetString(bytes, 0, bytes.Length);
        }

        /// <summary>
        /// Converts a byte array to string using UTF8 encoding.
        /// </summary>
        /// <param name="bytes">The byte array.</param>
        /// <param name="embeddedLength">Set to <code>true</code> if byte array includes length, <code>false</code> if length is given by array length.</param>
        /// <returns>A string.</returns>
        static public string AsString(this byte[] bytes, bool embeddedLength = true)
        { return bytes.AsString(Encoding.UTF8, embeddedLength); }

        static public byte[] AsBytes(this long value)
        { return BitConverter.GetBytes(value); }

        static public long AsLong(this byte[] bytes)
        { return BitConverter.ToInt64(bytes, 0); }

        static public byte[] AsBytes(this int value)
        { return BitConverter.GetBytes(value); }

        static public int AsInt(this byte[] bytes)
        { return BitConverter.ToInt32(bytes, 0); }
    }
}
