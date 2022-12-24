//
// Copyright (c) 2012 Canyala Innovation AB
//
// All rights reserved.
//

using System;

namespace Canyala.Lagoon.Web
{
    public class HttpException : Exception
    {
        public HttpStatus Status { get; private set; }

        public HttpException(int code, string description)
        {
            Status = new HttpStatus(code, description);
        }

        public HttpException(HttpStatus status)
        {
            Status = status;
        }
    }
}
