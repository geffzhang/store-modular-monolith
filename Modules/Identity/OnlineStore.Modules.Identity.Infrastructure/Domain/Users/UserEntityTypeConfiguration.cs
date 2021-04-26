using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OnlineStore.Modules.Identity.Domain;
using OnlineStore.Modules.Identity.Domain.Permissions;
using OnlineStore.Modules.Identity.Domain.Users;

namespace OnlineStore.Modules.Identity.Infrastructure.Domain.Users
{
    internal class UserEntityTypeConfiguration : IEntityTypeConfiguration<Identity.Domain.Users.User>
    {
        public void Configure(EntityTypeBuilder<Identity.Domain.Users.User> builder)
        {
            builder.ToTable("Users", "user");

            builder.HasKey(x => x.Id);

            builder
                .Property(x => x.Id)
                .HasField("Id")
                .HasColumnName("Id")
                .UsePropertyAccessMode(PropertyAccessMode.Field)
                .HasConversion(x => x.Id, id => (UserId)id);

            builder.Property(x => x.Email);
            builder.Property(x => x.Password);
            builder.Property(x => x.IsActive);
            builder.Property(x => x.FirstName);
            builder.Property(x => x.LastName);
            builder.Property(x => x.Name);

            // 1 : N => User : Roles
            builder.OwnsMany<Role>("Roles", b =>
            {
                b.WithOwner().HasForeignKey("UserId");
                b.ToTable("UserRoles", "user");
                b.Property<UserId>("UserId");
                b.Property(x => x.Name).HasColumnName("Name");
                b.HasKey("UserId", "Name");
            });

            // 1 : N => User : Addresses
            builder.OwnsMany<Address>("UserAddresses", b =>
            {
                b.WithOwner().HasForeignKey("UserId");
                b.ToTable("UserAddresses", "user");
                b.Property<UserId>("UserId");
                b.Property(x => x.City);
                b.Property(x => x.Country);
                b.Property(x => x.State);
                b.Property(x => x.Street);
                b.Property(x => x.RegionId);
                b.Property(x => x.RegionName);
                b.Property(x => x.ZipCode);
                b.HasKey("UserId", "Country", "City", "State", "Street");
            });

            // 1 : N => User : Permissions
            builder.OwnsMany<Permission>("Permissions", b =>
            {
                b.WithOwner().HasForeignKey("UserId");
                b.ToTable("UserPermissions", "user");
                b.Property<UserId>("UserId");
                b.Property(x => x.Name).HasColumnName("Name");
                b.HasKey("UserId", "Name");
            });
        }
    }
}