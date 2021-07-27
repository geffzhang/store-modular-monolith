using System;
using System.Diagnostics;
using BuildingBlocks.Core.Domain;
using BuildingBlocks.Core.Domain.DomainEventNotifications;
using BuildingBlocks.Core.Domain.DomainEvents;
using BuildingBlocks.Core.Messaging;
using BuildingBlocks.Cqrs;
using BuildingBlocks.Cqrs.Commands;
using BuildingBlocks.Cqrs.Queries;
using BuildingBlocks.Diagnostics.Cqrs;
using BuildingBlocks.Diagnostics.Messaging;
using BuildingBlocks.Diagnostics.Transports;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Exporter;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace BuildingBlocks.Diagnostics
{
    public static class ServiceCollectionExtensions
    {
        private const string SectionName = "OTel";

        public static IServiceCollection AddInMemoryMessagingTelemetry(this IServiceCollection services)
        {
            DiagnosticListener.AllListeners.Subscribe(listener =>
            {
                if (listener.Name == InMemoryTransportListener.InBoundName ||
                    listener.Name == InMemoryTransportListener.OutBoundName)
                {
                    listener.SubscribeWithAdapter(new InMemoryTransportListener());
                }
            });

            return services;
        }

        public static IServiceCollection AddOTel(this IServiceCollection services, IConfiguration configuration,
            string sectionName = SectionName, Action<ZipkinExporterOptions> configureZipkin = null,
            Action<JaegerExporterOptions> configureJaeger = null)
        {
            var options = configuration.GetSection(sectionName).Get<OTelOptions>();
            services.AddOptions<OTelOptions>().Bind(configuration.GetSection(sectionName)).ValidateDataAnnotations();

            services.AddOpenTelemetryTracing(builder =>
            {
                builder.SetSampler(new AlwaysOnSampler())
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddGrpcClientInstrumentation()
                    .AddSqlClientInstrumentation(opt => opt.SetDbStatementForText = true)
                    .AddSource(OTeMessagingOptions.OTelMessagingName)
                    .AddSource(OTelCqrsOptions.OTelCqrsRequestName)
                    .AddSource(OTelDomainOptions.OTelDomainEventHandlerName)
                    .AddSource(OTelDomainOptions.OTelDomainEventNotificationHandlerName)
                    .AddSource(OTelTransportOptions.InMemoryConsumerActivityName)
                    .AddSource(OTelTransportOptions.InMemoryProducerActivityName)
                    .AddZipkinExporter(o =>
                    {
                        configuration.Bind("OtelZipkin", o);
                        configureZipkin?.Invoke(o);
                        o.Endpoint = options.ZipkinExporterOptions.Endpoint; //"http://localhost:9411/api/v2/spans"
                    })
                    .AddJaegerExporter(c =>
                    {
                        configureJaeger?.Invoke(c);
                        c.AgentHost = options.JaegerExporterOptions.AgentHost; //localhost
                        c.AgentPort = options.JaegerExporterOptions.AgentPort; //6831
                    });

                if (options.Services is not null)
                    foreach (var service in options.Services)
                    {
                        builder.SetResourceBuilder(ResourceBuilder.CreateDefault()
                            .AddService(service)); //"Identity.Api"
                    }
            });

            services.TryDecorate(typeof(IDomainEventHandler<>), typeof(OTelDomainEventTracingDecorator<>));
            services.TryDecorate(typeof(IDomainEventNotificationHandler<>),
                typeof(OTelNotificationEventTracingDecorator<>));

            services.AddScoped(typeof(IMessageMiddleware<>), typeof(OTelDiagnosticsMessagingMiddleware<>));
            services.AddScoped(typeof(IRequestMiddleware<,>), typeof(OTelDiagnosticsCqrsRequestMiddleware<,>));

            return services;
        }
    }
}