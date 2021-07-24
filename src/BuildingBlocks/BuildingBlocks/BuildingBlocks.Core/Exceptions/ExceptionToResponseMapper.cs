using System;
using System.Collections.Concurrent;
using System.Net;
using BuildingBlocks.Core.Extensions;

namespace BuildingBlocks.Core.Exceptions
{
    internal sealed class ExceptionToResponseMapper : IExceptionToResponseMapper
    {
        private static readonly ConcurrentDictionary<Type, string> Codes = new();

        public ExceptionResponse Map(Exception exception)
        {
            return exception switch
            {
                DomainException ex => new ExceptionResponse(new {code = GetCode(ex), reason = ex.Message},
                    HttpStatusCode.BadRequest),
                AppException ex => new ExceptionResponse(new {code = GetCode(ex), reason = ex.Message},
                    HttpStatusCode.BadRequest),
                _ => new ExceptionResponse(new {code = "error", reason = "There was an error."},
                    HttpStatusCode.InternalServerError)
            };
        }

        private static string GetCode(Exception exception)
        {
            var type = exception.GetType();
            if (Codes.TryGetValue(type, out var code)) return code;

            var exceptionCode = exception.GetExceptionCode();
            Codes.TryAdd(type, exceptionCode);

            return exceptionCode;
        }
    }
}