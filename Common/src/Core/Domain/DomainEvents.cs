using System;
using System.Threading.Tasks;

namespace Common.Core.Domain
{
    /// <summary>
    /// Ref https://ardalis.com/immediate-domain-event-salvation-with-mediatr/
    /// </summary>
    public class DomainEvents
    {
        [ThreadStatic] // ensure separate func per thread to support parallel invocation
        public static Func<ICommandProcessor> CommandProcessor;
        public static async Task Raise<T>(T args) where T : IDomainEvent
        {
            var commandProcessor = CommandProcessor.Invoke();
            await commandProcessor.PublishDomainEventAsync(args);
        }
    }
}