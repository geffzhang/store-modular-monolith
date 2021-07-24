using BuildingBlocks.Core.Domain.Dispatching;
using Microsoft.Extensions.DependencyInjection;
using OnlineStore.Modules.Identity.Application.Users;
using OnlineStore.Modules.Identity.Application.Users.Contracts;

namespace OnlineStore.Modules.Identity.Application.Extensions
{
    public static class ApplicationExtensions
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddAutoMapper(typeof(ApplicationRoot).Assembly);
            services.AddSingleton<IUserDomainToIntegrationEventMapper, UserDomainToIntegrationEventMapper>();
            services.AddSingleton<IDomainNotificationsMapper, UserDomainNotificationMapper>();

            return services;
        }
    }
}