using System;
using System.Threading.Tasks;
using Common.Core;
using Common.Core.Scheduling;
using Common.Messaging.Outbox.EFCore;
using Common.Messaging.Scheduling.Hangfire.MessagesScheduler;
using Common.Persistence.MSSQL;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OnlineStore.Modules.Identity.Application.Features.Permissions.Services;
using OnlineStore.Modules.Identity.Application.Features.Users.Services;
using OnlineStore.Modules.Identity.Domain.Configurations.Settings;
using OnlineStore.Modules.Identity.Domain.Users.Services;
using OnlineStore.Modules.Identity.Infrastructure.Domain.Permissions;
using OnlineStore.Modules.Identity.Infrastructure.Domain.System;
using Common.Web.Extensions;
using Microsoft.EntityFrameworkCore;
using OnlineStore.Modules.Identity.Application.Features.System;

namespace OnlineStore.Modules.Identity.Infrastructure.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services,
            IConfiguration configuration)
        {
            services.Configure<MailSettings>(configuration.GetSection("MailSettings"));

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
            services.AddScoped<IUserEditable, UserEditable>();
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