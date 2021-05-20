using System;
using System.Collections.Generic;
using System.Configuration;
using Common.Caching;
using Common.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.FeatureManagement;
using Microsoft.OpenApi.Models;
using OnlineStore.Modules.Identity.Application.Features.Users.Services;
using OnlineStore.Modules.Identity.Domain.Configurations.Settings;
using OnlineStore.Modules.Identity.Domain.Users;
using OnlineStore.Modules.Identity.Domain.Users.Services;
using OnlineStore.Modules.Identity.Infrastructure.Domain.System;
using OnlineStore.Modules.Identity.Infrastructure.Middlewares;
using Serilog;

namespace OnlineStore.Modules.Identity.Infrastructure.Extensions
{
    public static class InfrastructureExtensions
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services,
            IConfiguration configuration)
        {
            var appSettings = new AppSettings();
            configuration.Bind(appSettings);

            services.AddCommon();
            services.AddIdentityServices(configuration, (option) => { });
            services.AddScoped<DataSeeder>();
            services.AddCaching(configuration);
            services.AddScoped<IUserEditable, UserEditable>();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo {Title = "OnlineStore.Modules.Identity.Api", Version = "v1"});
            });

            services.AddSwaggerGen(c =>
            {
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = @"JWT Authorization header using the Bearer scheme. \r\n\r\n 
                      Enter 'Bearer' [space] and then your token in the text input below.
                      \r\n\r\nExample: 'Bearer 12345abcdef'",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                            Scheme = "oauth2",
                            Name = "Bearer",
                            In = ParameterLocation.Header,
                        },
                        new List<string>()
                    }
                });

                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "OnlineStore.Modules.Identity.Api",
                });
            });


            return services;
        }


        public static IApplicationBuilder UseInfrastructure(this IApplicationBuilder app,
            IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "OnlineStore.Modules.Identity.Api v1"));
            }
            // packages exists in Common project
            app.UseMiddleware<StackifyMiddleware.RequestTracerMiddleware>();
            app.UseSerilogRequestLogging();
            
            app.UseCustomExceptionHandler();
            app.UseIdentityDataSeederAsync().GetAwaiter().GetResult();

            return app;
        }
    }
}