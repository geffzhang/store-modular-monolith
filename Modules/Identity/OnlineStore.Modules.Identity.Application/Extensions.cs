using Microsoft.Extensions.DependencyInjection;

namespace OnlineStore.Modules.Identity.Application
{
    public static class Extensions
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            return services;
        }
    }
}