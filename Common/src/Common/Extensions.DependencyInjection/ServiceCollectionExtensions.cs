using System;
using System.Collections.Generic;
using System.Reflection;
using Common.Contexts;
using Common.Exceptions;
using Common.Generators;
using Common.Logging;
using Common.Messaging;
using Common.Messaging.Commands;
using Common.Messaging.Events;
using Common.Messaging.Queries;
using Common.Modules;
using Common.Services;
using Common.Storage;
using Common.Web;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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
            if (string.IsNullOrWhiteSpace(sectionName))
            {
                sectionName = AppSectionName;
            }

            services.TryDecorate(typeof(ICommandHandler<>), typeof(UnitOfWorkCommandHandlerDecorator<>));
            services.TryDecorate(typeof(IEventHandler<>), typeof(UnitOfWorkEventHandlerDecorator<>));
            services.TryDecorate(typeof(IQueryHandler<,>), typeof(LoggingQueryHandlerDecorator<,>));
            services.TryDecorate(typeof(ICommandHandler<>), typeof(LoggingCommandHandlerDecorator<>));
            services.TryDecorate(typeof(IEventHandler<>), typeof(LoggingEventHandlerDecorator<>));

            CommandHandlersFromAssemblies(services);
            EventHandlersFromAssemblies(services);
            QueryHandlersFromAssemblies(services);


            services.AddSingleton<IQueryProcessor, QueryProcessor>();
            services.AddSingleton<ICommandProcessor, CommandProcessor>();

            services
                .AddSingleton<IDateTimeProvider, DateTimeProvider>()
                .AddSingleton<IRequestStorage, RequestStorage>()
                .AddSingleton<IRng, Rng>()
                .AddSingleton<IIdGenerator, Common.Generators.IdGenerator>()
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
                    x.SerializerSettings.Converters = new List<Newtonsoft.Json.JsonConverter>
                    {
                        new StringEnumConverter(new CamelCaseNamingStrategy())
                    };
                }); ;



            var appOptions = services.GetOptions<AppOptions>(sectionName);
            services.AddSingleton(appOptions);
            if (!appOptions.DisplayBanner || string.IsNullOrWhiteSpace(appOptions.Name))
            {
                return services;
            }

            var version = appOptions.DisplayVersion ? $" {appOptions.Version}" : string.Empty;
            Console.WriteLine(Figgle.FiggleFonts.Doom.Render($"{appOptions.Name}{version}"));

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
    }
}