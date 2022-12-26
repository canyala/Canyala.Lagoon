﻿//-------------------------------------------------------------------------------
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

using System.ComponentModel;

namespace System.Net;

/// <summary>Extension methods for working with WebClient asynchronously.</summary>
public static class WebClientExtensions
{
    /// <summary>Downloads the resource with the specified URI as a byte array, asynchronously.</summary>
    /// <param name="webClient">The WebClient.</param>
    /// <param name="address">The URI from which to download data.</param>
    /// <returns>A Task that contains the downloaded data.</returns>
    public static Task<byte[]?> DownloadDataTask(this WebClient webClient, string address)
    {
        return DownloadDataTask(webClient, new Uri(address));
    }

    /// <summary>Downloads the resource with the specified URI as a byte array, asynchronously.</summary>
    /// <param name="webClient">The WebClient.</param>
    /// <param name="address">The URI from which to download data.</param>
    /// <returns>A Task that contains the downloaded data.</returns>
    public static Task<byte[]?> DownloadDataTask(this WebClient webClient, Uri address)
    {
        // Create the task to be returned
        var tcs = new TaskCompletionSource<byte[]?>(address);

        // Setup the callback event handler
        void handler(object sender, DownloadDataCompletedEventArgs e)
        {
            EAPCommon.HandleCompletion(tcs, e, () => e.Result, () => webClient.DownloadDataCompleted -= handler);
        }

        webClient.DownloadDataCompleted += handler;

        // Start the async work
        try
        {
            webClient.DownloadDataAsync(address, tcs);
        }
        catch (Exception exc)
        {
            // If something goes wrong kicking off the async work,
            // unregister the callback and cancel the created task
            webClient.DownloadDataCompleted -= handler;
            _ = tcs.TrySetException(exc);
        }

        // Return the task that represents the async operation
        return tcs.Task;
    }

    /// <summary>Downloads the resource with the specified URI as a string, asynchronously.</summary>
    /// <param name="webClient">The WebClient.</param>
    /// <param name="address">The URI from which to download data.</param>
    /// <returns>A Task that contains the downloaded string.</returns>
    public static Task<string?> DownloadStringTask(this WebClient webClient, Uri address)
    {
        // Create the task to be returned
        var tcs = new TaskCompletionSource<string?>(address);

        // Setup the callback event handler
        void handler(object sender, DownloadStringCompletedEventArgs e)
        {
            EAPCommon.HandleCompletion(tcs, e, () => e.Result, () => webClient.DownloadStringCompleted -= handler);
        }

        webClient.DownloadStringCompleted += handler;

        // Start the async work
        try
        {
            webClient.DownloadStringAsync(address, tcs);
        }
        catch (Exception exc)
        {
            // If something goes wrong kicking off the async work,
            // unregister the callback and cancel the created task
            webClient.DownloadStringCompleted -= handler;
            _ = tcs.TrySetException(exc);
        }

        // Return the task that represents the async operation
        return tcs.Task;
    }

    /// <summary>Downloads the resource with the specified URI to a local file, asynchronously.</summary>
    /// <param name="webClient">The WebClient.</param>
    /// <param name="address">The URI from which to download data.</param>
    /// <param name="fileName">The name of the local file that is to receive the data.</param>
    /// <returns>A Task that contains the downloaded data.</returns>
    public static Task DownloadFileTask(this WebClient webClient, string address, string fileName)
    {
        return DownloadFileTask(webClient, new Uri(address), fileName);
    }

    /// <summary>Downloads the resource with the specified URI to a local file, asynchronously.</summary>
    /// <param name="webClient">The WebClient.</param>
    /// <param name="address">The URI from which to download data.</param>
    /// <param name="fileName">The name of the local file that is to receive the data.</param>
    /// <returns>A Task that contains the downloaded data.</returns>
    public static Task DownloadFileTask(this WebClient webClient, Uri address, string fileName)
    {
        // Create the task to be returned
        var tcs = new TaskCompletionSource<object?>(address);

        // Setup the callback event handler
        void handler(object? sender, AsyncCompletedEventArgs e)
        {
            EAPCommon.HandleCompletion(tcs, e, () => null, () => webClient.DownloadFileCompleted -= handler);
        }

        webClient.DownloadFileCompleted += handler;

        // Start the async work
        try
        {
            webClient.DownloadFileAsync(address, fileName, tcs);
        }
        catch (Exception exc)
        {
            // If something goes wrong kicking off the async work,
            // unregister the callback and cancel the created task
            webClient.DownloadFileCompleted -= handler;
            _ = tcs.TrySetException(exc);
        }

        // Return the task that represents the async operation
        return tcs.Task;
    }

    /// <summary>Downloads the resource with the specified URI as a string, asynchronously.</summary>
    /// <param name="webClient">The WebClient.</param>
    /// <param name="address">The URI from which to download data.</param>
    /// <returns>A Task that contains the downloaded string.</returns>
    public static Task<string?> DownloadStringTask(this WebClient webClient, string address)
    {
        return DownloadStringTask(webClient, new Uri(address));
    }

    /// <summary>Opens a readable stream for the data downloaded from a resource, asynchronously.</summary>
    /// <param name="webClient">The WebClient.</param>
    /// <param name="address">The URI for which the stream should be opened.</param>
    /// <returns>A Task that contains the opened stream.</returns>
    public static Task<Stream?> OpenReadTask(this WebClient webClient, string address)
    {
        return OpenReadTask(webClient, new Uri(address));
    }

    /// <summary>Opens a readable stream for the data downloaded from a resource, asynchronously.</summary>
    /// <param name="webClient">The WebClient.</param>
    /// <param name="address">The URI for which the stream should be opened.</param>
    /// <returns>A Task that contains the opened stream.</returns>
    public static Task<Stream?> OpenReadTask(this WebClient webClient, Uri address)
    {
        // Create the task to be returned
        TaskCompletionSource<Stream?> tcs = new(address);

        // Setup the callback event handler
        void handler(object sender, OpenReadCompletedEventArgs e)
        {
            EAPCommon.HandleCompletion(tcs, e, () => e.Result, () => webClient.OpenReadCompleted -= handler);
        }

        webClient.OpenReadCompleted += handler;

        // Start the async work
        try
        {
            webClient.OpenReadAsync(address, tcs);
        }
        catch (Exception exc)
        {
            // If something goes wrong kicking off the async work,
            // unregister the callback and cancel the created task
            webClient.OpenReadCompleted -= handler;
            _ = tcs.TrySetException(exc);
        }

        // Return the task that represents the async operation
        return tcs.Task;
    }

    /// <summary>Opens a writeable stream for uploading data to a resource, asynchronously.</summary>
    /// <param name="webClient">The WebClient.</param>
    /// <param name="address">The URI for which the stream should be opened.</param>
    /// <param name="method">The HTTP method that should be used to open the stream.</param>
    /// <returns>A Task that contains the opened stream.</returns>
    public static Task<Stream?> OpenWriteTask(this WebClient webClient, string address, string method)
    {
        return OpenWriteTask(webClient, new Uri(address), method);
    }

    /// <summary>Opens a writeable stream for uploading data to a resource, asynchronously.</summary>
    /// <param name="webClient">The WebClient.</param>
    /// <param name="address">The URI for which the stream should be opened.</param>
    /// <param name="method">The HTTP method that should be used to open the stream.</param>
    /// <returns>A Task that contains the opened stream.</returns>
    public static Task<Stream?> OpenWriteTask(this WebClient webClient, Uri address, string method)
    {
        // Create the task to be returned
        var tcs = new TaskCompletionSource<Stream?>(address);

        // Setup the callback event handler
        void handler(object sender, OpenWriteCompletedEventArgs e)
        {
            EAPCommon.HandleCompletion(tcs, e, () => e.Result, () => webClient.OpenWriteCompleted -= handler);
        }

        webClient.OpenWriteCompleted += handler;

        // Start the async work
        try
        {
            webClient.OpenWriteAsync(address, method, tcs);
        }
        catch (Exception exc)
        {
            // If something goes wrong kicking off the async work,
            // unregister the callback and cancel the created task
            webClient.OpenWriteCompleted -= handler;
            _ = tcs.TrySetException(exc);
        }

        // Return the task that represents the async operation
        return tcs.Task;
    }

    /// <summary>Uploads data to the specified resource, asynchronously.</summary>
    /// <param name="webClient">The WebClient.</param>
    /// <param name="address">The URI to which the data should be uploaded.</param>
    /// <param name="method">The HTTP method that should be used to upload the data.</param>
    /// <param name="data">The data to upload.</param>
    /// <returns>A Task containing the data in the response from the upload.</returns>
    public static Task<byte[]?> UploadDataTask(this WebClient webClient, string address, string method, byte[] data)
    {
        return UploadDataTask(webClient, new Uri(address), method, data);
    }

    /// <summary>Uploads data to the specified resource, asynchronously.</summary>
    /// <param name="webClient">The WebClient.</param>
    /// <param name="address">The URI to which the data should be uploaded.</param>
    /// <param name="method">The HTTP method that should be used to upload the data.</param>
    /// <param name="data">The data to upload.</param>
    /// <returns>A Task containing the data in the response from the upload.</returns>
    public static Task<byte[]?> UploadDataTask(this WebClient webClient, Uri address, string method, byte[] data)
    {
        // Create the task to be returned
        TaskCompletionSource<byte[]?> tcs = new(address);

        // Setup the callback event handler
        void handler(object sender, UploadDataCompletedEventArgs e)
        {
            EAPCommon.HandleCompletion(tcs, e, () => e.Result, () => webClient.UploadDataCompleted -= handler);
        }

        webClient.UploadDataCompleted += handler;

        // Start the async work
        try
        {
            webClient.UploadDataAsync(address, method, data, tcs);
        }
        catch (Exception exc)
        {
            // If something goes wrong kicking off the async work,
            // unregister the callback and cancel the created task
            webClient.UploadDataCompleted -= handler;
            _ = tcs.TrySetException(exc);
        }

        // Return the task that represents the async operation
        return tcs.Task;
    }

    /// <summary>Uploads a file to the specified resource, asynchronously.</summary>
    /// <param name="webClient">The WebClient.</param>
    /// <param name="address">The URI to which the file should be uploaded.</param>
    /// <param name="method">The HTTP method that should be used to upload the file.</param>
    /// <param name="fileName">A path to the file to upload.</param>
    /// <returns>A Task containing the data in the response from the upload.</returns>
    public static Task<byte[]?> UploadFileTask(this WebClient webClient, string address, string method, string fileName)
    {
        return UploadFileTask(webClient, new Uri(address), method, fileName);
    }

    /// <summary>Uploads a file to the specified resource, asynchronously.</summary>
    /// <param name="webClient">The WebClient.</param>
    /// <param name="address">The URI to which the file should be uploaded.</param>
    /// <param name="method">The HTTP method that should be used to upload the file.</param>
    /// <param name="fileName">A path to the file to upload.</param>
    /// <returns>A Task containing the data in the response from the upload.</returns>
    public static Task<byte[]?> UploadFileTask(this WebClient webClient, Uri address, string method, string fileName)
    {
        // Create the task to be returned
        TaskCompletionSource<byte[]?> tcs = new(address);

        // Setup the callback event handler
        void handler(object sender, UploadFileCompletedEventArgs e)
        {
            EAPCommon.HandleCompletion(tcs, e, () => e.Result, () => webClient.UploadFileCompleted -= handler);
        }

        webClient.UploadFileCompleted += handler;

        // Start the async work
        try
        {
            webClient.UploadFileAsync(address, method, fileName, tcs);
        }
        catch (Exception exc)
        {
            // If something goes wrong kicking off the async work,
            // unregister the callback and cancel the created task
            webClient.UploadFileCompleted -= handler;
            _ = tcs.TrySetException(exc);
        }

        // Return the task that represents the async operation
        return tcs.Task;
    }

    /// <summary>Uploads data in a string to the specified resource, asynchronously.</summary>

    /// <param name="webClient">The WebClient.</param>
    /// <param name="address">The URI to which the data should be uploaded.</param>
    /// <param name="method">The HTTP method that should be used to upload the data.</param>
    /// <param name="data">The data to upload.</param>
    /// <returns>A Task containing the data in the response from the upload.</returns>
    public static Task<string?> UploadStringTask(this WebClient webClient, string address, string method, string data)
    {
        return UploadStringTask(webClient, new Uri(address), method, data);
    }

    /// <summary>Uploads data in a string to the specified resource, asynchronously.</summary>
    /// <param name="webClient">The WebClient.</param>
    /// <param name="address">The URI to which the data should be uploaded.</param>
    /// <param name="method">The HTTP method that should be used to upload the data.</param>
    /// <param name="data">The data to upload.</param>
    /// <returns>A Task containing the data in the response from the upload.</returns>
    public static Task<string?> UploadStringTask(this WebClient webClient, Uri address, string method, string data)
    {
        // Create the task to be returned
        TaskCompletionSource<string?> tcs = new(address);

        // Setup the callback event handler
        void handler(object sender, UploadStringCompletedEventArgs e)
        {
            EAPCommon.HandleCompletion(tcs, e, () => e.Result, () => webClient.UploadStringCompleted -= handler);
        }

        webClient.UploadStringCompleted += handler;

        // Start the async work
        try
        {
            webClient.UploadStringAsync(address, method, data, tcs);
        }
        catch (Exception exc)
        {
            // If something goes wrong kicking off the async work,
            // unregister the callback and cancel the created task
            webClient.UploadStringCompleted -= handler;
            _ = tcs.TrySetException(exc);
        }

        // Return the task that represents the async operation
        return tcs.Task;
    }
}