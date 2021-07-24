using System;
using System.Linq;
using Ardalis.GuardClauses;
using BuildingBlocks.Core.Collections;
using BuildingBlocks.Core.Messaging.Serialization;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BuildingBlocks.Messages.Serialization.Hybrid
{
    public class HybridMessageSerializer : IHybridMessageSerializer
    {
        private readonly ILogger<HybridMessageSerializer> _logger;
        private readonly ITypeList<IMessageSerializer> _serializers = new TypeList<IMessageSerializer>();
        protected IServiceScopeFactory ServiceScopeFactory { get; }
        public IMessageSerializer DefaultSerializer { get; set; }

        public HybridMessageSerializer(IOptions<HybridSerializationOptions> options,
            IServiceScopeFactory serviceScopeFactory, ILogger<HybridMessageSerializer> logger)
        {
            _logger = logger;
            ServiceScopeFactory = serviceScopeFactory;
            foreach (var optionsProvider in options.Value.Providers)
            {
                _serializers.Add(optionsProvider);
            }
        }

        public void Add(IMessageSerializer serializer)
        {
            if (_serializers.Count == 0 && DefaultSerializer == null)
            {
                DefaultSerializer = serializer;
            }

            _serializers.Add(serializer.GetType());
        }

        public string Serialize([CanBeNull] object obj, bool camelCase = true, bool indented = false)
        {
            using (var scope = ServiceScopeFactory.CreateScope())
            {
                var serializerProvider = GetSerializerProvider(scope.ServiceProvider, obj?.GetType());
                return serializerProvider.Serialize(obj, camelCase, indented);
            }
        }

        public T Deserialize<T>([NotNull] string payload, bool camelCase = true)
        {
            Guard.Against.Null(payload, nameof(payload));

            using (var scope = ServiceScopeFactory.CreateScope())
            {
                var serializerProvider = GetSerializerProvider(scope.ServiceProvider, typeof(T));
                return serializerProvider.Deserialize<T>(payload, camelCase);
            }
        }

        public object Deserialize(Type type, [NotNull] string jsonString, bool camelCase = true)
        {
            Guard.Against.Null(jsonString, nameof(jsonString));

            using (var scope = ServiceScopeFactory.CreateScope())
            {
                var serializerProvider = GetSerializerProvider(scope.ServiceProvider, type);
                return serializerProvider.Deserialize(jsonString, type, camelCase);
            }
        }

        protected virtual IMessageSerializer GetSerializerProvider(IServiceProvider serviceProvider,
            [CanBeNull] Type type)
        {
            foreach (var providerType in _serializers.Reverse())
            {
                var provider = serviceProvider.GetRequiredService(providerType) as IMessageSerializer;
                if (provider.CanHandle(type))
                {
                    return provider;
                }
            }

            throw new Exception($"There is no IJsonSerializerProvider that can handle '{type.FullName}'!");
        }
    }
}