using System;
using System.Linq;
using Ardalis.GuardClauses;
using Common.Serialization;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Common.Messages.Serialization.Hybrid
{
    public class HybridMessageSerializer : IHybridSerializer
    {
        private readonly Logger<HybridJsonSerializer> _logger;
        protected JsonOptions Options { get; }

        protected IServiceScopeFactory ServiceScopeFactory { get; }

        public HybridJsonSerializer(IOptions<HybridSerializationOptions> options, IServiceScopeFactory serviceScopeFactory, Logger<HybridJsonSerializer> logger)
        {
            _logger = logger;
            Options = options.Value;
            ServiceScopeFactory = serviceScopeFactory;
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
                return serializerProvider.Deserialize(type, jsonString, camelCase);
            }
        }
        
        protected virtual IMessageSerializer GetSerializerProvider(IServiceProvider serviceProvider,
            [CanBeNull] Type type)
        {
            foreach (var providerType in Options.Providers.Reverse())
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