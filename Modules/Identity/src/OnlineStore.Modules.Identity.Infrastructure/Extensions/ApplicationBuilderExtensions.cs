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

            await identityContext.Database.MigrateAsync();

            await commandProcessor.SendCommandAsync(new SeedIdentityCommand());
        }
    }
}