using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using BuildingBlocks.Core.Domain.DomainEventNotifications;
using BuildingBlocks.Core.Domain.DomainEvents;
using BuildingBlocks.Core.Exceptions;
using BuildingBlocks.Core.Mail;
using BuildingBlocks.Core.Messaging;
using BuildingBlocks.Core.Messaging.Serialization;
using BuildingBlocks.Core.Messaging.Serialization.Newtonsoft;
using BuildingBlocks.Core.Messaging.Transport;
using BuildingBlocks.Core.Scheduling;
using BuildingBlocks.Cqrs;
using BuildingBlocks.Cqrs.Events;
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

            services.AddMessaging(assemblies);
            services.AddDomainEvents(assemblies ?? AppDomain.CurrentDomain.GetAssemblies());

            services
                .AddMongoPersistence(configuration)
                .AddSingleton<IExceptionToResponseMapper, ExceptionToResponseMapper>()
                .AddSingleton<IExceptionToMessageMapper, ExceptionToMessageMapper>()
                .AddSingleton<IExceptionCompositionRoot, ExceptionCompositionRoot>()
                .AddSingleton<IExceptionToMessageMapperResolver, ExceptionToMessageMapperResolver>()
                .AddScoped<ICommandProcessor, CommandProcessor>()
                .AddScoped<IMessagesExecutor, MessagesExecutor>()
                .AddScoped<IExecutionContextAccessor, ExecutionContextAccessor>();

            services.AddScoped(typeof(IMessageMiddleware<>), typeof(MessagingLoggingMiddleware<>));
            services.AddScoped(typeof(IMessageMiddleware<>), typeof(RetryMessageHandlingMiddleware<>));
            services.AddScoped(typeof(IRequestMiddleware<,>), typeof(CqrsRequestLoggingMiddleware<,>));
            services.AddScoped(typeof(IRequestMiddleware<,>), typeof(RetryRequestHandlingMiddleware<,>));

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
            services.AddSingleton<IDomainEventDispatcher, DomainEventDispatcher>();
            services.AddSingleton<IDomainEventNotificationDispatcher, DomainEventNotificationDispatcher>();

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

        private static IServiceCollection AddMessaging(this IServiceCollection services, IList<Assembly> scanAssemblies)
        {
            var assemblies = scanAssemblies ?? AppDomain.CurrentDomain.GetAssemblies();

            services.AddScoped<IMessageProcessor, MessageProcessor>();

            ScanForMessagingHandlers(services, assemblies);
            AddMessagingMiddlewares(services, assemblies);

            return services;
        }

        private static IServiceCollection AddMessagingMiddlewares(IServiceCollection services,
            IList<Assembly> assembliesToScan)
        {
            services.Scan(s => s.FromAssemblies(assembliesToScan)
                .AddClasses(c => c.AssignableTo(typeof(IMessageMiddleware<>)))
                .AsImplementedInterfaces()
                .WithScopedLifetime());

            return services;
        }

        private static IServiceCollection ScanForMessagingHandlers(IServiceCollection services,
            IList<Assembly> assembliesToScan)
        {
            services.Scan(s => s.FromAssemblies(assembliesToScan).AddClasses(c => c
                    .AssignableTo(typeof(IMessageHandler<>))
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