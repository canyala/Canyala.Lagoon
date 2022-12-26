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


using Microsoft.VisualStudio.TestTools.UnitTesting;

using Canyala.Lagoon.Extensions;

namespace Canyala.Lagoon.Test;

/// <summary>
/// Summary description for StringExtensionTest
/// </summary>
[TestClass]
public class StringExtensionsTest
{
    [TestMethod]
    public void EncodeDecodeKeyRoundtrip()
    {
        Assert.AreEqual("http://www.canyala.se#44?=1", "http://www.canyala.se#44?=1".EncodeKey().DecodeKey());
    }

    [TestMethod]
    public void EncodeKey()
    {
        var encoded = "http://www.canyala.se#44?=1".EncodeKey();
        Assert.AreEqual("http://www.canyala.se#44?=1", encoded.DecodeKey());
    }

    [TestMethod]
    public void TestPercentEncodeNonAscii()
    {
        var expected = "%C3%96sten++%C3%A4r+blas%C3%A9";
        var actual = "Östen  är blasé".AsPercentEncoded();
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void TestPercentEncodeReserved()
    {
        var expected = "%21+%23+%24+%26+%27+%28+%29+%2A+%2B+%2C+%2F+%3A+%3B+%3D+%3F+%40+%5B+%5D";
        var actual = "! # $ & ' ( ) * + , / : ; = ? @ [ ]".AsPercentEncoded();
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void TestPercentDecodeNonAscii()
    {
        var expected = "Östen  är blasé";
        var actual = "%C3%96sten++%C3%A4r+blas%C3%A9".AsPercentDecoded();
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void TestPercentDecodeReserved()
    {
        var expected = "! # $ & ' ( ) * + , / : ; = ? @ [ ]";
        var actual = "%21+%23+%24+%26+%27+%28+%29+%2A+%2B+%2C+%2F+%3A+%3B+%3D+%3F+%40+%5B+%5D".AsPercentDecoded();
        Assert.AreEqual(expected, actual);
    }
}
