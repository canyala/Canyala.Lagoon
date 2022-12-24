using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Canyala.Lagoon.Expressions;

namespace Canyala.Lagoon.Test
{
    [TestClass]
    public class ExpressionsTest
    {
        [TestMethod]
        public void InterpreterTest()
        {
            var symbols = new Symbols();
            symbols.Assign("?X", 10);

            var interpreter = new Interpreter("5 + 5 + ?X", symbols);
            Assert.AreEqual(20, interpreter.Value);
        }

        [TestMethod]
        public void CompilerTest()
        {
            var symbols = new Symbols();
            symbols.Assign("?X", 10);

            var compiler = new Compiler("5 + 5 + ?X", symbols);
            Assert.AreEqual(20, compiler.Value);
        }
    }
}
