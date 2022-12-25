//-------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

using Canyala.Lagoon.Web;

namespace Canyala.Lagoon.Web.Extensions
{
    public static class HttpListenerRequestExtensions
    {
        public static string Subpath(this HttpListenerRequest request, int startIndex, int length)
        {
            var segments = request.Url.Segments;
            int start = startIndex + 1;

            var builder = new StringBuilder();

            while (length-- > 0 && segments.Length > start)
                builder.Append(segments[start++]);
            
            return builder.ToString().TrimEnd('/');
        }

        public static string Subpath(this HttpListenerRequest request, int startIndex)
        {
            var segments = request.Url.Segments;
            int start = startIndex + 1;

            var builder = new StringBuilder();

            while (segments.Length > start)
                builder.Append(segments[start++]);

            return builder.ToString().TrimEnd('/');    
        }

        public static string NegotiateType(this HttpListenerRequest request, params string[] allowedTypes)
        {
            var acceptTypes = request.AcceptTypes.Select(text => AcceptType.Parse(text)).OrderByDescending(type => type.Quality).ToList();

            foreach (var acceptType in acceptTypes)
                if (allowedTypes.Contains(acceptType.CompositeType))
                    return acceptType.CompositeType;

            return allowedTypes[0];
        }

        public static string DeduceType(this HttpListenerRequest request, string @default)
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

            return @default;
        }
    }
}
