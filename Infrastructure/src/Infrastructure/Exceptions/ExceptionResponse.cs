using System.Net;

namespace Infrastructure.Exceptions
{
    internal class ExceptionResponse
    {
        public object Response { get; }
        public HttpStatusCode StatusCode { get; }

        public ExceptionResponse(object response, HttpStatusCode statusCode)
        {
            Response = response;
            StatusCode = statusCode;
        }
    }
}