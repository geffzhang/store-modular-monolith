using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Common.BackgroundServices;
using Common.Contexts;
using Common.Exceptions;
using Common.Generators;
using Common.Logging;
using Common.Messaging;
using Common.Messaging.Commands;
using Common.Messaging.Events;
using Common.Messaging.Queries;
using Common.Messaging.Transport;
using Common.Messaging.Transport.InMemory;
using Common.Services;
using Common.Storage;
using Common.Web;
using Figgle;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace Common.Extensions.DependencyInjection
{
    internal static class ServiceCollectionExtensions
    {
        private const string CorsPolicy = "cors";
        private const string AppSectionName = "app";

        public static IServiceCollection AddInfrastructure(this IServiceCollection services,
            IList<Assembly> assemblies, string sectionName = AppSectionName)
        {
            if (string.IsNullOrWhiteSpace(sectionName)) sectionName = AppSectionName;

            services.TryDecorate(typeof(ICommandHandler<>), typeof(UnitOfWorkCommandHandlerDecorator<>));
            services.TryDecorate(typeof(IIntegrationEventHandler<>), typeof(UnitOfWorkEventHandlerDecorator<>));
            services.TryDecorate(typeof(IQueryHandler<,>), typeof(LoggingQueryHandlerDecorator<,>));
            services.TryDecorate(typeof(ICommandHandler<>), typeof(LoggingCommandHandlerDecorator<>));
            services.TryDecorate(typeof(IIntegrationEventHandler<>), typeof(LoggingIntegrationEventHandlerDecorator<>));

            CommandHandlersFromAssemblies(services);
            EventHandlersFromAssemblies(services);
            QueryHandlersFromAssemblies(services);


            services.AddSingleton<IQueryProcessor, QueryProcessor>();
            services.AddSingleton<ICommandProcessor, CommandProcessor>();

            services
                .AddSingleton<IDateTimeProvider, DateTimeProvider>()
                .AddSingleton<IRequestStorage, RequestStorage>()
                .AddSingleton<IRng, Rng>()
                .AddSingleton<IIdGenerator, IdGenerator>()
                .AddScoped<ErrorHandlerMiddleware>()
                .AddScoped<UserMiddleware>()
                .AddSingleton<IExceptionToResponseMapper, ExceptionToResponseMapper>()
                .AddSingleton<IExceptionToMessageMapper, ExceptionToMessageMapper>()
                .AddSingleton<IExceptionToMessageMapperResolver, ExceptionToMessageMapperResolver>()
                .AddSingleton<ICommandProcessor, CommandProcessor>()
                .AddSingleton<IMessageChannel, MessageChannel>()
                .AddSingleton<IContextFactory, ContextFactory>()
                .AddScoped(ctx => ctx.GetRequiredService<IContextFactory>().Create())
                .AddSingleton<IHttpContextAccessor, HttpContextAccessor>()
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

            AddInMemoryMessageBroker(services, "messaging");


            var appOptions = services.GetOptions<AppOptions>(sectionName);
            services.AddSingleton(appOptions);
            if (!appOptions.DisplayBanner || string.IsNullOrWhiteSpace(appOptions.Name)) return services;

            var version = appOptions.DisplayVersion ? $" {appOptions.Version}" : string.Empty;
            Console.WriteLine(FiggleFonts.Doom.Render($"{appOptions.Name}{version}"));

            return services;
        }

        public static IServiceCollection CommandHandlersFromAssemblies(IServiceCollection services)
        {
            services.Scan(s => s.FromAssemblies(AppDomain.CurrentDomain.GetAssemblies())
                .AddClasses(c => c.AssignableTo(typeof(ICommandHandler<>))
                    .WithoutAttribute(typeof(DecoratorAttribute)))
                .AsImplementedInterfaces()
                .WithScopedLifetime());

            return services;
        }

        public static IServiceCollection QueryHandlersFromAssemblies(IServiceCollection services)
        {
            services.Scan(s => s.FromAssemblies(AppDomain.CurrentDomain.GetAssemblies())
                .AddClasses(c => c.AssignableTo(typeof(IQueryHandler<,>)))
                .AsImplementedInterfaces()
                .WithScopedLifetime());

            return services;
        }


        public static IServiceCollection EventHandlersFromAssemblies(IServiceCollection services)
        {
            services.Scan(s => s.FromAssemblies(AppDomain.CurrentDomain.GetAssemblies())
                .AddClasses(c => c.AssignableTo(typeof(IEventHandler<>))
                    .WithoutAttribute(typeof(DecoratorAttribute)))
                .AsImplementedInterfaces()
                .WithScopedLifetime());

            return services;
        }


        private static IServiceCollection AddInMemoryMessageBroker(IServiceCollection services, string sectionName)
        {
            var messagingOptions = services.GetOptions<MessagingOptions>(sectionName);
            services
                .AddSingleton(messagingOptions)
                .AddScoped<ITransport, InMemoryMessageBroker>();

            if (messagingOptions.UseBackgroundDispatcher) services.AddHostedService<InMemoryBackgroundDispatcher>();

            return services;
        }

        public static IApplicationBuilder ValidateContracts(this IApplicationBuilder app)
        {
            var contractRegistry = app.ApplicationServices.GetRequiredService<IContractRegistry>();
            contractRegistry.Validate();

            return app;
        }

        public static IApplicationBuilder UseInfrastructure(this IApplicationBuilder app)
        {
            app.UseCors(CorsPolicy);
            app.UseMiddleware<UserMiddleware>();
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