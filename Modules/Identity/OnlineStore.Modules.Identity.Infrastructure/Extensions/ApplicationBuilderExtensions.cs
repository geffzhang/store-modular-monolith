using System;
using System.Threading.Tasks;
using Common;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OnlineStore.Modules.Identity.Application.System;


namespace OnlineStore.Modules.Identity.Infrastructure.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        internal static async Task UseIdentityDataSeederAsync(this IApplicationBuilder app)
        {
            var scopeFactory = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>();

            var commandProcessor = scopeFactory.CreateScope().ServiceProvider.GetRequiredService<ICommandProcessor>();
            using (var scope = app.ApplicationServices.CreateScope())
            {
                var services = scope.ServiceProvider;

                var identityContext = services.GetRequiredService<IdentityDbContext>();

                await identityContext.Database.MigrateAsync();

                await commandProcessor.SendCommandAsync(new SeedIdentityCommand());
            }
        }

        // https://github.com/NickStrupat/EntityFramework.Triggers
        // public static IApplicationBuilder UseDbTriggers(this IApplicationBuilder appBuilder)
        // {
        //     Triggers<IAuditable>.Inserting += entry =>
        //     {
        //         var currentUserNameResolver = appBuilder.ApplicationServices.CreateScope().ServiceProvider
        //             .GetService<IUserNameResolver>();
        //         var currentTime = DateTime.UtcNow;
        //         var userName = currentUserNameResolver?.GetCurrentUserName();
        //
        //         entry.Entity.CreatedDate = entry.Entity.CreatedDate == default ? currentTime : entry.Entity.CreatedDate;
        //         entry.Entity.CreatedBy ??= userName;
        //     };
        //
        //     Triggers<IAuditable>.Updating += entry =>
        //     {
        //         var currentUserNameResolver = appBuilder.ApplicationServices.CreateScope().ServiceProvider
        //             .GetService<IUserNameResolver>();
        //         var currentTime = DateTime.UtcNow;
        //         var userName = currentUserNameResolver?.GetCurrentUserName();
        //
        //         entry.Entity.ModifiedDate = currentTime;
        //
        //         if (string.IsNullOrEmpty(userName) == false)
        //         {
        //             entry.Entity.ModifiedBy = userName;
        //         }
        //     };
        //
        //     return appBuilder;
        // }
    }
}