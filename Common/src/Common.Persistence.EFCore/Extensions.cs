using Microsoft.Extensions.DependencyInjection;

namespace Common.Persistence.EFCore
{
    public static class Extensions
    {
        public static IServiceCollection AddEfCore(this IServiceCollection services)
        {
            services
                .AddTransient<IUnitOfWork, UnitOfWork>();

            return services;
        }
    }
}