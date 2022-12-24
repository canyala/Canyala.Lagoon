using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Canyala.Lagoon.Extensions;

namespace Canyala.Lagoon.Test
{
    [TestClass]
    public class StorageExtensionsTest
    {
        [TestMethod]
        public void BytesToStringRoundTrip()
        {
            var bytes = "Ayn Rand".AsBytes();
            var text = bytes.AsString();

            Assert.AreEqual("Ayn Rand", text);
        }
    }
}
