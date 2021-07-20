using System;

namespace Common.Core.Exceptions
{
    internal interface IExceptionToResponseMapper
    {
        ExceptionResponse Map(Exception exception);
    }
}