using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OnlineStore.Modules.Identity.Infrastructure.Aggregates.Users.Models;

namespace OnlineStore.Modules.Identity.Infrastructure.Aggregates.Users
{
    public class ApplicationUserEntityTypeConfiguration : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            //https://docs.microsoft.com/en-us/aspnet/core/security/authentication/customize-identity-model#add-navigation-properties

            // Each User can have many UserClaims
            builder.HasMany(e => e.Claims)
                .WithOne()
                .HasForeignKey(uc => uc.UserId)
                .IsRequired();

            // Each User can have many UserLogins
            builder.HasMany(e => e.Logins)
                .WithOne()
                .HasForeignKey(ul => ul.UserId)
                .IsRequired();

            // Each User can have many UserTokens
            builder.HasMany(e => e.Tokens)
                .WithOne()
                .HasForeignKey(ut => ut.UserId)
                .IsRequired();

            // Each User can have many entries in the UserRole join table
            builder.HasMany(e => e.UserRoles)
                .WithOne(e => e.User)
                .HasForeignKey(ur => ur.UserId)
                .IsRequired();

            //https://docs.microsoft.com/en-us/ef/core/modeling/shadow-properties
            //https://docs.microsoft.com/en-us/ef/core/modeling/owned-entities
            builder.OwnsMany(x => x.RefreshTokens, b =>
            {
                b.WithOwner().HasForeignKey("UserId");
                b.Property<string>("UserId");
                b.HasKey("UserId");
                b.ToTable("RefreshToken", "Identities");
            });

            builder.Ignore(x => x.Permissions); // this ignored properties will handle in UserManager and RoleManager
            builder.Ignore(x => x.Roles);
            builder.Ignore(x => x.Logins);
            builder.Property(x => x.UserType).HasMaxLength(64);
            builder.Property(x => x.PhotoUrl).HasMaxLength(2048);
            builder.Property(x => x.Id).HasMaxLength(128).ValueGeneratedOnAdd();
        }
    }
}