using System;
using Common.Messages.Serialization.Hybrid;
using Common.Messages.Serialization.Json.Newtonsoft;
using Common.Messages.Serialization.Json.SystemTextJson;
using Microsoft.Extensions.DependencyInjection;

namespace Common.Messages.Serialization.Json
{
    public static class Extensions
    {
        public static IServiceCollection AddJson(this IServiceCollection services,
            Action<TextJsonSerializerOptions> textJsonSerializerOptions = null)
        {
            services.Configure(textJsonSerializerOptions);
            
            services.Configure<JsonOptions>(options => { options.Providers.Add<TextJsonMessageSerializer>(); });
            services.AddSingleton<IJsonSerializer, JsonSerializer>();
            
            return services;
        }

        public static IServiceCollection AddJson(this IServiceCollection services,
            Action<NewtonsoftJsonOptions> newtonsoftJsonOptions = null)
        {
            services.Configure(newtonsoftJsonOptions);

            services.Configure<JsonOptions>(options => { options.Providers.Add<NewtonsoftJsonSerializerProvider>(); });
            services.AddSingleton<IJsonSerializer, JsonSerializer>();
            
            return services;
        }
    }
}