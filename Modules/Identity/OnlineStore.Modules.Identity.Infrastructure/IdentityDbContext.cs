using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OnlineStore.Modules.Identity.Infrastructure.Domain.Roles;
using OnlineStore.Modules.Identity.Infrastructure.Domain.Users.Models;

namespace OnlineStore.Modules.Identity.Infrastructure
{
    /// <summary>
    /// https://docs.microsoft.com/en-us/aspnet/core/security/authentication/customize-identity-model
    /// https://docs.microsoft.com/en-us/aspnet/core/security/authentication/identity?view=aspnetcore-5.0&tabs=visual-studio
    /// https://docs.microsoft.com/en-us/dotnet/architecture/microservices/secure-net-microservices-web-applications/
    /// </summary>
    public class IdentityDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
    {
        public IdentityDbContext(DbContextOptions<IdentityDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<IdentityUserRole<string>>(userRole =>
            {
                userRole.HasOne<ApplicationRole>()
                    .WithMany(r => r.UserRoles)
                    .HasForeignKey(ur => ur.RoleId);

                userRole.HasOne<ApplicationUser>()
                    .WithMany(r => r.UserRoles)
                    .HasForeignKey(ur => ur.UserId);
            });


            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
            builder.Entity<ApplicationUser>().Ignore(x => x.Permissions);
            builder.Entity<ApplicationRole>().Ignore(x => x.Permissions);
            builder.Entity<ApplicationUser>().Ignore(x => x.Password);
            builder.Entity<ApplicationUser>().Ignore(x => x.Roles);
            builder.Entity<ApplicationUser>().Ignore(x => x.Logins);
            builder.Entity<ApplicationUser>().Property(x => x.UserType).HasMaxLength(64);
            builder.Entity<ApplicationUser>().Property(x => x.PhotoUrl).HasMaxLength(2048);
            builder.Entity<ApplicationUser>().Property(x => x.Id).HasMaxLength(128).ValueGeneratedOnAdd();
            builder.Entity<ApplicationUser>().Property(x => x.MemberId).HasMaxLength(128);
            builder.Entity<ApplicationRole>().Property(x => x.Id).HasMaxLength(128).ValueGeneratedOnAdd();
            builder.Entity<IdentityUserClaim<string>>().Property(x => x.UserId).HasMaxLength(128);
            builder.Entity<IdentityUserLogin<string>>().Property(x => x.UserId).HasMaxLength(128);
            builder.Entity<IdentityUserLogin<string>>().Property(x => x.LoginProvider).HasMaxLength(128);
            builder.Entity<IdentityUserLogin<string>>().Property(x => x.ProviderKey).HasMaxLength(128);
            builder.Entity<IdentityUserRole<string>>().Property(x => x.UserId).HasMaxLength(128);
            builder.Entity<IdentityUserRole<string>>().Property(x => x.RoleId).HasMaxLength(128);
            builder.Entity<IdentityRoleClaim<string>>().Property(x => x.RoleId).HasMaxLength(128);
            builder.Entity<IdentityUserToken<string>>().Property(x => x.UserId).HasMaxLength(128);

            builder.Entity<ApplicationUser>().Property(x => x.CreatedDate);

            MapsTables(builder);
        }

        private static void MapsTables(ModelBuilder builder)
        {
            builder.Entity<ApplicationUser>(b => { b.ToTable("User"); });

            builder.Entity<IdentityUserClaim<string>>(b => { b.ToTable("UserClaim"); });

            builder.Entity<IdentityUserLogin<string>>(b => { b.ToTable("UserLogin"); });

            builder.Entity<IdentityUserToken<string>>(b => { b.ToTable("UserToken"); });

            builder.Entity<ApplicationRole>(b => { b.ToTable("Role"); });

            builder.Entity<IdentityRoleClaim<string>>(b => { b.ToTable("RoleClaim"); });

            builder.Entity<IdentityUserRole<string>>(b => { b.ToTable("UserRoles"); });
        }

        // // https://github.com/NickStrupat/EntityFramework.Triggers
        // #region override Save*** methods to catch save events in Triggers, otherwise ApplicationUser not be catched because SecurityDbContext can't inherit DbContextWithTriggers
        // public override int SaveChanges()
        // {
        //     return this.SaveChangesWithTriggers(base.SaveChanges);
        // }
        // public override int SaveChanges(bool acceptAllChangesOnSuccess)
        // {
        //     return this.SaveChangesWithTriggers(base.SaveChanges, acceptAllChangesOnSuccess);
        // }
        // public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        // {
        //     return this.SaveChangesWithTriggersAsync(base.SaveChangesAsync, true, cancellationToken);
        // }
        // public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        // {
        //     return this.SaveChangesWithTriggersAsync(base.SaveChangesAsync, acceptAllChangesOnSuccess, cancellationToken);
        // }
        // #endregion
    }
}