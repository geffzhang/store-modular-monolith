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
using OnlineStore.Modules.Identity.Api.Extensions;
using OnlineStore.Modules.Identity.Application;
using OnlineStore.Modules.Identity.Application.Extensions;
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
            services.AddSwaggerDocumentation();

            services.AddInfrastructure(Configuration);
            services.AddApplication();

            services.AddCors(AppSettings);
            services.AddHealthCheck(AppSettings, Configuration);
            services.AddVersioning();
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
            app.UseSwaggerDocumentation();
            app.UseHealthCheck();

            app.UseCors(AppSettings.Cors.AllowAnyOrigin ? "AllowAnyOrigin" : "AllowedOrigins");

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}