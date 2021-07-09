using System;
using System.Collections.Generic;
using Common.Extensions;
using Common.Messaging;
using Common.Messaging.Events;
using Common.Utils;

namespace Common.Exceptions
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