using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using BuildingBlocks.Core.Domain.IntegrationEvents;
using BuildingBlocks.Core.Extensions;
using BuildingBlocks.Core.Messaging;
using BuildingBlocks.Core.Messaging.Events;
using BuildingBlocks.Cqrs.Commands;

namespace BuildingBlocks.Core.Modules
{
    internal sealed class ModuleClient : IModuleClient
    {
        private readonly IModuleRegistry _moduleRegistry;
        private readonly IModuleSerializer _moduleSerializer;
        private readonly ConcurrentDictionary<Type, MessageAttribute> _registrations = new();
        public ModuleClient(IModuleRegistry moduleRegistry, IModuleSerializer moduleSerializer)
        {
            _moduleRegistry = moduleRegistry;
            _moduleSerializer = moduleSerializer;
        }

        public Task SendAsync(string path, object request) => SendAsync<object>(path, request);

        public async Task<TResult> SendAsync<TResult>(string path, object request) where TResult : class
        {
            var registration = _moduleRegistry.GetRequestRegistration(path);
            if (registration is null)
            {
                throw new InvalidOperationException($"No action has been defined for path: '{path}'.");
            }

            if (request is IIntegrationEvent message)
            {
                // A synchronous request
                message.Id = Guid.Empty;
            }

            var receiverRequest = TranslateType(request, registration.RequestType);
            var result = await registration.Action(receiverRequest);

            return result is null ? null : TranslateType<TResult>(result);
        }

        public async Task PublishAsync(IMessage message)
        {
            var module = message.GetModuleName();
            var tasks = new List<Task>();
            var path = message.GetType().Name;
            var registrations = _moduleRegistry
                .GetBroadcastRegistrations(path)
                .Where(r => r.ReceiverType != message.GetType());

            foreach (var registration in registrations)
            {
                if (!_registrations.TryGetValue(registration.ReceiverType, out var messageAttribute))
                {
                    messageAttribute = registration.ReceiverType.GetCustomAttribute<MessageAttribute>();
                    if (message is ICommand)
                    {
                        messageAttribute = message.GetType().GetCustomAttribute<MessageAttribute>();
                        module = registration.ReceiverType.GetModuleName();
                    }

                    _registrations.TryAdd(registration.ReceiverType, messageAttribute);
                }

                if (messageAttribute is null || !messageAttribute.Enabled)
                {
                    continue;
                }

                if (messageAttribute.Module != module)
                {
                    continue;
                }

                var action = registration.Action;
                var receiverBroadcast = TranslateType(message, registration.ReceiverType);
                tasks.Add(action(receiverBroadcast));
            }

            await Task.WhenAll(tasks);
        }

        private T TranslateType<T>(object value)
            => _moduleSerializer.Deserialize<T>(_moduleSerializer.Serialize(value));
        
        private object TranslateType(object value, Type type)
            => _moduleSerializer.Deserialize(_moduleSerializer.Serialize(value), type);
    }
}