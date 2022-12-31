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

using Canyala.Lagoon.Expressions;

namespace Canyala.Lagoon.Test.All;

[TestClass]
public class ExpressionsTest
{
    [TestMethod]
    public void InterpreterTest()
    {
        var symbols = new Symbols();
        symbols.Assign("?X", 10);

        var interpreted = new Interpreted("5 + 5 + ?X", symbols);
        Assert.AreEqual(20, interpreted.Evaluate());
    }

    [TestMethod]
    public void CompilerTest()
    {
        var symbols = new Symbols();
        symbols.Assign("?X", 10);

        var compiled = new Compiled("5 + 5 + ?X", symbols);
        Assert.AreEqual(20, compiled.Evaluate());
    }
}
