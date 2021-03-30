using System;
using Common.Messaging.Events;

namespace Common.Exceptions
{
    public interface IExceptionToMessageMapperResolver
    {
        IActionRejected Map<T>(T exception) where T : Exception;
    }
}