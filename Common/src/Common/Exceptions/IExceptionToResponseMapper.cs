using System;

namespace Common.Exceptions
{
    internal interface IExceptionToResponseMapper
    {
        ExceptionResponse Map(Exception exception);
    }
}