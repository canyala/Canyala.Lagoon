/*
 
  MIT License

  Copyright (c) 2022 Canyala Innovation

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
using Canyala.Lagoon.Serialization;
using Canyala.Lagoon.Text;
using Canyala.Lagoon.Functional;

namespace Canyala.Lagoon.Test
{
    /// <summary>
    /// Summary description for PoshTest
    /// </summary>
    [TestClass]
    public class PoshTest
    {
        [TestMethod]
        public void Heading1FormattingShouldWork()
        {
            Assert
                .AreEqual(
                    "<h1>Hello</h1>",
                    Posh.New.Heading1("Hello").ToString()
                );
        }

        [TestMethod]
        public void UnorderedListFormattingShouldWork()
        {
            Assert
                .AreEqual(
                    "<ul><li>Hello</li><li>I</li><li>am</li><li>cool</li></ul>",
                    Posh.New.UnorderedList("Hello", "I", "am", "cool").ToString()
                );
        }

        [TestMethod]
        public void OrderedListFormattingShouldWork()
        {
            Assert
                .AreEqual(
                    "<ol><li>Hello</li><li>I</li><li>am</li><li>cool</li></ol>",
                    Posh.New.OrderedList("Hello", "I", "am", "cool").ToString()
                );
        }

        [TestMethod]
        public void DefinitionListFormattingShouldWork()
        {
            Assert
                .AreEqual(
                    "<dl><dt>Greeting</dt><dv>Hello</dv><dt>Entity</dt><dv>World</dv></dl>",
                    Posh.New.DefinitionList(Tuple.Create("Greeting", "Hello"), Tuple.Create("Entity", "World")).ToString()
                );
        }
    }
}
