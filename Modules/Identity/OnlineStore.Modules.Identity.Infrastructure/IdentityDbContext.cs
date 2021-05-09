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
    /// https://docs.microsoft.com/en-us/aspnet/core/security/authentication/identity
    /// https://docs.microsoft.com/en-us/dotnet/architecture/microservices/secure-net-microservices-web-applications
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

            builder.Entity<ApplicationUser>().Ignore(x => x.Permissions); // this ignored properties will handle in UserManager and RoleManager
            builder.Entity<ApplicationRole>().Ignore(x => x.Permissions);
            builder.Entity<ApplicationUser>().Ignore(x => x.Password);
            builder.Entity<ApplicationUser>().Ignore(x => x.Roles);
            builder.Entity<ApplicationUser>().Ignore(x => x.Logins);
            builder.Entity<ApplicationUser>().Property(x => x.UserType).HasMaxLength(64);
            builder.Entity<ApplicationUser>().Property(x => x.PhotoUrl).HasMaxLength(2048);
            builder.Entity<ApplicationUser>().Property(x => x.Id).HasMaxLength(128).ValueGeneratedOnAdd();
            builder.Entity<ApplicationUser>().Property(x => x.MemberId).HasMaxLength(128);
            builder.Entity<ApplicationRole>().Property(x => x.Id).HasColumnName("Code").HasMaxLength(50);
            builder.Entity<IdentityUserClaim<string>>().Property(x => x.UserId).HasMaxLength(128);
            builder.Entity<IdentityUserLogin<string>>().Property(x => x.UserId).HasMaxLength(128);
            builder.Entity<IdentityUserLogin<string>>().Property(x => x.LoginProvider).HasMaxLength(128);
            builder.Entity<IdentityUserLogin<string>>().Property(x => x.ProviderKey).HasMaxLength(128);
            builder.Entity<IdentityUserRole<string>>().Property(x => x.UserId).HasMaxLength(128);
            builder.Entity<IdentityUserRole<string>>().Property(x => x.RoleId).HasColumnName("RoleCode").HasMaxLength(50);
            builder.Entity<IdentityRoleClaim<string>>().Property(x => x.RoleId).HasColumnName("RoleCode").HasMaxLength(50);
            builder.Entity<IdentityUserToken<string>>().Property(x => x.UserId).HasMaxLength(128);

            MapsTables(builder);
        }

        private static void MapsTables(ModelBuilder builder)
        {
            builder.Entity<ApplicationUser>(b => { b.ToTable("User"); }).HasDefaultSchema("users");

            builder.Entity<IdentityUserClaim<string>>(b => { b.ToTable("UserClaim"); }).HasDefaultSchema("users");;

            builder.Entity<IdentityUserLogin<string>>(b => { b.ToTable("UserLogin"); }).HasDefaultSchema("users");;

            builder.Entity<IdentityUserToken<string>>(b => { b.ToTable("UserToken"); }).HasDefaultSchema("users");;

            builder.Entity<ApplicationRole>(b => { b.ToTable("Role"); }).HasDefaultSchema("users");;

            builder.Entity<IdentityRoleClaim<string>>(b => { b.ToTable("RoleClaim"); }).HasDefaultSchema("users");;

            builder.Entity<IdentityUserRole<string>>(b => { b.ToTable("UserRoles"); }).HasDefaultSchema("users");;
        }
    }
}