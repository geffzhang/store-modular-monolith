using System;
using System.Collections.Generic;
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
using Common.Web.Middlewares;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using IMailService = Common.Mail.IMailService;

[assembly: InternalsVisibleTo("OnlineStore.API")]
[assembly: InternalsVisibleTo("OnlineStore.Tests.Benchmarks")]
[assembly: InternalsVisibleTo("OnlineStore.Common.Tests.Integration")]

namespace Common.Dependency
{
    public static class Extensions
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
                .AddSingleton<IContextFactory, ContextFactory>()
                .AddScoped(ctx => ctx.GetRequiredService<IContextFactory>().Create())
                .AddSingleton<IHttpContextAccessor, HttpContextAccessor>()
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
                .AddClasses(c => c.AssignableTo(typeof(IEventHandler<>)))
                .AsImplementedInterfaces()
                .WithScopedLifetime());

            services.Scan(s => s.FromAssemblies(assemblies)
                .AddClasses(c => c.AssignableTo(typeof(IDomainEventNotificationHandler<>)))
                .AsImplementedInterfaces()
                .WithScopedLifetime());

            services.Scan(s => s.FromAssemblies(assemblies)
                .AddClasses(c => c.AssignableTo(typeof(IDomainEventNotification<>)))
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
                .AddClasses(c => c.AssignableTo(typeof(IQueryHandler<,>)))
                .AsImplementedInterfaces()
                .WithScopedLifetime());

            return services;
        }


        private static IServiceCollection AddIntegrationEvent(this IServiceCollection services, IList<Assembly> assemblies)
        {
            services.AddSingleton<IIntegrationEventDispatcher, IntegrationEventDispatcher>();

            services.Scan(s => s.FromAssemblies(assemblies)
                .AddClasses(c => c.AssignableTo(typeof(IEventHandler<>))
                    .WithoutAttribute(typeof(DecoratorAttribute)))
                .AsImplementedInterfaces()
                .WithScopedLifetime());

            return services;
        }


        public static IServiceCollection AddMessaging(this IServiceCollection services, IConfiguration configuration, string sectionName)
        {
            var messagingOptions = configuration.GetSection(sectionName).Get<MessagingOptions>();
            services.AddOptions<MessagingOptions>().Bind(configuration.GetSection(sectionName)).ValidateDataAnnotations();

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
    }
}