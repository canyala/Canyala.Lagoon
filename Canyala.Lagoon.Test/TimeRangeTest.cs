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

using Canyala.Lagoon.Functional;
using Canyala.Lagoon.Extensions;
using Canyala.Lagoon.Models;
      

namespace Canyala.Lagoon.Test;

[TestClass]
public class TimeRangeTest
{
    private DateTime[] fromTimes = new DateTime[] { 
            new DateTime(2000, 1, 1, 0, 0, 0),
            new DateTime(2000, 1, 1, 10, 0, 0),
            new DateTime(2000, 1, 31, 0, 0, 0),
            new DateTime(2001, 1, 1, 10, 15, 45)
         };

    private DateTime[] toTimes = new DateTime[] {
            new DateTime(2000, 1, 1, 0, 0, 0),
            new DateTime(2000, 1, 1, 10, 0, 0),
            new DateTime(2000, 1, 31, 0, 0, 0),
            new DateTime(2001, 1, 1, 10, 15, 45)
        };

    [TestMethod]
    public void TestEmpty1()
    {
        Assert.IsTrue(TimeRange.Empty.IsEmpty);
    }

    [TestMethod]
    public void TestEmpty2()
    {
        Assert.IsTrue(TimeRange.Create(new DateTime(1000), new DateTime(0)).IsEmpty);
    }

    [TestMethod]
    public void TestFrom()
    {
        Assert.AreEqual(TimeRange.Create(new DateTime(1000), new DateTime(2000)).From, new DateTime(1000));    
    }

    [TestMethod]
    public void TestTo()
    {
        Assert.AreEqual(TimeRange.Create(new DateTime(1000), new DateTime(2000)).To, new DateTime(2000));
    }

    [TestMethod]
    public void TestAsTimeSpan()
    {
        Assert.AreEqual(TimeRange.Create(fromTimes[0], toTimes[0]).AsTimeSpan, toTimes[0] - fromTimes[0]);
        Assert.AreEqual(TimeRange.Create(fromTimes[0], toTimes[2]).AsTimeSpan, toTimes[2] - fromTimes[0]);
        Assert.AreEqual(TimeRange.Create(fromTimes[2], toTimes[1]).AsTimeSpan, toTimes[1] - fromTimes[2]);
    }

    [TestMethod]
    public void TestUnion1()
    {
        Assert.AreEqual(TimeRange.Create(fromTimes[0], toTimes[2]).Union(TimeRange.Create(fromTimes[2], toTimes[3])), TimeRange.Create(fromTimes[0], toTimes[3]));
        Assert.AreEqual(TimeRange.Create(fromTimes[0], toTimes[0]).Union(TimeRange.Create(fromTimes[2], toTimes[3])), TimeRange.Create(fromTimes[0], toTimes[3]));
        Assert.AreEqual(TimeRange.Create(fromTimes[0], toTimes[1]).Union(TimeRange.Create(fromTimes[2], toTimes[3])), TimeRange.Create(fromTimes[0], toTimes[3]));
        Assert.AreEqual(TimeRange.Create(fromTimes[0], toTimes[1]).Union(TimeRange.Empty), TimeRange.Create(fromTimes[0], toTimes[1]));
        Assert.AreEqual(TimeRange.Empty.Union(TimeRange.Create(fromTimes[0], toTimes[1])), TimeRange.Create(fromTimes[0], toTimes[1]));
    }

    [TestMethod]
    public void TestUnion2()
    {
        List<TimeRange> input = new List<TimeRange> {
            TimeRange.Create(new DateTime(3000), new DateTime(5000)),
            TimeRange.Create(new DateTime(2000), new DateTime(7000)),
            TimeRange.Create(new DateTime(1000), new DateTime(3000)),
            TimeRange.Create(new DateTime(10000), new DateTime(20000)),
            TimeRange.Empty
        };

        Assert.AreEqual(TimeRange.Create(new DateTime(0), new DateTime(500)).Union(input.ToArray()), TimeRange.Create(new DateTime(0), new DateTime(20000)));
    }

    [TestMethod]
    public void TestIntersection1()
    {
        Assert.IsTrue(TimeRange.Create(fromTimes[0], toTimes[2]).Intersects(TimeRange.Create(fromTimes[0], toTimes[2])));
        Assert.IsTrue(TimeRange.Create(fromTimes[0], toTimes[2]).Intersects(TimeRange.Create(fromTimes[1], toTimes[3])));
        Assert.IsTrue(TimeRange.Create(fromTimes[0], toTimes[2]).Intersects(TimeRange.Create(fromTimes[2], toTimes[3])));
        Assert.IsFalse(TimeRange.Create(fromTimes[0], toTimes[1]).Intersects(TimeRange.Create(fromTimes[2], toTimes[3])));
        Assert.IsFalse(TimeRange.Create(fromTimes[0], toTimes[2]).Intersects(TimeRange.Empty));
        Assert.IsFalse(TimeRange.Empty.Intersects(TimeRange.Create(fromTimes[0], toTimes[2])));
    }

    [TestMethod]
    public void TestIntersection2()
    {
        var input1 = new [] {
            TimeRange.Create(new DateTime(3000), new DateTime(5000)),
            TimeRange.Create(new DateTime(2000), new DateTime(7000)),
            TimeRange.Create(new DateTime(1000), new DateTime(4000))
        };

        var input2 = Seq.Concat(TimeRange.Empty, input1).ToArray();

        Assert.AreEqual(TimeRange.Create(new DateTime(500), new DateTime(4000)).Intersection(input1), TimeRange.Create(new DateTime(3000), new DateTime(4000)));
        Assert.IsTrue(TimeRange.Create(new DateTime(500), new DateTime(4000)).Intersection(input2).IsEmpty);
        Assert.IsTrue(TimeRange.Create(new DateTime(0), new DateTime(500)).Intersection(input1).IsEmpty);
    }

    [TestMethod]
    public void TestIsSubrangeOf()
    {
        Assert.IsTrue(TimeRange.Create(new DateTime(1000), new DateTime(2000)).IsSubRangeOf(TimeRange.Create(new DateTime(0), new DateTime(3000))));
        Assert.IsTrue(TimeRange.Create(new DateTime(1000), new DateTime(2000)).IsSubRangeOf(TimeRange.Create(new DateTime(0), new DateTime(2000))));
        Assert.IsTrue(TimeRange.Create(new DateTime(1000), new DateTime(2000)).IsSubRangeOf(TimeRange.Create(new DateTime(1000), new DateTime(3000))));
        Assert.IsFalse(TimeRange.Create(new DateTime(1000), new DateTime(2000)).IsSubRangeOf(TimeRange.Create(new DateTime(1500), new DateTime(3000))));
        Assert.IsFalse(TimeRange.Create(new DateTime(1000), new DateTime(3000)).IsSubRangeOf(TimeRange.Create(new DateTime(2000), new DateTime(2500))));
        Assert.IsTrue(TimeRange.Empty.IsSubRangeOf(TimeRange.Create(new DateTime(1000), new DateTime(3000))));
        Assert.IsFalse(TimeRange.Create(new DateTime(1000), new DateTime(3000)).IsSubRangeOf(TimeRange.Empty));
        Assert.IsFalse(TimeRange.Empty.IsSubRangeOf(TimeRange.Empty));
    }

    [TestMethod]
    public void TestDifference()
    {
        var expected = new List<TimeRange> { TimeRange.Create(new DateTime(2000,1,1), new DateTime(2000,1,2)), TimeRange.Create(new DateTime(2000,1,5), new DateTime(2000,1,7)) };
        CollectionAssert.AreEqual(expected, TimeRange.Create(new DateTime(2000,1,1), new DateTime(2000,1,7)).Difference(TimeRange.Create(new DateTime(2000,1,2), new DateTime(2000,1,5))).ToList());

        expected = new List<TimeRange> { TimeRange.Create(new DateTime(2000, 1, 1), new DateTime(2000, 1, 2)) };
        CollectionAssert.AreEqual(expected, TimeRange.Create(new DateTime(2000, 1, 1), new DateTime(2000, 1, 7)).Difference(TimeRange.Create(new DateTime(2000, 1, 2), new DateTime(2000, 1, 7))).ToList());

        expected = new List<TimeRange> { TimeRange.Create(new DateTime(2000, 1, 5), new DateTime(2000, 1, 7)) };
        CollectionAssert.AreEqual(expected, TimeRange.Create(new DateTime(2000, 1, 1), new DateTime(2000, 1, 7)).Difference(TimeRange.Create(new DateTime(2000, 1, 1), new DateTime(2000, 1, 5))).ToList());

        CollectionAssert.AreEqual(Seq.Empty<TimeRange>().ToList(), TimeRange.Create(new DateTime(2000, 1, 1), new DateTime(2000, 1, 7)).Difference(TimeRange.Create(new DateTime(2000, 1, 1), new DateTime(2000, 1, 7))).ToList());

        CollectionAssert.AreEqual(Seq.Empty<TimeRange>().ToList(), TimeRange.Create(new DateTime(2000, 1, 1), new DateTime(2000, 1, 7)).Difference(TimeRange.Create(new DateTime(2000, 1, 9), new DateTime(2000, 1, 12))).ToList());
        CollectionAssert.AreEqual(Seq.Empty<TimeRange>().ToList(), TimeRange.Create(new DateTime(2000, 1, 1), new DateTime(2000, 1, 7)).Difference(TimeRange.Empty).ToList());
        CollectionAssert.AreEqual(Seq.Empty<TimeRange>().ToList(), TimeRange.Empty.Difference(TimeRange.Create(new DateTime(2000, 1, 9), new DateTime(2000, 1, 12))).ToList());
    }

    [TestMethod]
    public void TestReduceEmpty()
    {
        List<TimeRange> ranges = new List<TimeRange>();

        Assert.IsTrue(ranges.Reduce().Count() == 0);
    }

    [TestMethod]
    public void TestReduce1()
    {
        List<TimeRange> range = new List<TimeRange> { 
            TimeRange.Create(new DateTime(2000,1,1), new DateTime(2000,1,3)),
            TimeRange.Create(new DateTime(2000,1,2), new DateTime(2000,1,4)),
            TimeRange.Create(new DateTime(2000,1,5), new DateTime(2000,1,6)),
            TimeRange.Create(new DateTime(2000,1,6), new DateTime(2000,1,7))
        };

        List<TimeRange> expected = new List<TimeRange> {
            TimeRange.Create(new DateTime(2000,1,1), new DateTime(2000,1,4)),
            TimeRange.Create(new DateTime(2000,1,5), new DateTime(2000,1,7))
        };

        var actual = range.Reduce().ToList();

        CollectionAssert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void TestReduce2()
    {
        List<TimeRange> range = new List<TimeRange> { 
            TimeRange.Create(new DateTime(2000,1,1), new DateTime(2000,1,3)),
            TimeRange.Create(new DateTime(2000,1,2), new DateTime(2000,1,4)),
            TimeRange.Create(new DateTime(2000,1,5), new DateTime(2000,1,6)),
            TimeRange.Create(new DateTime(2000,1,6), new DateTime(2000,1,7)),
            TimeRange.Create(new DateTime(2000,1,4), new DateTime(2000,1,6)),
            TimeRange.Create(new DateTime(2000,1,2), new DateTime(2000,1,5))
        };

        List<TimeRange> expected = new List<TimeRange> {
            TimeRange.Create(new DateTime(2000,1,1), new DateTime(2000,1,7)),
        };

        var actual = range.Reduce().ToList();

        CollectionAssert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void TestComplement()
    {
        var range = TimeRange.Create(new DateTime(2000, 1, 3), new DateTime(2000, 1, 6));
        var input = new [] {
            TimeRange.Create(new DateTime(2000,1,2), new DateTime(2000,1,4)),
            TimeRange.Create(new DateTime(2000,1,5), new DateTime(2000,1,7)),
        };
        var expected = Seq.Array(TimeRange.Create(new DateTime(2000, 1, 4), new DateTime(2000, 1, 5)));
        CollectionAssert.AreEqual(expected, range.Complement(input).ToList());

        range = TimeRange.Create(new DateTime(2000, 1, 1), new DateTime(2000, 1, 10));
        expected = new[] {
            TimeRange.Create(new DateTime(2000,1,1), new DateTime(2000,1,2)),
            TimeRange.Create(new DateTime(2000,1,4), new DateTime(2000,1,5)),
            TimeRange.Create(new DateTime(2000,1,7), new DateTime(2000,1,10)),
        };
        CollectionAssert.AreEqual(expected, range.Complement(input).ToList());

        range = TimeRange.Create(new DateTime(2000, 1, 1), new DateTime(2000, 1, 31));
        input = new[] {
            TimeRange.Create(new DateTime(2000,1,2), new DateTime(2000,1,4)),
            TimeRange.Create(new DateTime(2000,1,5), new DateTime(2000,1,7)),
            TimeRange.Create(new DateTime(2000,1,3), new DateTime(2000,1,12)),
            TimeRange.Create(new DateTime(2000,1,17), new DateTime(2000,1,23)),
            TimeRange.Create(new DateTime(2000,1,15), new DateTime(2000,1,28)),
            TimeRange.Create(new DateTime(2000,1,29), new DateTime(2000,2,15)),
        };
        expected = new[] {
            TimeRange.Create(new DateTime(2000,1,1), new DateTime(2000,1,2)),
            TimeRange.Create(new DateTime(2000,1,12), new DateTime(2000,1,15)),
            TimeRange.Create(new DateTime(2000,1,28), new DateTime(2000,1,29)),
        };
        CollectionAssert.AreEqual(expected, range.Complement(input).ToList());
    }
    
}
