using System;

namespace Common.Core.Exceptions
{
    internal interface IExceptionCompositionRoot
    {
        ExceptionResponse Map(Exception exception);
    }
}