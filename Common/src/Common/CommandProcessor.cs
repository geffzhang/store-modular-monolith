using System;
using System.Linq;
using System.Threading.Tasks;
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
        private readonly IServiceProvider _serviceProvider;

        public CommandProcessor(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task PublishDomainEventAsync(params IDomainEvent[] domainEvents) 
        {
            if (domainEvents is null || !domainEvents.Any())
            {
                return;
            }
            
            using var scope = _serviceProvider.CreateScope();
            var dispatcher = scope.ServiceProvider.GetRequiredService<IDomainEventDispatcher>();
            await dispatcher.DispatchAsync(domainEvents);
        }

        public async Task PublishIntegrationEventAsync<T>(T integrationEvent) where T : class, IIntegrationEvent
        {
            if (integrationEvent is null) return;

            using var scope = _serviceProvider.CreateScope();
            var dispatcher = scope.ServiceProvider.GetRequiredService<IIntegrationEventDispatcher>();
            await dispatcher.PublishAsync(integrationEvent);
        }

        public async Task SendCommandAsync<T>(T command) where T : class, ICommand
        {
            if (command is null) return;

            using var scope = _serviceProvider.CreateScope();
            var dispatcher = scope.ServiceProvider.GetRequiredService<ICommandDispatcher>();
            await dispatcher.SendAsync(command);
        }

        public async Task PublishMessageAsync(params IMessage[] messages)
        {
            using var scope = _serviceProvider.CreateScope();
            var transport = scope.ServiceProvider.GetRequiredService<ITransport>();
            await transport.PublishAsync(messages);
        }
    }
}