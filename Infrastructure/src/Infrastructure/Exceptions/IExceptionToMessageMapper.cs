using System;
using System.Collections.Generic;
using Infrastructure.Messaging.Events;

namespace Infrastructure.Exceptions
{
    public interface IExceptionToMessageMapper
    {
        IEnumerable<Type> ExceptionTypes { get; }
        IActionRejected Map<T>(T exception) where T : Exception;
    }
}
