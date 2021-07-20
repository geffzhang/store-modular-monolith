using System;
using System.Collections.Generic;
using Common.Core.Messaging;

namespace Common.Core.Exceptions
{
    public interface IExceptionToMessageMapper
    {
        IEnumerable<Type> ExceptionTypes { get; }
        IActionRejected Map<T>(T exception) where T : Exception;
    }
}