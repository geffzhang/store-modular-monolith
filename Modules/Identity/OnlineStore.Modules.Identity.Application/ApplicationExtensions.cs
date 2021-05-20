using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace OnlineStore.Modules.Identity.Application
{
    public static class ApplicationExtensions
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            return services;
        }
    }
}