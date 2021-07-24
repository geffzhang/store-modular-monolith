using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Enrichers.Span;
using Serilog.Events;
using Serilog.Formatting.Elasticsearch;
using Serilog.Templates;

namespace OnlineStore.API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            //in .net 5 this is default format
            Activity.DefaultIdFormat = ActivityIdFormat.W3C;
            Activity.ForceDefaultIdFormat = true;

            var listener = new ActivityListener
            {
                ShouldListenTo = _ => true, // listener names
                ActivityStopped = activity =>
                {
                    foreach (var (key, value) in activity.Baggage)
                    {
                        activity.AddTag(key, value);
                    }
                }
            };
            ActivitySource.AddActivityListener(listener);

            //https://github.com/serilog/serilog-aspnetcore
            // The initial "bootstrap" logger is able to log errors during start-up. It's completely replaced by the
            // logger configured in `UseSerilog()` below, once configuration and dependency-injection have both been
            // set up successfully.
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console(new ExpressionTemplate("{ {@t, @mt, @l, @x, ..@p} }\n"))
                .CreateBootstrapLogger();

            try
            {
                var host = CreateHostBuilder(args).Build();
                // await SeedData(host);
                await host.RunAsync();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                //we could use our extension method for serilog 'UseLogging' from common for less code
                // .UseCustomSerilog(configuration => { })
                .UseSerilog((context, services, configuration) =>
                {
                    if (context.HostingEnvironment.IsDevelopment())
                    {
                        configuration.WriteTo.Console(new ExpressionTemplate("{ {@t, @mt, @l, @x, ..@p} }\n"));
                    }
                    else
                    {
                        configuration.WriteTo.Console(new ElasticsearchJsonFormatter());
                        configuration.WriteTo.Seq(Environment.GetEnvironmentVariable("SEQ_URL") ??
                                                  "http://localhost:5341");
                    }

                    configuration
                        .ReadFrom.Configuration(context.Configuration)
                        .ReadFrom.Services(services)
                        .Enrich.FromLogContext()
                        // .WriteTo.Console(new RenderedCompactJsonFormatter())
                        // .WriteTo.Console(
                        //     outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}",
                        //     theme: AnsiConsoleTheme.Code)
                        .MinimumLevel.Is(LogEventLevel.Information)
                        .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
                        .Enrich.WithSpan()
                        .WriteTo.Stackify();
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}