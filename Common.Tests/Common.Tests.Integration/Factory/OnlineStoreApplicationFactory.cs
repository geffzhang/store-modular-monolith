using System.Linq;
using Common.Messaging.Outbox;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Moq;
using OnlineStore.Modules.Identity.Application.Features.Users.Contracts;
using Xunit.Abstractions;

namespace Common.Tests.Integration.Factory
{
    //https://andrewlock.net/converting-integration-tests-to-net-core-3/
    //https://docs.microsoft.com/en-us/aspnet/core/test/integration-tests
    public class OnlineStoreApplicationFactory<TEntryPoint> : WebApplicationFactory<TEntryPoint> where TEntryPoint : class
    {
        public ITestOutputHelper Output { get; set; }
        public IConfiguration Configuration => Services.GetRequiredService<IConfiguration>();
        public IServiceScopeFactory ScopeFactory => Services.GetRequiredService<IServiceScopeFactory>();

        public string CurrentUserId { get; set; }

        // This won't be called because we're using the generic host
        // protected override IWebHostBuilder CreateWebHostBuilder()
        // {
        //     var builder = base.CreateWebHostBuilder();
        //     builder.UseEnvironment("tests");
        //     builder.ConfigureLogging(logging =>
        //     {
        //         logging.ClearProviders();
        //         logging.AddXUnit(Output);
        //     });

        //     return builder;
        // }

        //use this if we use IHost and generic host
        protected override IHostBuilder CreateHostBuilder()
        {
            var builder = base.CreateHostBuilder();

            // builder.ConfigureLogging(logging =>
            // {
            //     logging.ClearProviders(); // Remove other loggers
            //     logging.AddXUnit(Output); // Use the ITestOutputHelper instance
            // });

            return builder;
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            //Be careful: configuration in ConfigureWebHost will override configuration in CreateHostBuilder
            // we can use settings that defined on CreateHostBuilder but some on them maybe override in ConfigureWebHost both of them add its configurations to `IHostBuilder`

            builder.UseEnvironment("tests"); //https://docs.microsoft.com/en-us/aspnet/core/test/integration-tests#set-the-environment
            // Don't run IHostedServices when running as a test

            builder.ConfigureTestServices((services) => { services.RemoveAll(typeof(IHostedService)); });

            //The test app's builder.ConfigureServices callback is executed after the app's Startup.ConfigureServices code is executed.
            builder.ConfigureServices(services =>
            {
                var currentUserServiceDescriptor = services.FirstOrDefault(d => d.ServiceType == typeof(ICurrentUserService));
                services.Remove(currentUserServiceDescriptor);
                
                // Register testing version
                services.AddScoped(_ => Mock.Of<ICurrentUserService>(s => s.UserId == CurrentUserId));
            });
            builder.Configure((context,applicationBuilder) =>
            {
            });
        }
    }
}