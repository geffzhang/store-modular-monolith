using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Common;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OnlineStore.Modules.Identity.Application.Features.System;
using OnlineStore.Modules.Identity.Infrastructure;
using Serilog;
using Serilog.Formatting.Compact;
using Serilog.Templates;
using Serilog.Enrichers.Span;
using Serilog.Events;
using Serilog.Formatting.Elasticsearch;

namespace OnlineStore.Modules.Identity.Api
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            Activity.DefaultIdFormat = ActivityIdFormat.W3C;
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
                await SeedData(host);
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
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });

        public static async Task SeedData(IHost host)
        {
            using var scope = host.Services.CreateScope();
            var services = scope.ServiceProvider;
            var commandProcessor = services.GetRequiredService<ICommandProcessor>();
            var identityContext = services.GetRequiredService<IdentityDbContext>();
            //TODO:Replacing with DbUp 
            //https://stackoverflow.com/a/38241900/581476
            //https://www.michalbialecki.com/2020/07/20/adding-entity-framework-core-5-migrations-to-net-5-project/
            await identityContext.Database.MigrateAsync();
            IdentityDbUpInitializer.Initialize(identityContext.Database.GetConnectionString());

            await commandProcessor.SendCommandAsync(new SeedIdentityCommand());
        }
    }
}