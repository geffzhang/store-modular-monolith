using System;
using System.Threading.Tasks;
using Common;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OnlineStore.Modules.Identity.Application.Features.System;
using OnlineStore.Modules.Identity.Infrastructure;
using Serilog;

namespace OnlineStore.Modules.Identity.Api
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            await SeedData(host);
            await host.RunAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSerilog((context, loggerConfiguration) =>
                {
                    loggerConfiguration
                        .Enrich.FromLogContext()
                        .WriteTo.Console()
                        // .WriteTo.Console(new ExpressionTemplate(
                        //     "{ {@t, @mt, @l: if @l = 'Information' then undefined() else @l, @x, ..@p} }\n", theme: TemplateTheme.Code))
                        .WriteTo.Seq(
                            Environment.GetEnvironmentVariable("SEQ_URL") ?? "http://localhost:5341")
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