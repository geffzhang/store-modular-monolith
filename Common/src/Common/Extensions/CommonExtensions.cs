using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Common.Dependency.ServiceLocator;
using Common.Domain;
using Common.Domain.Dispatching;
using Common.Exceptions;
using Common.Mail;
using Common.Messaging;
using Common.Messaging.Commands;
using Common.Messaging.Events;
using Common.Messaging.Queries;
using Common.Messaging.Serialization;
using Common.Messaging.Serialization.Newtonsoft;
using Common.Messaging.Transport;
using Common.Messaging.Transport.InMemory;
using Common.Persistence.Mongo;
using Common.Scheduling;
using Common.Web.Contexts;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using IMailService = Common.Mail.IMailService;

[assembly: InternalsVisibleTo("OnlineStore.API")]
[assembly: InternalsVisibleTo("OnlineStore.Tests.Benchmarks")]
[assembly: InternalsVisibleTo("OnlineStore.Common.Tests.Integration")]

namespace Common.Extensions
{
    public static class CommonExtensions
    {
        public static IServiceCollection AddCommon(this IServiceCollection services, IConfiguration configuration,
            IList<Assembly> assemblies = null
            // IList<IModule> modules = null,
        )
        {
            services.Configure<MailConfiguration>(configuration.GetSection("MailConfiguration"));
            services.AddSingleton<IMailService, SmtpMailService>();

            services.AddNewtonsoftMessageSerializer(options =>
            {
                options.Converters = new List<JsonConverter> {new StringEnumConverter(new CamelCaseNamingStrategy())};
                //options.UnSupportedTypes.Add<Test>();
            });

            //Adding Auto Mapper
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());


            services.AddCommand(assemblies ?? AppDomain.CurrentDomain.GetAssemblies());
            services.AddQuery(assemblies ?? AppDomain.CurrentDomain.GetAssemblies());
            services.AddDomainEvents(assemblies ?? AppDomain.CurrentDomain.GetAssemblies());
            services.AddIntegrationEvent(assemblies ?? AppDomain.CurrentDomain.GetAssemblies());

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
                .AddSingleton<IMessageChannel, MessageChannel>()
                .AddSingleton<IExecutionContextAccessor, ExecutionContextAccessor>()
                .AddSingleton<IDependencyResolver, DefaultDependencyResolver>()
                .AddMessaging(configuration, "messaging");


            services.TryDecorate(typeof(ICommandHandler<>), typeof(DomainEventsDispatcherCommandHandlerDecorator<>));
            services.TryDecorate(typeof(IEventHandler<>), typeof(DomainEventsDispatcherEventHandlerDecorator<>));

            // services.TryDecorate(typeof(IIntegrationEventHandler<>), typeof(LoggingIntegrationEventHandlerDecorator<>));
            // services.TryDecorate(typeof(IQueryHandler<,>), typeof(LoggingQueryHandlerDecorator<,>));
            // services.TryDecorate(typeof(ICommandHandler<>), typeof(LoggingCommandHandlerDecorator<>));
            // services.TryDecorate(typeof(IEventHandler<>), typeof(LoggingEventHandlerDecorator<>));
            // services.TryDecorate(typeof(IEventHandler<>), typeof(LoggingDomainEventHandlerDecorator<>));
            // services.TryDecorate(typeof(IEventHandler<>), typeof(LoggingNotificationEventHandlerDecorator<>));

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


        private static IServiceCollection AddIntegrationEvent(this IServiceCollection services,
            IList<Assembly> assemblies)
        {
            services.AddSingleton<IIntegrationEventDispatcher, IntegrationEventDispatcher>();

            services.Scan(s => s.FromAssemblies(assemblies)
                .AddClasses(c => c.AssignableTo(typeof(IEventHandler<>))
                    .WithoutAttribute(typeof(DecoratorAttribute)))
                .AsImplementedInterfaces()
                .WithScopedLifetime());

            return services;
        }


        public static IServiceCollection AddMessaging(this IServiceCollection services, IConfiguration configuration,
            string sectionName)
        {
            var messagingOptions = configuration.GetSection(sectionName).Get<MessagingOptions>();
            services.AddOptions<MessagingOptions>().Bind(configuration.GetSection(sectionName))
                .ValidateDataAnnotations();

            services
                .AddSingleton<IAsyncMessageDispatcher, InMemoryAsyncMessageDispatcher>()
                .AddScoped<ITransport, InMemoryMessageBroker>();

            // Adding background service
            if (messagingOptions is {UseBackgroundDispatcher: true})
            {
                services.AddHostedService<InMemoryBackgroundDispatcher>();
            }

            return services;
        }

        public static IApplicationBuilder ValidateContracts(this IApplicationBuilder app)
        {
            var contractRegistry = app.ApplicationServices.GetRequiredService<IContractRegistry>();
            contractRegistry.Validate();

            return app;
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