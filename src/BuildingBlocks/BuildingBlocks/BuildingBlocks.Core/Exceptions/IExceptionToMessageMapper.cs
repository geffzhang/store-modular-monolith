using System;
using System.Collections.Generic;
using BuildingBlocks.Core.Messaging;

namespace BuildingBlocks.Core.Exceptions
{
    public interface IExceptionToMessageMapper
    {
        IEnumerable<Type> ExceptionTypes { get; }
        IActionRejected Map<T>(T exception) where T : Exception;
    }
}