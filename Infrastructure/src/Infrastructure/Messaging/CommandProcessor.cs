using System;
using System.Threading.Tasks;
using IInfrastructure.Contexts;
using Infrastructure.Messaging.Commands;
using Infrastructure.Messaging.Events;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Messaging
{
    internal sealed class CommandProcessor : ICommandProcessor
    {
        private readonly IServiceScopeFactory _serviceFactory;
        private readonly IMessageChannel _channel;

        public CommandProcessor(IServiceScopeFactory serviceFactory, IMessageChannel channel)
        {
            _serviceFactory = serviceFactory;
            this._channel = channel;
        }
       
        public async Task PublishEventAsync<T>(T @event) where T : class, IEvent
        {
            if (@event is null)
            {
                return;
            }

            using var scope = _serviceFactory.CreateScope();
            if (@event.CorrelationId == Guid.Empty)
            {
                var context = scope.ServiceProvider.GetRequiredService<IContext>();
                @event.CorrelationId = context.CorrelationId;
            }

            if (typeof(T) == typeof(IEvent))
            {
                var handlerType = typeof(IEventHandler<>).MakeGenericType(@event.GetType());
                var eventHandlers = scope.ServiceProvider.GetServices(handlerType);
                foreach (var handler in eventHandlers)
                {
                    if (handler is null)
                    {
                        continue;
                    }

                    await (Task)handler.GetType().GetMethod(nameof(IEventHandler<T>.HandleAsync))
                        ?.Invoke(handler, new[] { @event });
                }

                // dynamic eventHandlers = scope.ServiceProvider.GetServices(handlerType);
                // foreach (var handler in eventHandlers)
                // {
                //     await (Task) handler.HandleAsync((dynamic) @event);
                // }

                return;
            }

            var handlers = scope.ServiceProvider.GetServices<IEventHandler<T>>();
            foreach (var handler in handlers)
            {
                await handler.HandleAsync(@event);
            }
        }
       
        public async Task SendCommandAsync<T>(T command) where T : class, ICommand
        {
            if (command is null)
            {
                return;
            }

            using var scope = _serviceFactory.CreateScope();
            if (command.CorrelationId == Guid.Empty)
            {
                var context = scope.ServiceProvider.GetRequiredService<IContext>();
                command.CorrelationId = context.CorrelationId;
            }

            var handler = scope.ServiceProvider.GetRequiredService<ICommandHandler<T>>();
            await handler.HandleAsync(command);
        }
       
        public async Task PublishMessageAsync<T>(T message) where T : class, IMessage
            => await _channel.Writer.WriteAsync(message);
    }

}
