using BuildingBlocks.Core.Scheduling;
using BuildingBlocks.Messaging.Outbox.EFCore;
using BuildingBlocks.Messaging.Scheduling.Hangfire.MessagesScheduler;
using BuildingBlocks.Persistence.MSSQL;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OnlineStore.Modules.Identity.Application.Permissions.Services;
using OnlineStore.Modules.Identity.Application.System;
using OnlineStore.Modules.Identity.Infrastructure.Aggregates.System;
using OnlineStore.Modules.Identity.Infrastructure.Aggregates.Users.Services;

namespace OnlineStore.Modules.Identity.Infrastructure.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddMssqlPersistence<IdentityDbContext>(configuration,
                configurator: s => { s.AddRepository(typeof(Repository<>)); },
                optionBuilder: options =>
                {
                    // options.UseTriggers(triggerOptions => {
                    //     triggerOptions.AddTrigger<AuditTrigger>();
                    // });
                });

            services.AddEntityFrameworkOutbox<IdentityDbContext>(configuration);
            AddScopeServices(services);
            AddTransientServices(services);
            AddSingletonServices(services);

            services.AddIdentityServices(configuration, (option) => { });

            return services;
        }


        private static void AddScopeServices(IServiceCollection services)
        {
            services.AddScoped<IDataSeeder, DataSeeder>();
        }

        private static void AddTransientServices(IServiceCollection services)
        {
        }

        private static void AddSingletonServices(IServiceCollection services)
        {
            services.AddSingleton<IMessagesScheduler, HangfireMessagesScheduler>();
            services.AddSingleton<IPermissionService, PermissionService>();
        }
    }
}