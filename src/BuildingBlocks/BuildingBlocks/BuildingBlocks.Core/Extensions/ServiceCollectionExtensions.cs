using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using BuildingBlocks.Core.Domain;
using BuildingBlocks.Core.Domain.Dispatching;
using BuildingBlocks.Core.Exceptions;
using BuildingBlocks.Core.Mail;
using BuildingBlocks.Core.Messaging;
using BuildingBlocks.Core.Messaging.Commands;
using BuildingBlocks.Core.Messaging.Events;
using BuildingBlocks.Core.Messaging.Queries;
using BuildingBlocks.Core.Messaging.Serialization;
using BuildingBlocks.Core.Messaging.Serialization.Newtonsoft;
using BuildingBlocks.Core.Messaging.Transport;
using BuildingBlocks.Core.Scheduling;
using BuildingBlocks.Logging.Serilog;
using BuildingBlocks.Persistence.Mongo;
using BuildingBlocks.Web.Contexts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using IMailService = BuildingBlocks.Core.Mail.IMailService;

[assembly: InternalsVisibleTo("OnlineStore.API")]
[assembly: InternalsVisibleTo("OnlineStore.Tests.Benchmarks")]
[assembly: InternalsVisibleTo("OnlineStore.BuildingBlocks.Tests.Integration")]

namespace BuildingBlocks.Core.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCore(this IServiceCollection services, IConfiguration configuration,
            IList<Assembly> assemblies = null, Action<IServiceCollection> doMoreActions = null)
        {
            services.Configure<MailConfiguration>(configuration.GetSection("MailConfiguration"));
            services.AddSingleton<IMailService, SmtpMailService>();

            services.AddNewtonsoftMessageSerializer(options =>
            {
                options.Converters = new List<JsonConverter> {new StringEnumConverter(new CamelCaseNamingStrategy())};
                //options.UnSupportedTypes.Add<Test>();
            });

            services.AddCommand(assemblies ?? AppDomain.CurrentDomain.GetAssemblies());
            services.AddEvent(assemblies ?? AppDomain.CurrentDomain.GetAssemblies());
            services.AddMessage(assemblies ?? AppDomain.CurrentDomain.GetAssemblies());
            services.AddQuery(assemblies ?? AppDomain.CurrentDomain.GetAssemblies());
            services.AddDomainEvents(assemblies ?? AppDomain.CurrentDomain.GetAssemblies());

            services
                .AddMongoPersistence(configuration)
                // .AddModuleInfo(modules)
                // .AddModuleRequests(assemblies ?? AppDomain.CurrentDomain.GetAssemblies())
                .AddSingleton<IExceptionToResponseMapper, ExceptionToResponseMapper>()
                .AddSingleton<IExceptionToMessageMapper, ExceptionToMessageMapper>()
                .AddSingleton<IExceptionCompositionRoot, ExceptionCompositionRoot>()
                .AddSingleton<IExceptionToMessageMapperResolver, ExceptionToMessageMapperResolver>()
                .AddSingleton<ICommandProcessor, CommandProcessor>()
                .AddSingleton<IQueryProcessor, QueryProcessor>()
                .AddSingleton<IMessagesExecutor, MessagesExecutor>()
                .AddSingleton<IExecutionContextAccessor, ExecutionContextAccessor>();

            services.TryDecorate(typeof(ICommandHandler<>), typeof(CommandHandlerLoggingDecorator<>));
            services.TryDecorate(typeof(IMessageHandler<>), typeof(MessageHandlerLoggingDecorator<>));
            services.TryDecorate(typeof(IQueryHandler<,>), typeof(QueryHandlerLoggingDecorator<,>));

            DomainEvents.CommandProcessor = () => ServiceActivator.GetService<ICommandProcessor>();

            doMoreActions?.Invoke(services);

            return services;
        }

        private static IServiceCollection AddNewtonsoftMessageSerializer(this IServiceCollection services,
            Action<NewtonsoftJsonOptions> newtonsoftJsonOptions = null)
        {
            services.Configure(newtonsoftJsonOptions);

            services.AddSingleton<NewtonsoftJsonUnSupportedTypeMatcher>();
            services.AddSingleton<IMessageSerializer, NewtonsoftJsonMessageSerializer>();

            return services;
        }

        public static IServiceCollection AddDomainEvents(this IServiceCollection services,
            IEnumerable<Assembly> assemblies)
        {
            services.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();
            services.AddScoped<IDomainEventNotificationDispatcher, DomainEventNotificationDispatcher>();

            services.Scan(s => s.FromAssemblies(assemblies)
                .AddClasses(c => c.AssignableTo(typeof(IEventHandler<>))
                    .WithoutAttribute(typeof(DecoratorAttribute)))
                .AsImplementedInterfaces()
                .WithScopedLifetime());

            services.Scan(s => s.FromAssemblies(assemblies)
                .AddClasses(c => c.AssignableTo(typeof(IDomainEventNotificationHandler<>))
                    .WithoutAttribute(typeof(DecoratorAttribute)))
                .AsImplementedInterfaces()
                .WithScopedLifetime());

            return services;
        }

        private static IServiceCollection AddMessage(this IServiceCollection services, IList<Assembly> assemblies)
        {
            services.AddSingleton<IMessageDispatcher, MessageDispatcher>();

            services.Scan(s => s.FromAssemblies(assemblies).AddClasses(c => c.AssignableTo(typeof(IMessageHandler<>))
                    .WithoutAttribute(typeof(DecoratorAttribute)))
                .AsImplementedInterfaces()
                .WithScopedLifetime());

            return services;
        }

        private static IServiceCollection AddCommand(this IServiceCollection services,
            IList<Assembly> assemblies)
        {
            services.AddSingleton<ICommandDispatcher, CommandDispatcher>();

            services.Scan(s => s.FromAssemblies(assemblies)
                .AddClasses(c => c.AssignableTo(typeof(ICommandHandler<>))
                    .WithoutAttribute(typeof(DecoratorAttribute)))
                .AsImplementedInterfaces()
                .WithScopedLifetime());

            return services;
        }

        private static IServiceCollection AddEvent(this IServiceCollection services,
            IList<Assembly> assemblies)
        {
            services.AddSingleton<IEventDispatcher, EventDispatcher>();

            services.Scan(s => s.FromAssemblies(assemblies)
                .AddClasses(c => c.AssignableTo(typeof(IEventHandler<>))
                    .WithoutAttribute(typeof(DecoratorAttribute)))
                .AsImplementedInterfaces()
                .WithScopedLifetime());

            return services;
        }

        private static IServiceCollection AddQuery(this IServiceCollection services, IList<Assembly> assemblies)
        {
            services.AddSingleton<IQueryDispatcher, QueryDispatcher>();

            services.Scan(s => s.FromAssemblies(assemblies)
                .AddClasses(c => c.AssignableTo(typeof(IQueryHandler<,>))
                    .WithoutAttribute(typeof(DecoratorAttribute)))
                .AsImplementedInterfaces()
                .WithScopedLifetime());

            return services;
        }

        public static IServiceCollection AddBusSubscriber(this IServiceCollection services, Type subscriberType)
        {
            if (services.All(s => s.ImplementationType != subscriberType))
                services.AddSingleton(typeof(ISubscriber), subscriberType);
            return services;
        }

        public static void Unregister<TService>(this IServiceCollection services)
        {
            var descriptor = services.FirstOrDefault(d => d.ServiceType == typeof(TService));
            services.Remove(descriptor);
        }

        public static void Replace<TService, TImplementation>(this IServiceCollection services,
            ServiceLifetime lifetime)
        {
            services.Unregister<TService>();
            services.Add(new ServiceDescriptor(typeof(TService), typeof(TImplementation), lifetime));
        }

        public static void ReplaceScoped<TService, TImplementation>(this IServiceCollection services)
            where TService : class
            where TImplementation : class, TService
        {
            services.Unregister<TService>();
            services.AddScoped<TService, TImplementation>();
        }

        public static void ReplaceScoped<TService>(this IServiceCollection services,
            Func<IServiceProvider, TService> implementationFactory)
            where TService : class
        {
            services.Unregister<TService>();
            services.AddScoped(implementationFactory);
        }

        public static void ReplaceTransient<TService, TImplementation>(this IServiceCollection services)
            where TService : class
            where TImplementation : class, TService
        {
            services.Unregister<TService>();
            services.AddTransient<TService, TImplementation>();
        }

        public static void ReplaceTransient<TService>(this IServiceCollection services,
            Func<IServiceProvider, TService> implementationFactory)
            where TService : class
        {
            services.Unregister<TService>();
            services.AddTransient(implementationFactory);
        }

        public static void ReplaceSingleton<TService, TImplementation>(this IServiceCollection services)
            where TService : class
            where TImplementation : class, TService
        {
            services.Unregister<TService>();
            services.AddSingleton<TService, TImplementation>();
        }

        public static void RegisterOptions<TOptions>(this IServiceCollection services, IConfiguration configuration)
            where TOptions : class, new()
        {
            var options = new TOptions();
            configuration.Bind(typeof(TOptions).Name, options);

            services.AddSingleton(options);
        }

        public static void RegisterOptions<TOptions>(this IServiceCollection services, IConfiguration configuration,
            string name) where TOptions : class, new()
        {
            var options = new TOptions();
            configuration.Bind(name, options);

            services.AddSingleton(options);
        }
    }
}