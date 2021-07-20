using System;
using Common.Core.Extensions;
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
using OnlineStore.Modules.Identity.Infrastructure.Middlewares;
using Serilog;
using Common.Diagnostics;
using Common.Logging.Serilog;
using Common.Web.Extensions;
using Messaging.Transport.InMemory;
using OnlineStore.Modules.Identity.Application.Features.System;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace OnlineStore.Modules.Identity.Infrastructure.Extensions
{
    public static class InfrastructureExtensions
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services,
            IConfiguration configuration)
        {
            services.Configure<MailSettings>(configuration.GetSection("MailSettings"));
            services.AddCommon(configuration);

            AddOTel(services);

            services.AddMssqlPersistence<IdentityDbContext>(configuration,
                configurator: s => { s.AddRepository(typeof(Repository<>)); },
                optionBuilder: options =>
                {
                    // options.UseTriggers(triggerOptions => {
                    //     triggerOptions.AddTrigger<AuditTrigger>();
                    // });
                });
            services.AddInMemoryMessaging(configuration, "messaging");
            services.AddEntityFrameworkOutbox<IdentityDbContext>(configuration);

            AddScopeServices(services);
            AddTransientServices(services);
            AddSingletonServices(services);

            services.AddIdentityServices(configuration, (option) => { });
            services.AddCaching(configuration);

            return services;
        }

        private static void AddOTel(IServiceCollection services)
        {
            services.AddInMemoryMessagingTelemetry();
            services.AddOpenTelemetryTracing(builder => builder
                .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("Identity.Api"))
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation()
                .AddSqlClientInstrumentation(opt => opt.SetDbStatementForText = true)
                .AddZipkinExporter(o => { o.Endpoint = new Uri("http://localhost:9411/api/v2/spans"); })
                .AddJaegerExporter(c =>
                {
                    c.AgentHost = "localhost";
                    c.AgentPort = 6831;
                })
            );
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


        private static void AddCaching(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddEasyCaching(options =>
            {
                options.UseInMemory(configuration, "mem");
                // options.UseRedis(configuration, "redis").WithMessagePack();
                // // combine local and distributed
                // options.UseHybrid(config =>
                //     {
                //         config.TopicName = "easycache-topic";
                //         config.EnableLogging = false;
                //
                //         // specify the local cache provider name after v0.5.4
                //         config.LocalCacheProviderName = "mem";
                //         // specify the distributed cache provider name after v0.5.4
                //         config.DistributedCacheProviderName = "redis";
                //     })
                //     // use redis bus
                //     .WithRedisBus(busConf => { busConf.Endpoints.Add(new ServerEndPoint("127.0.0.1", 6380)); });
            });
        }

        public static IApplicationBuilder UseInfrastructure(this IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseMiddleware<StackifyMiddleware.RequestTracerMiddleware>();
            //https://andrewlock.net/using-serilog-aspnetcore-in-asp-net-core-3-logging-the-selected-endpoint-name-with-serilog/
            if (env.IsTest() == false)
                app.UseSerilogRequestLogging(opts =>
                    opts.EnrichDiagnosticContext = RequestLoggingHelper.EnrichFromRequest);
            app.UseCustomExceptionHandler();

            return app;
        }
    }
}