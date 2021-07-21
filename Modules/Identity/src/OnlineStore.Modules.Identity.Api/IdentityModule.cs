using Common.Core.Modules;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using OnlineStore.Modules.Identity.Application.Extensions;
using OnlineStore.Modules.Identity.Infrastructure.Extensions;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;

namespace OnlineStore.Modules.Identity.Api
{
    public class IdentityModule : IModule
    {
        public string Name => "Identity";
        public string Path => "";
        public IConfiguration Configuration { get; private set; } = null!;

        public void Init(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddInfrastructure(Configuration);
            services.AddApplication();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment environment)
        {
            app.UseInfrastructure(environment);
        }

        public void ConfigureEndpoints(IEndpointRouteBuilder endpoints)
        {
        }
    }
}