using System.Collections.Generic;
using System.Linq;
using Common.Core.Extensions;
using Common.Core.Modules;
using Common.Diagnostics;
using Common.Logging.Serilog;
using Common.Swagger;
using Common.Validations;
using Common.Web;
using Common.Web.Extensions;
using Messaging.Transport.InMemory;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.FeatureManagement;
using Serilog;
using Shopping.API.Extensions;

namespace OnlineStore.API
{
    public class Startup
    {
        private IConfiguration Configuration { get; }
        private Cors Cors { get; }
        private AppOptions AppOptions { get; }
        private IList<IModule> Modules { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            Cors = Configuration.GetSection("CORS").Get<Cors>();
            AppOptions = Configuration.GetSection("AppOptions").Get<AppOptions>();

            var assemblies = ModuleLoader.LoadAssemblies(configuration);
            Modules = ModuleLoader.LoadModules(assemblies);
            Modules.ToList().ForEach(x => x.Init(Configuration));
        }


        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddWebApi(Configuration);
            services.AddCore(Configuration);
            services.AddOTel(Configuration);
            services.AddSwagger(typeof(ApiRoot).Assembly);
            services.AddCustomValidators();
            services.AddCors(Cors);
            services.AddHealthCheck(Configuration, AppOptions.ApiAddress);
            services.AddVersioning();
            services.AddCaching(Configuration);
            services.AddInMemoryMessaging(Configuration, "messaging");
            services.AddFeatureManagement();

            foreach (var module in Modules)
            {
                module.ConfigureServices(services);
            }
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMiddleware<StackifyMiddleware.RequestTracerMiddleware>();
            //https://andrewlock.net/using-serilog-aspnetcore-in-asp-net-core-3-logging-the-selected-endpoint-name-with-serilog/
            if (env.IsTest() == false)
                app.UseSerilogRequestLogging(opts =>
                    opts.EnrichDiagnosticContext = RequestLoggingHelper.EnrichFromRequest);
            app.UseCustomExceptionHandler();

            app.UseStaticFiles();
            app.UserWebApi(env);
            app.UseRouting();

            app.UseAuthorization();
            app.UseHealthCheck();

            app.UseCors("Open");
            app.UseCors(Cors.AllowAnyOrigin ? "AllowAnyOrigin" : "AllowedOrigins");

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapGet("/", context => context.Response.WriteAsync("Online Store API!"));
            });

            var provider = app.ApplicationServices.GetService<IApiVersionDescriptionProvider>();
            app.UseSwagger(provider);

            foreach (var module in Modules)
            {
                module.Configure(app, env);
            }
        }
    }
}