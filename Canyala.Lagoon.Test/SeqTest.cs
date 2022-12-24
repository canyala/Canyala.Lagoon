using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Canyala.Lagoon.Extensions;
using Canyala.Lagoon.Functional;

namespace Canyala.Lagoon.Test
{
    [TestClass]
    public class SeqTest
    {
        [TestMethod]
        public void SequencesShouldSupportFirstAndRest()
        {
            var reversed = ReverseList(1.UpTo(10));
        }

        private static IEnumerable<T> ReverseList<T>(IEnumerable<T> seq)
        {
            if (seq.Any())
                return Seq.Concat(ReverseList(seq.Rest()), seq.First());
            else
                return seq;
        }
    }
}
