using System.Reflection;
using Common.Caching;
using Common.Extensions.DependencyInjection;
using Common.Messaging.Scheduling.Hangfire.MessagesScheduler;
using Common.Scheduling;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OnlineStore.Modules.Identity.Application.Features.Users.Services;
using OnlineStore.Modules.Identity.Domain.Configurations.Settings;
using OnlineStore.Modules.Identity.Domain.Users.Services;
using OnlineStore.Modules.Identity.Infrastructure.Domain.System;
using OnlineStore.Modules.Identity.Infrastructure.Middlewares;
using Serilog;

namespace OnlineStore.Modules.Identity.Infrastructure.Extensions
{
    public static class InfrastructureExtensions
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services,
            IConfiguration configuration)
        {
            services.Configure<MailSettings>(configuration.GetSection("MailSettings"));
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            services.AddCommon();

            AddScopeServices(services);
            AddTransientServices(services);
            AddSingletonServices(services);

            services.AddIdentityServices(configuration, (option) => { });
            services.AddCaching(configuration);

            return services;
        }

        private static void AddScopeServices(IServiceCollection services)
        {
            services.AddScoped<DataSeeder>();
            services.AddScoped<IUserEditable, UserEditable>();
        }

        private static void AddTransientServices(IServiceCollection services)
        {
        }
        
        private static void AddSingletonServices(IServiceCollection services)
        {
            services.AddSingleton<IMessagesScheduler, HangfireMessagesScheduler>();
        }
        
        public static IApplicationBuilder UseInfrastructure(this IApplicationBuilder app,
            IWebHostEnvironment env)
        {
            // packages exists in Common project
            app.UseMiddleware<StackifyMiddleware.RequestTracerMiddleware>();
            app.UseSerilogRequestLogging();

            app.UseCustomExceptionHandler();
            app.UseIdentityDataSeederAsync().GetAwaiter().GetResult();

            return app;
        }
    }
}