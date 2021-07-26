using System;
using System.Collections.Generic;
using System.Linq;
using BuildingBlocks.Core.Domain.Exceptions;
using BuildingBlocks.Core.Exceptions;
using BuildingBlocks.Logging.Serilog;
using BuildingBlocks.Web.Contexts;
using BuildingBlocks.Web.Contexts.Middleware;
using BuildingBlocks.Web.Http.Handlers;
using BuildingBlocks.Web.Middlewares;
using BuildingBlocks.Web.Storage;
using BuildingBlocks.Web.Validation;
using Figgle;
using Hellang.Middleware.ProblemDetails;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace BuildingBlocks.Web.Extensions
{
    public static class ServiceCollectionExtensions
    {
        private const string AppOptionsSectionName = "AppOptions";
        private const string ExecutionContextSection = "ExecutionContextOptions";

        public static IServiceCollection AddWebApi(this IServiceCollection services, IConfiguration configuration,
            string appOptionSection = AppOptionsSectionName, string executionContextSection = ExecutionContextSection)
        {
            services.AddHttpContextAccessor();
            services.AddOptions<ExecutionContextOptions>().Bind(configuration.GetSection(executionContextSection))
                .ValidateDataAnnotations();

            services.AddScoped<ICorrelationContextAccessor, CorrelationContextAccessor>();
            services.AddScoped<IExecutionContextAccessor, ExecutionContextAccessor>();
            services.AddScoped<IExecutionContextFactory, ExecutionContextFactory>();
            services.AddScoped<IHttpTraceId, HttpTraceId>();
            services.AddSingleton<IRequestStorage, RequestStorage>();

            //https://andrewlock.net/handling-web-api-exceptions-with-problemdetails-middleware/
            services.AddProblemDetails(x =>
            {
                // Control when an exception is included
                x.IncludeExceptionDetails = (ctx, ex) =>
                {
                    // Fetch services from HttpContext.RequestServices
                    var env = ctx.RequestServices.GetRequiredService<IHostEnvironment>();
                    return env.IsDevelopment() || env.IsStaging();
                };
                x.Map<InvalidCommandException>(ex => new InvalidCommandProblemDetails(ex));
                x.Map<BusinessRuleValidationException>(ex => new BusinessRuleValidationExceptionProblemDetails(ex));
            });

            var appOptions = configuration.GetSection(appOptionSection).Get<AppOptions>();
            services.AddOptions<AppOptions>().Bind(configuration.GetSection(appOptionSection))
                .ValidateDataAnnotations();

            var disabledModules = new List<string>();
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

            //https://andrewlock.net/using-serilog-aspnetcore-in-asp-net-core-3-logging-mvc-propertis-with-serilog/
            services.AddControllers(options => options.Filters.Add<SerilogLoggingActionFilter>())
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
                    //http://danielwertheim.se/json-net-private-setters/
                    x.SerializerSettings.ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor;
                    x.SerializerSettings.ContractResolver = new ContractResolverWithPrivates();
                    x.SerializerSettings.Converters = new List<JsonConverter>
                    {
                        new StringEnumConverter(new CamelCaseNamingStrategy())
                    };
                });

            services
                .AddScoped<ErrorHandlerMiddleware>()
                .AddScoped<UserMiddleware>()
                .AddScoped<ResourceIdGeneratorMiddleware>()
                .AddScoped<LogContextMiddleware>()
                .AddScoped<AddCorrelationContextToResponseMiddleware>();

            services.AddHttpClient();
            services.AddScoped<CorrelationContextMessageHandler>();
            services.ConfigureAll<HttpClientFactoryOptions>(options =>
            {
                options.HttpMessageHandlerBuilderActions.Add(builder =>
                {
                    builder.AdditionalHandlers.Add(builder.Services
                        .GetRequiredService<CorrelationContextMessageHandler>());
                });
            });

            if (!appOptions.DisplayBanner || string.IsNullOrWhiteSpace(appOptions.Name)) return services;

            var version = appOptions.DisplayVersion ? $" {appOptions.Version}" : string.Empty;
            Console.WriteLine(FiggleFonts.Doom.Render($"{appOptions.Name}{version}"));

            return services;
        }
    }
}