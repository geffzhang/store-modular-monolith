using System;
using System.Collections.Generic;
using System.Linq;
using Common.Web.Middlewares;
using Figgle;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace Common.Web
{
    public static class Extensions
    {
        private const string AppOptionsSectionName = "AppOptions";

        public static IServiceCollection AddWebApi(this IServiceCollection services, IConfiguration configuration,
            string appOptionSection = AppOptionsSectionName)
        {
            var appOptions = configuration.GetSection(appOptionSection).Get<AppOptions>();
            services.AddOptions<AppOptions>().Bind(configuration.GetSection(appOptionSection)).ValidateDataAnnotations();
            
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

            services.AddControllers()
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

            services.AddScoped<ErrorHandlerMiddleware>()
                .AddScoped<CorrelationMiddleware>()
                .AddScoped<UserMiddleware>();
            
            if (!appOptions.DisplayBanner || string.IsNullOrWhiteSpace(appOptions.Name)) return services;

            var version = appOptions.DisplayVersion ? $" {appOptions.Version}" : string.Empty;
            Console.WriteLine(FiggleFonts.Doom.Render($"{appOptions.Name}{version}"));

            return services;
        }


        public static IApplicationBuilder UserWebApi(this IApplicationBuilder app)
        {
            app.UseMiddleware<UserMiddleware>();
            app.UseMiddleware<CorrelationMiddleware>();
            app.UseMiddleware<ErrorHandlerMiddleware>();

            return app;
        }
    }
}