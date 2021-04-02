using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Oakton;

namespace OnlineStore.Modules.Catalog.Api
{
    public class Program
    {
        //https://andrewlock.net/viewing-application-configuration-using-oaktons-describe-command/
        public static Task Main(string[] args)
        {
            return CreateHostBuilder(args).RunOaktonCommands(args);
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
        }
    }
}