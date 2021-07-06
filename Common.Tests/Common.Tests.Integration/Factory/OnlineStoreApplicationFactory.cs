using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Common.Persistence.MSSQL;
using Common.Tests.Integration.Constants;
using Common.Tests.Integration.Extensions;
using Common.Tests.Integration.Mocks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Moq;
using OnlineStore.Modules.Identity.Application.Features.System;
using OnlineStore.Modules.Identity.Application.Features.Users.Contracts;
using OnlineStore.Modules.Identity.Infrastructure;
using Serilog;
using Serilog.Events;
using Serilog.Templates;
using Xunit.Abstractions;

namespace Common.Tests.Integration.Factory
{
    //https://andrewlock.net/converting-integration-tests-to-net-core-3/
    //https://docs.microsoft.com/en-us/aspnet/core/test/integration-tests
    public class OnlineStoreApplicationFactory<TEntryPoint> : WebApplicationFactory<TEntryPoint>
        where TEntryPoint : class
    {
        public IConfiguration Configuration => Services.GetRequiredService<IConfiguration>();
        public string CurrentUserId { get; set; }
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
            //https://github.com/yorchideas/Serilog.Sinks.Xunit2
            builder = builder.UseSerilog((_, _, configuration) => configuration.WriteTo.Xunit(OutputHelper));

            return builder;
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            //Be careful: configuration in ConfigureWebHost will override configuration in CreateHostBuilder
            // we can use settings that defined on CreateHostBuilder but some on them maybe override in ConfigureWebHost both of them add its configurations to `IHostBuilder`

            builder.UseEnvironment(
                "tests"); //https://docs.microsoft.com/en-us/aspnet/core/test/integration-tests#set-the-environment
            //The test app's builder.ConfigureTestServices callback is executed after the app's Startup.ConfigureServices code is executed.
            builder.ConfigureTestServices((services) =>
            {
                services.RemoveAll(typeof(IHostedService));
                services.ReplaceScoped(_ => Mock.Of<ICurrentUserService>(s => s.UserId == CurrentUserId));

                services.AddTestAuthentication();

                var roleClaims = UsersConstants.AdminUser.Roles.Select(role => new Claim(ClaimTypes.Role, role));
                var otherClaims = new List<Claim>
                {
                    new(ClaimTypes.NameIdentifier, UsersConstants.AdminUser.UserId),
                    new(ClaimTypes.Name, UsersConstants.AdminUser.UserName),
                    new(ClaimTypes.Email, UsersConstants.AdminUser.UserEmail)
                };
                var user = new MockAuthUser(roleClaims.Concat(otherClaims).ToArray());
                services.AddScoped(_ => user);

                TestRegistrationServices?.Invoke(services);
            });

            //The test app's builder.ConfigureServices callback is executed before the SUT's Startup.ConfigureServices code.
            builder.ConfigureServices(services =>
            {
                var sp = services.BuildServiceProvider();

                using var scope = sp.CreateScope();
                var identityContext = scope.ServiceProvider.GetRequiredService<IDbFacadeResolver>();
                var seeder = scope.ServiceProvider.GetRequiredService<IDataSeeder>();
                identityContext.Database.MigrateAsync();
                IdentityDbUpInitializer.Initialize(identityContext.Database.GetConnectionString());

                seeder.SeedAllAsync().GetAwaiter().GetResult();
            });

            //https://visualstudiomagazine.com/Blogs/Tool-Tracker/2019/03/Keeping-Configuration-Settings.aspx
            // builder.ConfigureAppConfiguration((context, config) =>
            // {
            //     config.AddInMemoryCollection(new[]
            //     {
            //         new KeyValuePair<string, string>(
            //             "mssql:ConnectionStrings", "")
            //     });
            // });
        }
    }
}