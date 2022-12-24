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
    public static class StreamExtensions
    {
        public static void Write(this Stream stream, IEnumerable<byte> bytes)
        {
            foreach (var value in bytes)
                stream.WriteByte(value);
        }

        public static IEnumerable<byte> AsBytes(this Stream stream, int bufferSize = 1024)
        {
            var bytesRead = 0;
            var buffer = new byte[bufferSize];
            while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
                for (int i = 0; i < bytesRead; i++)
                    yield return buffer[i];
        }
    }
}
