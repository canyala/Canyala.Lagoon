//-------------------------------------------------------------------------------
//
//  MIT License
//
//  Copyright (c) 2012-2022 Canyala Innovation
//
//  Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
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

using System.ComponentModel;

namespace System.Threading.Tasks;

internal class EAPCommon
{
    internal static void HandleCompletion<T>(
        TaskCompletionSource<T?> tcs, AsyncCompletedEventArgs e, Func<T?> getResult, Action unregisterHandler)
    {
        // Transfers the results from the AsyncCompletedEventArgs and getResult() to the
        // TaskCompletionSource, but only AsyncCompletedEventArg's UserState matches the TCS
        // (this check is important if the same WebClient is used for multiple, asynchronous
        // operations concurrently).  Also unregisters the handler to avoid a leak.
        if (tcs is not null && e.UserState == tcs)
        {
            if (e.Cancelled) _ = tcs.TrySetCanceled();
            else if (e.Error != null) _ = tcs.TrySetException(e.Error);
            else _ = tcs.TrySetResult(getResult());
            unregisterHandler();
        }
    }
}