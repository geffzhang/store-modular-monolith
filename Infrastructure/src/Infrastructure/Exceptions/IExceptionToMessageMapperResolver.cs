using System;
using Infrastructure.Messaging.Events;

namespace Infrastructure.Exceptions
{
    public interface IExceptionToMessageMapperResolver
    {
        IActionRejected Map<T>(T exception) where T : Exception;
    }
}