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

using System.Net;
using System.Text;

namespace Canyala.Lagoon.Web.Extensions;

public static class HttpListenerRequestExtensions
{
    public static string Subpath(this HttpListenerRequest request, int startIndex, int length)
    {
        var segments = request.Url?.Segments;
        int start = startIndex + 1;

        var builder = new StringBuilder();

        while (length-- > 0 && segments?.Length > start)
            builder.Append(segments[start++]);
        
        return builder.ToString().TrimEnd('/');
    }

    public static string Subpath(this HttpListenerRequest request, int startIndex)
    {
        var segments = request.Url?.Segments;
        int start = startIndex + 1;

        var builder = new StringBuilder();

        while (segments?.Length > start)
            builder.Append(segments[start++]);

        return builder.ToString().TrimEnd('/');    
    }

    public static string NegotiateType(this HttpListenerRequest request, params string[] allowedTypes)
    {
        if (request.AcceptTypes is not null)
        {
            var acceptTypes = request.AcceptTypes.Select(text => AcceptType.Parse(text)).OrderByDescending(type => type.Quality).ToList();

            foreach (var acceptType in acceptTypes)
                if (allowedTypes.Contains(acceptType.CompositeType))
                    return acceptType.CompositeType;
        }

        return allowedTypes[0];
    }

    public static string DeduceType(this HttpListenerRequest request, string @default)
    {
        if (request.Url is not null)
        {
            if (request.Url.AbsolutePath.EndsWith(".html"))
                return MediaTypes.TextHTML;

            if (request.Url.AbsolutePath.EndsWith(".htm"))
                return MediaTypes.TextHTML;

            if (request.Url.AbsolutePath.EndsWith(".csv"))
                return MediaTypes.Csv;

            if (request.Url.AbsolutePath.EndsWith(".tsv"))
                return MediaTypes.Tsv;

            if (request.Url.AbsolutePath.EndsWith(".rdf"))
                return MediaTypes.RdfXml;

            if (request.Url.AbsolutePath.EndsWith(".ttl"))
                return MediaTypes.Turtle;

            if (request.Url.AbsolutePath.EndsWith(".n3"))
                return MediaTypes.N3;
        }

        return @default;
    }
}
