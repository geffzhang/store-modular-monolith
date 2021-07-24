using System;
using BuildingBlocks.Core.Messaging;

namespace BuildingBlocks.Core.Exceptions
{
    public interface IExceptionToMessageMapperResolver
    {
        IActionRejected Map<T>(T exception) where T : Exception;
    }
}