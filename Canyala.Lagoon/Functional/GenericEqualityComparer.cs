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

namespace Canyala.Lagoon.Functional;

/// <summary>
/// Generic equality comparer implementation which is returned by the 
/// Equality.With() static (<see cref="Equality"/>) method to be used with linq expressions where
/// you need to supply an IEqualityComparer.
/// </summary>
/// <typeparam name="T">Type of objects being compared.</typeparam>
internal class GenericEqualityComparer<T> : EqualityComparer<T?>
{
    internal GenericEqualityComparer(Func<T?, T?, bool> equalsFunc)
    {
        _Equals = equalsFunc;
    }

    internal GenericEqualityComparer(Func<T?, T?, bool> equalsFunc, Func<T?, int> hashcodeFunc)
    {
        _Equals = equalsFunc;
        _Hashcode = hashcodeFunc;
    }

    protected Func<T?, int> _Hashcode = x => x is not null ? x.GetHashCode() : default;
    protected Func<T?, T?, bool> _Equals = (x, y) => x is not null && x.Equals(y);

    public override bool Equals(T? x, T? y)
    {
        return _Equals(x, y);
    }

    public override int GetHashCode(T? obj)
    {
        return _Hashcode(obj);
    }
}
