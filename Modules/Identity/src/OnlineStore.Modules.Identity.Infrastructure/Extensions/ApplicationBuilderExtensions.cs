using System;
using System.Threading.Tasks;
using Common.Core;
using Common.Web.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OnlineStore.Modules.Identity.Application.Features.System;

namespace OnlineStore.Modules.Identity.Infrastructure.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseInfrastructure(this IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsTest() == false)
                SeedData(app.ApplicationServices).GetAwaiter().GetResult();

            return app;
        }

        private static async Task SeedData(IServiceProvider serviceProvider)
        {
            var commandProcessor = serviceProvider.GetRequiredService<ICommandProcessor>();
            var identityContext =
                serviceProvider
                    .GetRequiredService<Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityDbContext>();
            //TODO:Replacing with DbUp
            //https://stackoverflow.com/a/38241900/581476
            //https://www.michalbialecki.com/2020/07/20/adding-entity-framework-core-5-migrations-to-net-5-project/
            await identityContext.Database.MigrateAsync();
            IdentityDbUpInitializer.Initialize(identityContext.Database.GetConnectionString());

            await commandProcessor.SendCommandAsync(new SeedIdentityCommand());
        }
    }
}