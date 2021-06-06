using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Common.Dependency.ServiceLocator;
using Common.Domain;
using Common.Domain.Dispatching;
using Common.Exceptions;
using Common.Logging.Serilog;
using Common.Mail;
using Common.Messaging;
using Common.Messaging.Commands;
using Common.Messaging.Events;
using Common.Messaging.Inbox;
using Common.Messaging.Inbox.Mongo;
using Common.Messaging.Outbox;
using Common.Messaging.Outbox.Mongo;
using Common.Messaging.Queries;
using Common.Messaging.Serialization;
using Common.Messaging.Serialization.Newtonsoft;
using Common.Messaging.Transport;
using Common.Messaging.Transport.InMemory;
using Common.Modules;
using Common.Persistence.Mongo;
using Common.Redis;
using Common.Scheduling;
using Common.Storage;
using Common.Web;
using Common.Web.Contexts;
using Common.Web.Middlewares;
using Figgle;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using IMailService = Common.Mail.IMailService;

[assembly: InternalsVisibleTo("OnlineStore.API")]
[assembly: InternalsVisibleTo("OnlineStore.Tests.Benchmarks")]
[assembly: InternalsVisibleTo("OnlineStore.Common.Tests.Integration")]

namespace Common.Extensions.DependencyInjection
{
    public static class Extensions
    {
        private const string CorsPolicy = "cors";
        private const string AppSectionName = "app";

        public static IServiceCollection AddCommon(this IServiceCollection services,
            IList<Assembly> assemblies = null,
            IList<IModule> modules = null,
            string sectionName = AppSectionName)
        {
            if (string.IsNullOrWhiteSpace(sectionName)) sectionName = AppSectionName;

            var disabledModules = new List<string>();
            IConfiguration configuration;
            using (var serviceProvider = services.BuildServiceProvider())
            {
                configuration = serviceProvider.GetRequiredService<IConfiguration>();
                foreach (var (key, value) in configuration.AsEnumerable())
                {
                    if (!key.Contains(":module:enabled"))
                    {
                        continue;
                    }

                    if (!bool.Parse(value))
                    {
                        disabledModules.Add(key.Split(":")[0]);
                    }
                }
            }


            services.Configure<MailConfiguration>(configuration.GetSection("MailConfiguration"));
            services.AddSingleton<IMailService, SmtpMailService>();

            services.AddNewtonsoftMessageSerializer(options =>
            {
                options.Converters = new List<JsonConverter> {new StringEnumConverter(new CamelCaseNamingStrategy())};
                //options.UnSupportedTypes.Add<Test>();
            });

            //Adding Auto Mapper
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            services.TryDecorate(typeof(ICommandHandler<>), typeof(DomainEventsDispatcherCommandHandlerDecorator<>));
            services.TryDecorate(typeof(IEventHandler<>), typeof(DomainEventsDispatcherEventHandlerDecorator<>));

            services.TryDecorate(typeof(IIntegrationEventHandler<>), typeof(LoggingIntegrationEventHandlerDecorator<>));
            services.TryDecorate(typeof(IQueryHandler<,>), typeof(LoggingQueryHandlerDecorator<,>));
            services.TryDecorate(typeof(ICommandHandler<>), typeof(LoggingCommandHandlerDecorator<>));
            services.TryDecorate(typeof(IEventHandler<>), typeof(LoggingEventHandlerDecorator<>));
            services.TryDecorate(typeof(IEventHandler<>), typeof(LoggingDomainEventHandlerDecorator<>));
            services.TryDecorate(typeof(IEventHandler<>), typeof(LoggingNotificationEventHandlerDecorator<>));

            services.AddCommand(assemblies ?? AppDomain.CurrentDomain.GetAssemblies());
            services.AddQuery(assemblies ?? AppDomain.CurrentDomain.GetAssemblies());
            services.AddDomainEvents(assemblies ?? AppDomain.CurrentDomain.GetAssemblies());
            services.AddIntegrationEvent(assemblies ?? AppDomain.CurrentDomain.GetAssemblies());

            services
                .AddSingleton<IRequestStorage, RequestStorage>()
                .AddRedis()
                .AddMongoPersistence()
                .AddModuleInfo(modules)
                .AddModuleRequests(assemblies ?? AppDomain.CurrentDomain.GetAssemblies())
                .AddScoped<ErrorHandlerMiddleware>()
                .AddScoped<UserMiddleware>()
                .AddSingleton<IExceptionToResponseMapper, ExceptionToResponseMapper>()
                .AddSingleton<IExceptionToMessageMapper, ExceptionToMessageMapper>()
                .AddSingleton<IExceptionCompositionRoot, ExceptionCompositionRoot>()
                .AddSingleton<IExceptionToMessageMapperResolver, ExceptionToMessageMapperResolver>()
                .AddSingleton<ICommandProcessor, CommandProcessor>()
                .AddScoped<IQueryProcessor, QueryProcessor>()
                .AddScoped<IMessagesExecutor, MessagesExecutor>()
                .AddSingleton<IMessageChannel, MessageChannel>()
                .AddSingleton<IContextFactory, ContextFactory>()
                .AddScoped(ctx => ctx.GetRequiredService<IContextFactory>().Create())
                .AddSingleton<IHttpContextAccessor, HttpContextAccessor>()
                .AddSingleton<IExecutionContextAccessor, ExecutionContextAccessor>()
                .AddSingleton<IDependencyResolver, DefaultDependencyResolver>()
                .AddCors(cors =>
                {
                    cors.AddPolicy(CorsPolicy, x =>
                    {
                        x.WithOrigins("*")
                            .WithMethods("POST", "PUT", "DELETE")
                            .WithHeaders("Content-Type", "Authorization");
                    });
                })
                .AddControllers()
                .ConfigureApplicationPartManager(manager =>
                {
                    var removedParts = new List<ApplicationPart>();
                    foreach (var disabledModule in disabledModules)
                    {
                        var parts = manager.ApplicationParts.Where(x => x.Name.Contains(disabledModule,
                            StringComparison.InvariantCultureIgnoreCase));
                        removedParts.AddRange(parts);
                    }

                    foreach (var part in removedParts)
                    {
                        manager.ApplicationParts.Remove(part);
                    }

                    manager.FeatureProviders.Add(new InternalControllerFeatureProvider());
                })
                .AddNewtonsoftJson(x =>
                {
                    x.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                    x.SerializerSettings.Converters = new List<JsonConverter>
                    {
                        new StringEnumConverter(new CamelCaseNamingStrategy())
                    };
                });

            AddMessaging(services, "messaging");

            var appOptions = services.GetOptions<AppOptions>(sectionName);
            services.AddSingleton(appOptions);
            if (!appOptions.DisplayBanner || string.IsNullOrWhiteSpace(appOptions.Name)) return services;

            var version = appOptions.DisplayVersion ? $" {appOptions.Version}" : string.Empty;
            Console.WriteLine(FiggleFonts.Doom.Render($"{appOptions.Name}{version}"));

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
                .AddClasses(c => c.AssignableTo(typeof(IEventHandler<>)))
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


        public static IServiceCollection AddMessaging(this IServiceCollection services, string sectionName)
        {
            var messagingOptions = services.GetOptions<MessagingOptions>(sectionName);

            services
                .AddSingleton(messagingOptions)
                .AddSingleton<IAsyncMessageDispatcher, InMemoryAsyncMessageDispatcher>()
                .AddScoped<ITransport, InMemoryMessageBroker>();

            // Adding background service
            if (messagingOptions.UseBackgroundDispatcher)
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

        public static IApplicationBuilder UseCommon(this IApplicationBuilder app)
        {
            app.UseCors(CorsPolicy);
            app.UseMiddleware<UserMiddleware>();
            app.UseMiddleware<CorrelationMiddleware>();
            app.UseMiddleware<ErrorHandlerMiddleware>();

            return app;
        }

        public static TModel GetOptions<TModel>(this IServiceCollection services, string settingsSectionName)
            where TModel : new()
        {
            using var serviceProvider = services.BuildServiceProvider();
            var configuration = serviceProvider.GetService<IConfiguration>();
            return configuration.GetOptions<TModel>(settingsSectionName);
        }

        public static TModel GetOptions<TModel>(this IConfiguration configuration, string sectionName)
            where TModel : new()
        {
            var model = new TModel();
            configuration.GetSection(sectionName).Bind(model);
            return model;
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

        public static void ReplaceTransient<TService, TImplementation>(this IServiceCollection services)
            where TService : class
            where TImplementation : class, TService
        {
            services.Unregister<TService>();
            services.AddTransient<TService, TImplementation>();
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