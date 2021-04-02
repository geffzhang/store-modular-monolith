using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Common.Exceptions;
using Common.Messaging.Commands;
using Common.Messaging.Events;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Common.Modules
{
    public static class Extensions
    {
        public static IModuleSubscriber UseModuleRequests(this IApplicationBuilder app)
        {
            return app.ApplicationServices.GetRequiredService<IModuleSubscriber>();
        }

        public static IContractRegistry UseContracts(this IApplicationBuilder app)
        {
            return app.ApplicationServices.GetRequiredService<IContractRegistry>();
        }

        public static IServiceCollection AddExceptionToMessageMapper<T>(this IServiceCollection services)
            where T : class, IExceptionToMessageMapper
        {
            services.AddSingleton<T>();
            services.AddSingleton<IExceptionToMessageMapper, T>();

            return services;
        }

        internal static IServiceCollection AddModuleRequests(this IServiceCollection services,
            IList<Assembly> assemblies)
        {
            services.AddModuleRegistry(assemblies);
            services.AddSingleton<IModuleSubscriber, ModuleSubscriber>();
            services.AddSingleton<IModuleClient, ModuleClient>();
            services.AddSingleton<IContractRegistry, ContractRegistry>();

            return services;
        }

        private static void AddModuleRegistry(this IServiceCollection services, IEnumerable<Assembly> assemblies)
        {
            var types = assemblies.SelectMany(a => a.GetTypes()).ToArray();

            var commandTypes = types
                .Where(t => t.IsClass && typeof(ICommand).IsAssignableFrom(t))
                .ToArray();

            var eventTypes = types
                .Where(t => t.IsClass && typeof(IIntegrationEvent).IsAssignableFrom(t))
                .ToArray();

            // services.AddSingleton<IModuleSerializer, JsonModuleSerializer>();
            services.AddSingleton<IModuleSerializer, MessagePackModuleSerializer>();
            services.AddSingleton<IModuleRegistry>(sp =>
            {
                var registry = new ModuleRegistry();
                var commandProcessor = sp.GetRequiredService<ICommandProcessor>();
                var dispatcherType = commandProcessor.GetType();

                foreach (var type in commandTypes)
                    registry.AddBroadcastAction(type, @event =>
                        (Task) dispatcherType.GetMethod(nameof(commandProcessor.SendCommandAsync))
                            ?.MakeGenericMethod(type)
                            .Invoke(commandProcessor, new[] {@event}));

                foreach (var type in eventTypes)
                    registry.AddBroadcastAction(type, @event =>
                        (Task) dispatcherType.GetMethod(nameof(commandProcessor.PublishIntegrationEventAsync))
                            ?.MakeGenericMethod(type)
                            .Invoke(commandProcessor, new[] {@event}));

                return registry;
            });
        }
    }
}