using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Common.Extensions.DependencyInjection;
using Common.Messaging.Scheduling.Internal;

namespace OnlineStore.Modules.Users.Infrastructure
{
    public static class Extensions
    {
        private const string Schema = "users-module";

        internal static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            services.AddCommon();
            services.AddInternalMessageScheduler();


            return services;
        }

        internal static IApplicationBuilder UseInfrastructure(this IApplicationBuilder app)
        {
            return app;
        }
    }
}