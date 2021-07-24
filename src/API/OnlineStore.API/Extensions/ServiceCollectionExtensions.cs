using System;
using BuildingBlocks.Web;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Shopping.API.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddHealthCheck(this IServiceCollection services, IConfiguration configuration,
            string urlGroup = "")
        {
            services.AddHealthChecks()
                .AddDbContextCheck<IdentityDbContext>(name: "Identity DB Context", HealthStatus.Degraded)
                .AddUrlGroup(new Uri(urlGroup), "Online Store", HealthStatus.Degraded)
                .AddSqlServer(configuration.GetConnectionString("OnlineStore"));

            services.AddHealthChecksUI(setup => { setup.AddHealthCheckEndpoint("Basic Health Check", $"/healthz"); })
                .AddInMemoryStorage();

            return services;
        }

        public static IServiceCollection AddVersioning(this IServiceCollection services)
        {
            services.AddApiVersioning(config =>
            {
                config.DefaultApiVersion = new ApiVersion(1, 0);
                config.AssumeDefaultVersionWhenUnspecified = true;
                config.ReportApiVersions = true;
            });

            return services;
        }

        public static IServiceCollection AddCors(this IServiceCollection services, Cors cors)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("AllowedOrigins", builder => builder
                    .WithOrigins(cors.AllowedOrigins)
                    .AllowAnyMethod()
                    .AllowAnyHeader());

                options.AddPolicy("AllowAnyOrigin", builder => builder
                    .AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader());

                options.AddPolicy("CustomPolicy", builder => builder
                    .AllowAnyOrigin()
                    .WithMethods("Get")
                    .WithHeaders("Content-Type"));
            });
            return services;
        }


        public static void AddCaching(this IServiceCollection services, IConfiguration configuration)
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
    }
}