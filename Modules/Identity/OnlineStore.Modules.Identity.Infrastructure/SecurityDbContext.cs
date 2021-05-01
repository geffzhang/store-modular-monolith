using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OnlineStore.Modules.Identity.Infrastructure.Domain.Roles;
using OnlineStore.Modules.Identity.Infrastructure.Domain.Users;

namespace OnlineStore.Modules.Identity.Infrastructure
{
    //https://docs.microsoft.com/en-us/aspnet/core/security/authentication/customize-identity-model
    //https://docs.microsoft.com/en-us/aspnet/core/security/authentication/identity?view=aspnetcore-5.0&tabs=visual-studio
    //https://docs.microsoft.com/en-us/dotnet/architecture/microservices/secure-net-microservices-web-applications/
    public class SecurityDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
    {
        public SecurityDbContext(DbContextOptions<SecurityDbContext> options) : base(options)
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

            MapsTables(builder);

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
            builder.Entity<ApplicationUser>().Property(x => x.StoreId).HasMaxLength(128);
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
        }

        private static void MapsTables(ModelBuilder builder)
        {
            builder.Entity<IdentityUser>(b => { b.ToTable("User"); });

            builder.Entity<IdentityUserClaim<string>>(b => { b.ToTable("UserClaim"); });

            builder.Entity<IdentityUserLogin<string>>(b => { b.ToTable("UserLogin"); });

            builder.Entity<IdentityUserToken<string>>(b => { b.ToTable("UserToken"); });

            builder.Entity<IdentityRole>(b => { b.ToTable("Role"); });

            builder.Entity<IdentityRoleClaim<string>>(b => { b.ToTable("RoleClaim"); });

            builder.Entity<IdentityUserRole<string>>(b => { b.ToTable("UserRoles"); });
        }
    }
}