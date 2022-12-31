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

using Canyala.Lagoon.Extensions;

namespace Canyala.Lagoon.Web;

/// <summary>
/// Provides a representation of HTTP status codes.
/// </summary>
public class HttpStatus
{
    /// <summary>
    /// Numeric code.
    /// </summary>
    public int Code { get; private set; }

    /// <summary>
    /// Textual description.
    /// </summary>
    public string Description { get; private set; }

    /// <summary>
    /// Creates a HTTP status code
    /// </summary>
    /// <param name="code">Numeric code.</param>
    /// <param name="description">Textual description.</param>
    public HttpStatus(int code, string description)
    {
        Code = code;
        Description = description;
    }

    /// <summary>
    /// Throws this status code as an HttpException.
    /// </summary>
    public void Throw()
    {
        throw new HttpException(this);
    }

    /// <summary>
    /// Converts a status to a string.
    /// </summary>
    /// <returns>A string representing the status.</returns>
    public override string ToString()
    {
        return "{0} {1}".Args(Code, Description);
    }

    /// <summary>
    /// This class of status codes indicates the action requested by the client was received, understood, accepted and processed successfully.
    /// </summary>
    public static class Success
    {
        /// <summary>
        /// Standard response for successful HTTP requests.
        /// The actual response will depend on the request method used. In a GET request, the response will contain an entity corresponding to the requested resource. 
        /// In a POST request the response will contain an entity describing or containing the result of the action.
        /// </summary>
        public static readonly HttpStatus OK = new(200, "OK");

        /// <summary>
        /// The request has been fulfilled and resulted in a new resource being created.
        /// </summary>
        public static readonly HttpStatus Created = new(201, "Created");

        /// <summary>
        /// The request has been accepted for processing, but the processing has not been completed. 
        /// The request might or might not eventually be acted upon, as it might be disallowed when processing actually takes place.
        /// </summary>
        public static readonly HttpStatus Accepted = new(202, "Accepted");

        /// <summary>
        /// The server successfully processed the request, but is returning information that may be from another source.
        /// </summary>
        public static readonly HttpStatus NonAuthoritativeInformation = new(203, "Non-Authoritative Information ");

        /// <summary>
        /// The server successfully processed the request, but is not returning any content.
        /// </summary>
        public static readonly HttpStatus NoContent = new(204, "No Content");

        /// <summary>
        /// The server successfully processed the request, but is not returning any content. 
        /// Unlike a 204 response, this response requires that the requester reset the document view.
        /// </summary>
        public static readonly HttpStatus ResetContent = new(205, "Reset Content");

        /// <summary>
        /// The server is delivering only part of the resource due to a range header sent by the client. 
        /// The range header is used by tools like wget to enable resuming of interrupted downloads, or split a download into multiple simultaneous streams
        /// </summary>
        public static readonly HttpStatus PartialContent = new(206, " Partial Content");
    }

    /// <summary>
    /// The client must take additional action to complete the request.
    /// </summary>
    public static class Redirection
    {
        /// <summary>
        /// Indicates multiple options for the resource that the client may follow. 
        /// It, for instance, could be used to present different format options for video, list files with different extensions, 
        /// or word sense disambiguation.
        /// </summary>
        public static readonly HttpStatus MultipleChoices = new(300, "Multiple Choices");

        /// <summary>
        /// Indicates that the resource has not been modified since the version specified by the request headers If-Modified-Since or If-Match.
        /// This means that there is no need to retransmit the resource, since the client still has a previously-downloaded copy.
        /// </summary>
        public static readonly HttpStatus NotModified = new(304, "Not Modified");
    }

    /// <summary>
    /// The 4xx class of status code is intended for cases in which the client seems to have erred. 
    /// Except when responding to a HEAD request, the server should include an entity containing an explanation of the error situation, 
    /// and whether it is a temporary or permanent condition. These status codes are applicable to any request method. 
    /// User agents should display any included entity to the user.
    /// </summary>
    public static class ClientError
    {
        /// <summary>
        /// The request cannot be fulfilled due to bad syntax.[
        /// </summary>
        public static readonly HttpStatus BadRequest = new(400, "Bad Request");

        /// <summary>
        /// Similar to 403 Forbidden, but specifically for use when authentication is required and has failed or has not yet been provided.
        /// The response must include a WWW-Authenticate header field containing a challenge applicable to the requested resource.
        /// </summary>
        public static readonly HttpStatus Unauthorized = new(401, "Unauthorized");

        /// <summary>
        /// Reserved for future use.
        /// The original intention was that this code might be used as part of some form of digital cash or micropayment scheme, 
        /// but that has not happened, and this code is not usually used. As an example of its use, however, Apple's defunct MobileMe service 
        /// generated a 402 error if the MobileMe account was delinquent. In addition, YouTube uses this status if a particular IP address has made 
        /// excessive requests, and requires the person to enter a CAPTCHA.
        /// </summary>
        public static readonly HttpStatus PaymentRequired = new(402, "Payment Required");

        /// <summary>
        /// The request was a valid request, but the server is refusing to respond to it.
        /// Unlike a 401 Unauthorized response, authenticating will make no difference. On servers where authentication is required, 
        /// this commonly means that the provided credentials were successfully authenticated but that the credentials still do not grant 
        /// the client permission to access the resource (e.g. a recognized user attempting to access restricted content).
        /// </summary>
        public static readonly HttpStatus Forbidden = new(403, "Forbidden");

        /// <summary>
        /// The requested resource could not be found but may be available again in the future. 
        /// Subsequent requests by the client are permissible.
        /// </summary>
        public static readonly HttpStatus NotFound = new(404, "Not Found");

        /// <summary>
        /// A request was made of a resource using a request method not supported by that resource;
        /// for example, using GET on a form which requires data to be presented via POST, 
        /// or using PUT on a read-only resource.
        /// </summary>
        public static readonly HttpStatus MethodNotAllowed = new(405, "Method Not Allowed");

        /// <summary>
        /// The requested resource is only capable of generating content not acceptable according to the Accept headers sent in the request.
        /// </summary>
        public static readonly HttpStatus NotAcceptable = new(406, "Not Acceptable");

        /// <summary>
        /// The client must first authenticate itself with the proxy.
        /// </summary>
        public static readonly HttpStatus ProxyAuthenticationRequired = new(407, "Proxy Authentication Required");

        /// <summary>
        /// The server timed out waiting for the request.
        /// According to W3 HTTP specifications: "The client did not produce a request within the time that the server was prepared to wait. 
        /// The client MAY repeat the request without modifications at any later time."
        /// </summary>
        public static readonly HttpStatus RequestTimeout = new(408, "Request Timeout");

        /// <summary>
        /// Indicates that the request could not be processed because of conflict in the request, such as an edit conflict.
        /// </summary>
        public static readonly HttpStatus Conflict = new(409, "Conflict");

        /// <summary>
        /// Indicates that the resource requested is no longer available and will not be available again.
        /// This should be used when a resource has been intentionally removed and the resource should be purged.
        /// Upon receiving a 410 status code, the client should not request the resource again in the future. 
        /// Clients such as search engines should remove the resource from their indices.
        /// Most use cases do not require clients and search engines to purge the resource, and a "404 Not Found" may be used instead.
        /// </summary>
        public static readonly HttpStatus Gone = new(410, "Gone");

        /// <summary>
        /// The request did not specify the length of its content, which is required by the requested resource.
        /// </summary>
        public static readonly HttpStatus LengthRequired = new(411, "Length Required");
        
        /// <summary>
        /// The server does not meet one of the preconditions that the requester put on the request.
        /// </summary>
        public static readonly HttpStatus PreconditionFailed = new(412, "Precondition Failed");

        /// <summary>
        /// The request is larger than the server is willing or able to process.
        /// </summary>
        public static readonly HttpStatus RequestEntityTooLarge = new(413, "Request Entity Too Large");

        /// <summary>
        /// The URI provided was too long for the server to process.
        /// </summary>
        public static readonly HttpStatus RequestURITooLong = new(414, "Request-URI Too Long");

        /// <summary>
        /// The request entity has a media type which the server or resource does not support.
        /// For example, the client uploads an image as image/svg+xml, but the server requires that images use a different format.
        /// </summary>
        public static readonly HttpStatus UnsupportedMediaType = new(415, "Unsupported Media Type");

        /// <summary>
        /// The client has asked for a portion of the file, but the server cannot supply that portion. 
        /// For example, if the client asked for a part of the file that lies beyond the end of the file.
        /// </summary>
        public static readonly HttpStatus RequestedRangeNotSatisfiable = new(416, "Requested Range Not Satisfiable");

        /// <summary>
        /// The server cannot meet the requirements of the Expect request-header field.
        /// </summary>
        public static readonly HttpStatus ExpectationFailed = new(417, "Expectation Failed");

        /// <summary>
        /// Used to indicate that the server has returned no information to the client and closed the connection (useful as a deterrent for malware).
        /// </summary>
        public static readonly HttpStatus NoResponse = new(444, "No Response");

        /// <summary>
        /// Defined in the internet draft "A New HTTP Status Code for Legally-restricted Resources".
        /// Intended to be used when resource access is denied for legal reasons, e.g. censorship or government-mandated blocked access. 
        /// A reference to the 1953 dystopian novel Fahrenheit 451, where books are outlawed.
        /// </summary>
        public static readonly HttpStatus UnavailableForLegalReasons = new(451, "Unavailable For Legal Reasons");
    }

    /// <summary>
    /// The server failed to fulfill an apparently valid request.
    /// </summary>
    public static class ServerError
    {
        /// <summary>
        /// A generic error message, given when no more specific message is suitable.
        /// </summary>
        public static readonly HttpStatus InternalServerError = new(500, "Internal Server Error");

        /// <summary>
        /// The server either does not recognize the request method, or it lacks the ability to fulfil the request.
        /// </summary>
        public static readonly HttpStatus NotImplemented = new(501, "Not Implemented");

        /// <summary>
        /// The server was acting as a gateway or proxy and received an invalid response from the upstream server.
        /// </summary>
        public static readonly HttpStatus BadGateway = new(502, "Bad Gateway");

        /// <summary>
        /// The server is currently unavailable (because it is overloaded or down for maintenance).
        /// Generally, this is a temporary state.
        /// </summary>
        public static readonly HttpStatus ServiceUnavailable = new(503, "Service Unavailable");

        /// <summary>
        /// The server was acting as a gateway or proxy and did not receive a timely response from the upstream server.
        /// </summary>
        public static readonly HttpStatus GatewayTimeout = new(504, "Gateway Timeout");

        /// <summary>
        /// The server does not support the HTTP protocol version used in the request.
        /// </summary>
        public static readonly HttpStatus HTTPVersionNotSupported = new(505, "HTTP Version Not Supported");
    }
}
