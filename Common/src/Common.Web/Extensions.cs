using System;
using System.Collections.Generic;
using System.Linq;
using Common.Domain.Exceptions;
using Common.Exceptions;
using Common.Web.Contexts;
using Common.Web.Http.Handlers;
using Common.Web.Middlewares;
using Common.Web.Validation;
using Figgle;
using Hellang.Middleware.ProblemDetails;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace Common.Web
{
    public static class Extensions
    {
        private const string AppOptionsSectionName = "AppOptions";
        private const string ExecutionContextSection = "ExecutionContextOptions";
        private static ExecutionContextOptions _contextOptions;

        public static IServiceCollection AddWebApi(this IServiceCollection services, IConfiguration configuration,
            string appOptionSection = AppOptionsSectionName, string executionContextSection = ExecutionContextSection)
        {
            services.AddHttpContextAccessor();
            services.AddOptions<ExecutionContextOptions>().Bind(configuration.GetSection(executionContextSection))
                .ValidateDataAnnotations();
            _contextOptions = configuration.GetSection(executionContextSection).Get<ExecutionContextOptions>();

            services.AddScoped<ICorrelationContextAccessor, CorrelationContextAccessor>();
            services.AddScoped<IExecutionContextAccessor, ExecutionContextAccessor>();
            services.AddScoped<IExecutionContextFactory, ExecutionContextFactory>();

            services.AddProblemDetails(x =>
            {
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
            services.AddControllers(options => options.Filters.Add<Common.Logging.Serilog.SerilogLoggingActionFilter>())
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

            //https://dominikjeske.github.io/configure-httpclients/
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


        public static IApplicationBuilder UserWebApi(this IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseMiddleware<UserMiddleware>();
            app.UseMiddleware<ErrorHandlerMiddleware>();
            app.UseMiddleware<ResourceIdGeneratorMiddleware>();
            app.UseMiddleware<LogContextMiddleware>();
            app.UseMiddleware<AddCorrelationContextToResponseMiddleware>();

            if (env.IsDevelopment())
            {
                app.UseProblemDetails();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            return app;
        }


        public static void SetResourceId(this JObject jObject, string id)
        {
            var idProperty = jObject.Property("id", StringComparison.InvariantCultureIgnoreCase);
            if (idProperty is null)
            {
                jObject.Add("id", id);
                return;
            }

            idProperty.Value = id;
        }

        public static string GetResourceIdFoRequest(this HttpContext context)
            => context.Items.TryGetValue(_contextOptions?.ResourceIdHeaderKey ?? "resource-id", out var id)
                ? id as string
                : string.Empty;

        public static void SetResourceIdFoRequest(this HttpContext context, string id)
            => context.Items.TryAdd(_contextOptions?.ResourceIdHeaderKey ?? "resource-id", id);

        public static void SetOperationHeader(this HttpResponse response, string id)
            => response.Headers.Add(_contextOptions?.OperationHeaderKey ?? "x-operation", $"operations/{id}");
    }
}