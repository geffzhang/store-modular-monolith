using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Common.Messaging;
using Common.Messaging.Commands;
using Common.Utils;

namespace Common.Modules
{
    internal sealed class ModuleClient : IModuleClient
    {
        private readonly IModuleRegistry _moduleRegistry;

        private readonly ConcurrentDictionary<Type, MessageAttribute> _registrations =
            new();

        private readonly IModuleSerializer _serializer;

        public ModuleClient(IModuleRegistry moduleRegistry, IModuleSerializer serializer)
        {
            _moduleRegistry = moduleRegistry;
            _serializer = serializer;
        }

        public async Task<TResult> RequestAsync<TResult>(string path, object request) where TResult : class
        {
            var registration = _moduleRegistry.GetRequestRegistration(path);
            if (registration is null)
                throw new InvalidOperationException($"No action has been defined for path: {path}");

            if (request is IMessage message)
                // A synchronous request
                message.Id = Guid.Empty;

            var receiverRequest = TranslateType(request, registration.RequestType);
            var result = await registration.Action(receiverRequest);

            return _serializer.Deserialize<TResult>(_serializer.Serialize(result));
        }

        public async Task SendAsync(IMessage message)
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

                if (messageAttribute is null || !messageAttribute.Enabled) continue;

                if (messageAttribute.Module != module) continue;

                var action = registration.Action;
                var receiverBroadcast = TranslateType(message, registration.ReceiverType);
                tasks.Add(action(receiverBroadcast));
            }

            await Task.WhenAll(tasks);
        }

        private object TranslateType(object value, Type type)
        {
            return _serializer.Deserialize(_serializer.Serialize(value), type);
        }
    }
}