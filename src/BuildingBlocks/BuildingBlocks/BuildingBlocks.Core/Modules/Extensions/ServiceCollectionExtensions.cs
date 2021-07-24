using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using BuildingBlocks.Core.Exceptions;
using BuildingBlocks.Core.Messaging.Commands;
using BuildingBlocks.Core.Messaging.Events;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace BuildingBlocks.Core.Modules.Extensions
{
    public static class ServiceCollectionExtensions
    {
        private static readonly JsonSerializerSettings SerializerSettings = new()
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            Converters = new List<JsonConverter>
            {
                new StringEnumConverter(new CamelCaseNamingStrategy())
            }
        };

        public static IServiceCollection AddExceptionToMessageMapper<T>(this IServiceCollection services)
            where T : class, IExceptionToMessageMapper
        {
            services.AddSingleton<T>();
            services.AddSingleton<IExceptionToMessageMapper, T>();

            return services;
        }
        
        internal static IServiceCollection AddModuleRequests(this IServiceCollection services, IList<Assembly> assemblies)
        {
            services.AddModuleRegistry(assemblies);
            services.AddSingleton<IModuleSubscriber, ModuleSubscriber>();
            services.AddSingleton<IModuleClient, ModuleClient>();
            services.AddSingleton<IContractRegistry, ContractRegistry>();

            return services;
        }
        
        internal static IServiceCollection AddModuleInfo(this IServiceCollection services, IList<IModule> modules)
        {
            if (modules != null)
            {
                var moduleInfoProvider = new ModuleInfoProvider();
                var moduleInfo =
                    modules?.Select(x => new ModuleInfo(x.Name, x.Path, x.Policies ?? Enumerable.Empty<string>())) ??
                    Enumerable.Empty<ModuleInfo>();
                moduleInfoProvider.Modules.AddRange(moduleInfo);
                services.AddSingleton(moduleInfoProvider);
            }

            return services;
        }

        internal static void MapModuleInfo(this IEndpointRouteBuilder endpoint)
        {
            endpoint.MapGet("modules", context =>
            {
                var moduleInfoProvider = context.RequestServices.GetRequiredService<ModuleInfoProvider>();
                var json = JsonConvert.SerializeObject(moduleInfoProvider.Modules, SerializerSettings);
                context.Response.ContentType = "application/json";
                return context.Response.WriteAsync(json);
            });
        }
        
        internal static IHostBuilder ConfigureModules(this IHostBuilder builder)
            => builder.ConfigureAppConfiguration((ctx, cfg) =>
            {
                foreach (var settings in GetSettings("*"))
                {
                    cfg.AddJsonFile(settings);
                }

                foreach (var settings in GetSettings($"*.{ctx.HostingEnvironment.EnvironmentName}"))
                {
                    cfg.AddJsonFile(settings);
                }

                IEnumerable<string> GetSettings(string pattern)
                    => Directory.EnumerateFiles(ctx.HostingEnvironment.ContentRootPath,
                        $"module.{pattern}.json", SearchOption.AllDirectories);
            });

        private static void AddModuleRegistry(this IServiceCollection services, IEnumerable<Assembly> assemblies)
        {
            var types = assemblies.SelectMany(a => a.GetTypes()).ToArray();

            var commandTypes = types
                .Where(t => t.IsClass && typeof(ICommand).IsAssignableFrom(t))
                .ToArray();

            var eventTypes = types
                .Where(t => t.IsClass && typeof(IEvent).IsAssignableFrom(t))
                .ToArray();

            // services.AddSingleton<IModuleSerializer, JsonModuleSerializer>();
            services.AddSingleton<IModuleSerializer, MessagePackModuleSerializer>();
            services.AddSingleton<IModuleRegistry>(sp =>
            {
                var registry = new ModuleRegistry();
                var commandProcessor = sp.GetRequiredService<ICommandProcessor>();
                var commandProcessorType = commandProcessor.GetType();

                foreach (var type in commandTypes)
                {
                    registry.AddBroadcastAction(type, @event =>
                        (Task) commandProcessorType.GetMethod(nameof(commandProcessor.SendCommandAsync))
                            ?.MakeGenericMethod(type)
                            .Invoke(commandProcessor, new[] {@event}));
                }

                foreach (var type in eventTypes)
                {
                    registry.AddBroadcastAction(type, @event =>
                        (Task) commandProcessorType.GetMethod(nameof(commandProcessor.PublishDomainEventAsync))
                            ?.MakeGenericMethod(type)
                            .Invoke(commandProcessor, new[] {@event}));
                }

                return registry;
            });
        }
    }
}
