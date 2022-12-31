//-------------------------------------------------------------------------------
//
//  MIT License
//
//  Copyright (c) 2012-2022 Canyala Innovation (Martin Fredriksson)
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

using Canyala.Lagoon.Text;
using Canyala.Lagoon.Extensions;

namespace Canyala.Lagoon.Test;

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
