using System;
using System.Collections.Generic;
using Common.Core.Extensions;
using Common.Core.Messaging;
using Common.Core.Messaging.Events;

namespace Common.Core.Exceptions
{
    internal sealed class ExceptionToMessageMapper : IExceptionToMessageMapper
    {
        public IEnumerable<Type> ExceptionTypes { get; } =
            new[] {typeof(AppException), typeof(DomainException), typeof(Exception)};

        public IActionRejected Map<T>(T exception) where T : Exception
        {
            return exception switch
            {
                AppException ex => new ActionRejected(ex.GetExceptionCode(), ex.Message),
                DomainException ex => new ActionRejected(ex.GetExceptionCode(), ex.Message),
                _ => new ActionRejected("error", "There was an error.")
            };
        }
    }
}