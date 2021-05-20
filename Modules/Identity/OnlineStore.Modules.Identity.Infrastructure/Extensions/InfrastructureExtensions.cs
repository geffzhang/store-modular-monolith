using Common.Caching;
using Common.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OnlineStore.Modules.Identity.Application.Users.Services;
using OnlineStore.Modules.Identity.Domain.Users;
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
            services.AddScoped<IUserEditable, UserEditable>();

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