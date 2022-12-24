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
