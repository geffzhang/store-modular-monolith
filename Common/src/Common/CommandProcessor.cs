using System;
using System.Threading.Tasks;
using Common.Contexts;
using Common.Domain;
using Common.Messaging;
using Common.Messaging.Commands;
using Common.Messaging.Events;
using Common.Messaging.Transport;
using Microsoft.Extensions.DependencyInjection;

namespace Common
{
    internal sealed class CommandProcessor : ICommandProcessor
    {
        private readonly IServiceScopeFactory _serviceFactory;

        public CommandProcessor(IServiceScopeFactory serviceFactory)
        {
            _serviceFactory = serviceFactory;
        }

        public async Task PublishDomainEventAsync<T>(T domainEvent) where T : class, IDomainEvent
        {
            if (domainEvent is null) return;

            using var scope = _serviceFactory.CreateScope();

            if (typeof(T) == typeof(IDomainEvent))
            {
                var handlerType = typeof(IDomainEventHandler<>).MakeGenericType(domainEvent.GetType());
                var eventHandlers = scope.ServiceProvider.GetServices(handlerType);
                foreach (var handler in eventHandlers)
                {
                    if (handler is null) continue;

                    await (Task) handler.GetType().GetMethod(nameof(IDomainEventHandler<T>.HandleAsync))
                        ?.Invoke(handler, new[] {domainEvent});
                }

                return;
            }

            var handlers = scope.ServiceProvider.GetServices<IDomainEventHandler<T>>();
            foreach (var handler in handlers) await handler.HandleAsync(domainEvent);
        }

        public async Task PublishDomainNotificationEventAsync<T>(T domainNotificationEvent)
            where T : class, IDomainNotificationEvent
        {
            if (domainNotificationEvent is null) return;

            using var scope = _serviceFactory.CreateScope();

            if (typeof(T) == typeof(IDomainNotificationEvent))
            {
                var handlerType = typeof(IDomainNotificationEvent<>).MakeGenericType(domainNotificationEvent.GetType());
                var eventHandlers = scope.ServiceProvider.GetServices(handlerType);
                foreach (var handler in eventHandlers)
                {
                    if (handler is null) continue;

                    await (Task) handler.GetType().GetMethod(nameof(IDomainNotificationEventHandler<T>.HandleAsync))
                        ?.Invoke(handler, new[] {domainNotificationEvent});
                }

                return;
            }

            var handlers = scope.ServiceProvider.GetServices<IDomainNotificationEventHandler<T>>();
            foreach (var handler in handlers) await handler.HandleAsync(domainNotificationEvent);
        }

        public async Task PublishIntegrationEventAsync<T>(T integrationEvent) where T : class, IIntegrationEvent
        {
            if (integrationEvent is null) return;

            using var scope = _serviceFactory.CreateScope();
            if (integrationEvent.CorrelationId == Guid.Empty)
            {
                var context = scope.ServiceProvider.GetRequiredService<IContext>();
                integrationEvent.CorrelationId = context.CorrelationId;
            }

            if (typeof(T) == typeof(IIntegrationEvent))
            {
                var handlerType = typeof(IIntegrationEventHandler<>).MakeGenericType(integrationEvent.GetType());
                var eventHandlers = scope.ServiceProvider.GetServices(handlerType);
                foreach (var handler in eventHandlers)
                {
                    if (handler is null) continue;

                    await (Task) handler.GetType().GetMethod(nameof(IIntegrationEventHandler<T>.HandleAsync))
                        ?.Invoke(handler, new[] {integrationEvent});
                }

                return;
            }

            var handlers = scope.ServiceProvider.GetServices<IIntegrationEventHandler<T>>();
            foreach (var handler in handlers) await handler.HandleAsync(integrationEvent);
        }

        public async Task SendCommandAsync<T>(T command) where T : class, ICommand
        {
            if (command is null) return;

            using var scope = _serviceFactory.CreateScope();
            if (command.CorrelationId == Guid.Empty)
            {
                var context = scope.ServiceProvider.GetRequiredService<IContext>();
                command.CorrelationId = context.CorrelationId;
            }

            var handler = scope.ServiceProvider.GetRequiredService<ICommandHandler<T>>();
            await handler.HandleAsync(command);
        }

        public async Task PublishMessageAsync<T>(T message) where T : IMessage
        {
            using var scope = _serviceFactory.CreateScope();
            var transport = scope.ServiceProvider.GetRequiredService<ITransport>();

            await transport.PublishAsync(message);
        }
    }
}