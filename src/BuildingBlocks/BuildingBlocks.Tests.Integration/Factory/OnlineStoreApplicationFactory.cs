using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using BuildingBlocks.Core.Extensions;
using BuildingBlocks.Persistence.MSSQL;
using BuildingBlocks.Tests.Integration.Constants;
using BuildingBlocks.Tests.Integration.Extensions;
using BuildingBlocks.Tests.Integration.Mocks;
using BuildingBlocks.Web.Contexts;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NSubstitute;
using OnlineStore.Modules.Identity.Application.System;
using OnlineStore.Modules.Identity.Infrastructure;
using Serilog;
using Xunit.Abstractions;

namespace BuildingBlocks.Tests.Integration.Factory
{
    //https://andrewlock.net/converting-integration-tests-to-net-core-3/
    //https://docs.microsoft.com/en-us/aspnet/core/test/integration-tests
    public class OnlineStoreApplicationFactory<TEntryPoint> : WebApplicationFactory<TEntryPoint>
        where TEntryPoint : class
    {
        public IConfiguration Configuration => Services.GetRequiredService<IConfiguration>();
        public ITestOutputHelper OutputHelper { get; set; }
        public IEnumerable<IDataSeeder> DataSeeders { get; set; }
        public Action<IServiceCollection> TestRegistrationServices { get; set; }

        public OnlineStoreApplicationFactory() : this(null)
        {
        }

        public OnlineStoreApplicationFactory(Action<IServiceCollection> testRegistrationServices = null)
        {
            TestRegistrationServices = testRegistrationServices ?? (collection => { });
        }

        //use this if we use IHost and generic host
        protected override IHostBuilder CreateHostBuilder()
        {
            var builder = base.CreateHostBuilder();
            // // to remove logging in serilog
            // Log.Logger = Logger.None; //Log.Logger = new LoggerConfiguration().CreateLogger();
            // builder = builder.UseSerilog();

            // to log in xunit output
            builder = builder.UseSerilog((_, _, configuration) =>
            {
                if (OutputHelper is not null)
                    configuration.WriteTo.Xunit(OutputHelper);
            });

            return builder;
        }

        //https://github.com/dotnet/aspnetcore/issues/17707
        protected override IHost CreateHost(IHostBuilder builder)
        {
            builder.UseContentRoot(Directory.GetCurrentDirectory());
            return base.CreateHost(builder);
        }
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            //https://docs.microsoft.com/en-us/aspnet/core/test/integration-tests#set-the-environment
            builder.UseEnvironment("test");

            //The test app's builder.ConfigureTestServices callback is executed after the app's Startup.ConfigureServices code is executed.
            builder.ConfigureTestServices((services) =>
            {
                // services.RemoveAll(typeof(IHostedService));

                services.AddScoped(_ => CreateAnonymouslyUserMock());

                services.AddTestAuthentication();

                TestRegistrationServices?.Invoke(services);

                services.ReplaceScoped(CreateHttpContextAccessorMock);

                services.ReplaceScoped(CreateExecutionContextAccessorMock);
            });

            //The test app's builder.ConfigureServices callback is executed before the SUT's Startup.ConfigureServices code.
            builder.ConfigureServices(services =>
            {
                var sp = services.BuildServiceProvider();

                using var scope = sp.CreateScope();
                var identityContext = scope.ServiceProvider.GetRequiredService<IDbFacadeResolver>();
                var seeder = scope.ServiceProvider.GetRequiredService<IDataSeeder>();
                identityContext.Database.MigrateAsync().GetAwaiter().GetResult();

                seeder.SeedAllAsync().GetAwaiter().GetResult();
            });

            //https://visualstudiomagazine.com/Blogs/Tool-Tracker/2019/03/Keeping-Configuration-Settings.aspx
            // builder.ConfigureAppConfiguration((context, config) =>
            // {
            //     config.AddInMemoryCollection(new[]
            //     {
            //         new KeyValuePair<string, string>(
            //             "mssql:ConnectionStrings", "Data Source=.\sqlexpress;Initial Catalog=OnlineStore-Test;Integrated Security=True;Connect Timeout=30")
            //     });
            // });
        }

        public HttpClient CreateClientWithTestAuth()
        {
            var client = CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(AuthConstants.Scheme);

            return client;
        }

        private static IHttpContextAccessor CreateHttpContextAccessorMock(IServiceProvider serviceProvider)
        {
            var httpContextAccessorMock = Substitute.For<IHttpContextAccessor>();
            httpContextAccessorMock.HttpContext = new DefaultHttpContext
            {
                RequestServices = serviceProvider
            };
            var res = httpContextAccessorMock.HttpContext.AuthenticateAsync(AuthConstants.Scheme).GetAwaiter()
                .GetResult();
            httpContextAccessorMock.HttpContext.User = res.Ticket?.Principal!;
            return httpContextAccessorMock;
        }

        private MockAuthUser CreateAnonymouslyUserMock()
        {
            return new MockAuthUser();
        }

        private static IExecutionContextAccessor CreateExecutionContextAccessorMock(IServiceProvider serviceProvider)
        {
            var executionContextFactory = serviceProvider.GetService<IExecutionContextFactory>();
            return _ = new ExecutionContextAccessorMock(executionContextFactory?.Create());
        }
    }
}