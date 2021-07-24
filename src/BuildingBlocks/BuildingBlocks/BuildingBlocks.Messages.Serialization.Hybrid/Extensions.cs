using System;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.Messages.Serialization.Hybrid
{
    public static class Extensions
    {
        public static IServiceCollection AddHybridMessageSerializer(this IServiceCollection services,
            Action<HybridSerializationOptions> hybridSerializationOptions)
        {
            services.Configure(hybridSerializationOptions);
            
            services.AddSingleton<IHybridMessageSerializer, HybridMessageSerializer>();
            
            return services;
        }
    }
}