using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Canyala.Lagoon.Text;
using Canyala.Lagoon.Extensions;
using System.Globalization;

namespace Canyala.Lagoon.Test
{
    [TestClass]
    public class SubStringTest
    {
        [TestMethod]
        public void SplitShouldSkipBodiesAndStringConstantsTest()
        {
            var json = new SubString("{\"Person\":{\"a\":1,\"b\":2},\"Animal\":\"Tiger\"}");

            var body = json.Trim(1, -1);
            var bodyStr = body.ToString();

            var parts = body.Split(',').ToArray();
            var x = parts[0].ToString();
            var y = parts[1].ToString();

            var parts1 = parts[0].Split(':').ToArray();

            var parts11 = parts1[1].Trim(1, -1).Split(',').ToArray();

            var parts2 = parts[1].Split(':').ToArray();
        }

        [TestMethod]
        public void SplitsShouldBeAbleToRemoveEmptyEntries()
        {
            var example = "hello;;happy;world".AsSubString();
            var parts = example.Split(';');

            Assert.AreEqual("hello;happy;world", parts.Select(e => e.ToString()).Join(';'));
        }

        [TestMethod]
        public void TrimStartShouldWork()
        {
            var str = new SubString("   \tFoo ");

            Assert.AreEqual("Foo ", str.TrimStart().ToString());
        }
    }
}
