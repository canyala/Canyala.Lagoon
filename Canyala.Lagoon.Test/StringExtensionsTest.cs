using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Canyala.Lagoon.Extensions;

namespace Canyala.Lagoon.Test
{
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
            Assert.AreEqual("http://www.canyala.se#44?=1", "http://www.canyala.se#44?=1".EncodeKey().DecodeKey());
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
}
