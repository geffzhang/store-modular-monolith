using System;
using System.Collections.Generic;
using System.Linq;
using Common.Web;
using Logging.Serilog;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog.Enrichers.Span;
using Serilog.Events;
using Serilog.Filters;
using Serilog.Templates;


namespace Common.Logging.Serilog
{
    public static class ServiceCollectionExtensions
    {
        private const string LoggerSectionName = "Logging";
        private const string AppOptionSectionName = "AppOptions";

        public static IHostBuilder UseCustomSerilog(this IHostBuilder builder,
            Action<LoggerConfiguration> extraConfigure = null,
            string loggerSectionName = LoggerSectionName,
            string appSectionName = AppOptionSectionName)
        {
            return builder.UseSerilog((context, serviceProvider, loggerConfiguration) =>
            {
                var httpContext = serviceProvider.GetService<IHttpContextAccessor>();
                loggerConfiguration
                    .ReadFrom.Configuration(context.Configuration)
                    .ReadFrom.Services(serviceProvider)
                    .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
                    .Enrich.FromLogContext()
                    .Enrich.WithSpan()
                    .Enrich.WithTraceId(httpContext);

                var appOptions = context.Configuration.GetSection(appSectionName).Get<AppOptions>();
                var loggerOptions = context.Configuration.GetSection(loggerSectionName).Get<LoggerOptions>();
                if (appOptions is { } && loggerOptions is { })
                    MapOptions(loggerOptions, appOptions, loggerConfiguration, context);

                extraConfigure?.Invoke(loggerConfiguration);
            });
        }

        private static void MapOptions(LoggerOptions loggerOptions, AppOptions appOptions,
            LoggerConfiguration loggerConfiguration, HostBuilderContext hostBuilderContext)
        {
            var level = GetLogEventLevel(loggerOptions.Level);

            loggerConfiguration
                .MinimumLevel.Is(level)
                .Enrich.WithProperty("Environment", hostBuilderContext.HostingEnvironment.EnvironmentName)
                .Enrich.WithProperty("Application", appOptions.Name)
                .Enrich.WithProperty("Instance", appOptions.Instance)
                .Enrich.WithProperty("Version", appOptions.Version);

            if (loggerOptions.UseStackify)
                loggerConfiguration.WriteTo.Stackify();

            if (hostBuilderContext.HostingEnvironment.IsDevelopment())
            {
                loggerConfiguration.WriteTo.Console(new ExpressionTemplate("{ {@t, @mt, @l, @x, ..@p} }\n"));
            }
            else
            {
                if (loggerOptions.UseElasticSearch)
                    loggerConfiguration.WriteTo.Elasticsearch(loggerOptions.ElasticSearchLoggingOptions?.Url);
                if (loggerOptions.UseSeq)
                    loggerConfiguration.WriteTo.Seq(Environment.GetEnvironmentVariable("SEQ_URL") ??
                                                    loggerOptions.SeqOptions.Url);
                loggerConfiguration.WriteTo.Console(new ExpressionTemplate("{ {@t, @mt, @l, @x, ..@p} }\n"));
            }

            foreach (var (key, value) in loggerOptions.Tags ?? new Dictionary<string, object>())
                loggerConfiguration.Enrich.WithProperty(key, value);

            foreach (var (key, value) in loggerOptions.MinimumLevelOverrides ?? new Dictionary<string, string>())
            {
                var logLevel = GetLogEventLevel(value);
                loggerConfiguration.MinimumLevel.Override(key, logLevel);
            }

            loggerOptions.ExcludePaths?.ToList().ForEach(p => loggerConfiguration.Filter
                .ByExcluding(Matching.WithProperty<string>("RequestPath", n => n.EndsWith(p))));

            loggerOptions.ExcludeProperties?.ToList().ForEach(p => loggerConfiguration.Filter
                .ByExcluding(Matching.WithProperty(p)));
        }

        public static LoggerConfiguration WithTraceId(this LoggerEnrichmentConfiguration loggerEnrichmentConfiguration,
            IHttpContextAccessor httpContextAccessor)
        {
            if (loggerEnrichmentConfiguration == null)
                throw new ArgumentNullException(nameof(loggerEnrichmentConfiguration));

            if (httpContextAccessor == null) throw new ArgumentNullException(nameof(httpContextAccessor));

            return loggerEnrichmentConfiguration.With(new TraceIdEnricher(httpContextAccessor));
        }

        private static LogEventLevel GetLogEventLevel(string level)
        {
            return Enum.TryParse<LogEventLevel>(level, true, out var logLevel)
                ? logLevel
                : LogEventLevel.Information;
        }
    }
}