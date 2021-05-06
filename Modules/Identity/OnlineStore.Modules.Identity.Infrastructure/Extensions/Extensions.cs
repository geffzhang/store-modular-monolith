using Common.Caching;
using Common.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OnlineStore.Modules.Identity.Infrastructure.Domain.System;

namespace OnlineStore.Modules.Identity.Infrastructure.Extensions
{
    public static class Extensions
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddCommon();
            services.AddIdentityServices(configuration, (option) => { });

            services.AddScoped<DataSeeder>();
            services.AddCaching(configuration);

            return services;
        }


        public static IApplicationBuilder UseInfrastructure(this IApplicationBuilder app)
        {
            // app.UseDbTriggers();
            app.UseIdentityDataSeederAsync().GetAwaiter().GetResult();

            return app;
        }
    }
}