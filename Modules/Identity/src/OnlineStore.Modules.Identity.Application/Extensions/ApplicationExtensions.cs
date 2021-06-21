using System.Reflection;
using Common.Domain;
using Common.Domain.Dispatching;
using Microsoft.Extensions.DependencyInjection;
using OnlineStore.Modules.Identity.Application.Features.Permissions.Services;
using OnlineStore.Modules.Identity.Application.Features.Users;
using OnlineStore.Modules.Identity.Application.Features.Users.Contracts;

namespace OnlineStore.Modules.Identity.Application.Extensions
{
    public static class ApplicationExtensions
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddAutoMapper(Assembly.GetExecutingAssembly());

            services.AddSingleton<IUserDomainToIntegrationEventMapper, UserDomainToIntegrationEventMapper>();
            services.AddSingleton<IDomainNotificationsMapper, UserDomainNotificationMapper>();
            return services;
        }
    }
}