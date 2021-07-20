using System;
using Common.Core.Messaging;

namespace Common.Core.Exceptions
{
    public interface IExceptionToMessageMapperResolver
    {
        IActionRejected Map<T>(T exception) where T : Exception;
    }
}