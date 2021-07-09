using Common.Web;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.FeatureManagement;
using OnlineStore.Modules.Identity.Api.Extensions;
using OnlineStore.Modules.Identity.Application.Extensions;
using OnlineStore.Modules.Identity.Infrastructure.Extensions;
using Common.Web.Extensions;

namespace OnlineStore.Modules.Identity.Api
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        private Cors Cors { get; }
        private AppOptions AppOptions { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            Cors = Configuration.GetSection("CORS").Get<Cors>();
            AppOptions = Configuration.GetSection("AppOptions").Get<AppOptions>();
        }


        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddWebApi(Configuration);
            services.AddSwaggerDocumentation();
            services.AddInfrastructure(Configuration);
            services.AddApplication();
            services.AddCors(Cors);
            services.AddHealthCheck(Configuration, AppOptions.ApiAddress);
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
            app.UserWebApi(env);
            app.UseInfrastructure(env);
            app.UseRouting();


            app.UseAuthorization();
            app.UseSwaggerDocumentation();
            app.UseHealthCheck();

            app.UseCors("Open");
            app.UseCors(Cors.AllowAnyOrigin ? "AllowAnyOrigin" : "AllowedOrigins");

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}