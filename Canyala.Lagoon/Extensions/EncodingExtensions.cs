//
// Copyright (c) 2013 Canyala Innovation AB
//
// All rights reserved.
//

using Canyala.Lagoon.Functional;
using System.Collections.Generic;
using System.Text;

namespace Canyala.Lagoon.Extensions
{
    public static class EncodingExtensions
    {
        static public IEnumerable<byte> AsBytes(this IEnumerable<char> characters, Encoding encoding)
        {
            foreach (var c in characters)
                foreach (var b in encoding.GetBytes(Seq.Array(c)))
                    yield return b;
        }

        static public IEnumerable<char> AsChars(this IEnumerable<byte> bytes, Encoding encoding)
        {
            foreach (var b in bytes)
                foreach (var c in encoding.GetChars(Seq.Array(b)))
                    yield return c;
        }
    }
}
