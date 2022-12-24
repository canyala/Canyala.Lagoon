using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Canyala.Lagoon.Extensions;
using Canyala.Lagoon.Text;

namespace Canyala.Lagoon.Test
{
    [TestClass]
    public class AnalyzerTest
    {
        [TestMethod]
        public void PermutationsTest()
            { Assert.AreEqual(Analyzer.Permutations("bad").Join(';'), "bad;ba;ad;b;a;d"); }

        [TestMethod]
        public void WordsTest()
            { Assert.AreEqual(Analyzer.Words("Hello, world! Again!").Join(';'), "Hello;world;Again"); }

        [TestMethod]
        public void SentencesTest()
            { Assert.AreEqual(Analyzer.Sentences("I am cool. You are too.").Join(';'), "I am cool.;You are too."); }

        [TestMethod]
        public void LinesTest()
            { Assert.AreEqual(4, Analyzer.Lines("Hello\r\nWorld\r\nThis\r\nis").Count()); }
    }
}
