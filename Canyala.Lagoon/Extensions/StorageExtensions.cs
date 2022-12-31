//-------------------------------------------------------------------------------
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

using System.Text;

namespace Canyala.Lagoon.Extensions;

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
