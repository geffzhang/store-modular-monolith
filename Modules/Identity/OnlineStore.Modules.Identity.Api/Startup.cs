using System;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.FeatureManagement;
using Microsoft.OpenApi.Models;
using OnlineStore.Modules.Identity.Application;
using OnlineStore.Modules.Identity.Domain.Configurations.Settings;
using OnlineStore.Modules.Identity.Infrastructure.Extensions;

namespace OnlineStore.Modules.Identity.Api
{
    public class Startup
    {
        private AppSettings AppSettings { get; set; }
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

            AppSettings = new AppSettings();
            Configuration.Bind(AppSettings);
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddInfrastructure(Configuration);
            services.AddApplication();

            services.AddCors(options =>
            {
                options.AddPolicy("AllowedOrigins", builder => builder
                    .WithOrigins(AppSettings.Cors.AllowedOrigins)
                    .AllowAnyMethod()
                    .AllowAnyHeader());

                options.AddPolicy("AllowAnyOrigin", builder => builder
                    .AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader());

                options.AddPolicy("CustomPolicy", builder => builder
                    .AllowAnyOrigin()
                    .WithMethods("Get")
                    .WithHeaders("Content-Type"));
            });


            services.AddHealthChecks()
                .AddDbContextCheck<IdentityDbContext>(name: "Identity DB Context", failureStatus: HealthStatus.Degraded)
                .AddUrlGroup(new Uri(AppSettings.ApplicationDetail.ContactWebsite), name: "Online Store",
                    failureStatus: HealthStatus.Degraded)
                .AddSqlServer(Configuration.GetConnectionString("OnlineStore"));

            services.AddHealthChecksUI(setupSettings: setup =>
            {
                setup.AddHealthCheckEndpoint("Basic Health Check", $"/healthz");
            }).AddInMemoryStorage();

            services.AddApiVersioning(config =>
            {
                config.DefaultApiVersion = new ApiVersion(1, 0);
                config.AssumeDefaultVersionWhenUnspecified = true;
                config.ReportApiVersions = true;
            });

            services.AddFeatureManagement();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseStaticFiles();
            app.UseInfrastructure(env);
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthorization();

            app.UseHealthChecks("/healthz", new HealthCheckOptions
            {
                Predicate = _ => true,
                ResponseWriter =  UIResponseWriter.WriteHealthCheckUIResponse,
                ResultStatusCodes =
                {
                    [HealthStatus.Healthy] = StatusCodes.Status200OK,
                    [HealthStatus.Degraded] = StatusCodes.Status500InternalServerError,
                    [HealthStatus.Unhealthy] = StatusCodes.Status503ServiceUnavailable,
                },
            }).UseHealthChecksUI(setup =>
            {
                setup.ApiPath = "/healthcheck";
                setup.UIPath = "/healthcheck-ui";
            });



            app.UseCors("Open");

            app.UseCors(AppSettings.Cors.AllowAnyOrigin ? "AllowAnyOrigin" : "AllowedOrigins");

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}