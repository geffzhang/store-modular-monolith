using BuildingBlocks.Core.Domain.DomainEventNotifications;
using Microsoft.Extensions.DependencyInjection;
using OnlineStore.Modules.Identity.Application.Authentication.Services;
using OnlineStore.Modules.Identity.Application.Users;
using OnlineStore.Modules.Identity.Application.Users.Contracts;

namespace OnlineStore.Modules.Identity.Application.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddAutoMapper(typeof(ApplicationRoot).Assembly);
            services.AddSingleton<IUserDomainEventsToIntegrationEventsMapper, UserDomainEventsToIntegrationEventsMapper>();
            services.AddSingleton<IDomainNotificationsMapper, UserDomainNotificationMapper>();
            services.AddSingleton<ITokenStorageService, TokenStorageService>();

            return services;
        }
    }
}