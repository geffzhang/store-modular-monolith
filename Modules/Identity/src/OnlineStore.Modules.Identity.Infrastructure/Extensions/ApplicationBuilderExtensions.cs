using System.Threading.Tasks;
using Common;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OnlineStore.Modules.Identity.Application.Features.System;


namespace OnlineStore.Modules.Identity.Infrastructure.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        internal static async Task UseIdentityDataSeederAsync(this IApplicationBuilder app)
        {
            var scopeFactory = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>();
            var commandProcessor = scopeFactory.CreateScope().ServiceProvider.GetRequiredService<ICommandProcessor>();
            using var scope = app.ApplicationServices.CreateScope();
            var services = scope.ServiceProvider;

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