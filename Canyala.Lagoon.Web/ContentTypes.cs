/*
 
  MIT License

  Copyright (c) 2022 Canyala Innovation

  Permission is hereby granted, free of charge, to any person obtaining a copy
  of this software and associated documentation files (the "Software"), to deal
  in the Software without restriction, including without limitation the rights
  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
  copies of the Software, and to permit persons to whom the Software is
  furnished to do so, subject to the following conditions:

  The above copyright notice and this permission notice shall be included in all
  copies or substantial portions of the Software.

  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
  SOFTWARE.

*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Canyala.Lagoon.Extensions;

namespace Canyala.Lagoon.Web
{
    public static class MediaTypes
    {
        public const string TextPlain = "text/plain";
        public const string TextHTML = "text/html";
        public const string FormUrlEncoded = "application/x-www-form-urlencoded";
        public const string Csv = "text/csv";
        public const string Tsv = "text/tsv";
        public const string Json = "application/json";
        public const string SparqlQuery = "application/sparql-query";
        public const string SparqlUpdate = "application/sparql-update";
        public const string Turtle = "text/turtle";
        public const string RdfXml = "application/rdf+xml";
        public const string N3 = "text/n3";
    }

    public static class FileExtensions
    {
        public const string TextHTML = "htm;html";
        public const string Csv = "csv";
        public const string Tsv = "tsv";
        public const string Json = "json";
        public const string Turtle = "ttl";
        public const string RdfXml = "rdf";
        public const string N3 = "n3";
    }

    public class AcceptType
    {
        public string Type { get; private set; }
        public string SubType { get; private set; }
        public double Quality { get; private set; }
        public int Level { get; private set; }

        public string CompositeType { get { return String.Concat(Type, '/', SubType); } }

        private AcceptType(string text)
        {
            var parts = text.Split(';');
            var typeParts = parts[0].Split('/');
            SubType = typeParts[1];
            Type = typeParts[0];

            Quality = 1.0;

            for (int i=1; i<parts.Length; i++)
            {
                var valueParts = parts[i].ToLower().Split('=');

                if (valueParts[0] == "q")
                {
                    Quality = double.Parse(valueParts[1]);
                    continue;
                }

                if (valueParts[0] == "level")
                {
                    Level = int.Parse(valueParts[1]);
                    continue;
                }
            }
        }

        public static AcceptType Parse(string text)
        {
            return new AcceptType(text);
        }
    }
}
