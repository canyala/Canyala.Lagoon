/*
 
  MIT License

  Copyright (c) 2012-2022 Canyala Innovation

  Permission is hereby granted, free of charge, to any person obtaining a copy
  of this software and associated documentation files (the "Software"), to deal
  in the Software without restriction, including without limitation the rights
  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
  copies of the Software, and to permit persons to whom the Software is
  furnished to do so, subject to the following conditions:

  The above copyright notice and this permission notice shall be included in all
  copies or substantial portions of the Software.

  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
  SOFTWARE.

*/

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
