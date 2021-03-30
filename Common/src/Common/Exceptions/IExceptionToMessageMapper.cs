using System;
using System.Collections.Generic;
using Common.Messaging.Events;

namespace Common.Exceptions
{
    public interface IExceptionToMessageMapper
    {
        IEnumerable<Type> ExceptionTypes { get; }
        IActionRejected Map<T>(T exception) where T : Exception;
    }
}
