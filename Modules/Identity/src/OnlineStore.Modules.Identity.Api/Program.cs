using System;
using System.Threading.Tasks;
using Common.Logging.Serilog;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SpectreConsole;

namespace OnlineStore.Modules.Identity.Api
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            await CreateHostBuilder(args).Build().RunAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseLogging(loggerConfiguration =>
                {
                    loggerConfiguration
                    .Enrich.FromLogContext()
                    .WriteTo.SpectreConsole("{Timestamp:HH:mm:ss} [{Level:u4}] {Message:lj}{NewLine}{Exception}", LogEventLevel.Information)
                    .WriteTo.Console()
                    .WriteTo.Seq(
                        Environment.GetEnvironmentVariable("SEQ_URL") ?? "http://localhost:5341")
                    .WriteTo.Stackify();
                })
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
    }
}