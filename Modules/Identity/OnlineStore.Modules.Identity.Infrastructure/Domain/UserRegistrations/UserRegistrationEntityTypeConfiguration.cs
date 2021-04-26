using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OnlineStore.Modules.Identity.Domain;
using OnlineStore.Modules.Identity.Domain.UserRegistrations;
using OnlineStore.Modules.Identity.Domain.Users;

namespace OnlineStore.Modules.Identity.Infrastructure.Domain.UserRegistrations
{
    internal class UserRegistrationEntityTypeConfiguration : IEntityTypeConfiguration<UserRegistration>
    {
        public void Configure(EntityTypeBuilder<UserRegistration> builder)
        {
            builder.ToTable("UserRegistrations", "user");

            builder.HasKey(x => x.Id);
            
            builder
                .Property(x => x.Id)
                .HasField("Id")
                .HasColumnName("Id")
                .UsePropertyAccessMode(PropertyAccessMode.Field)
                .HasConversion(x => x.Id, id => (UserRegistrationId)id);

            // 1 : N => User : Addresses
            builder.OwnsMany<Address>("Addresses", b =>
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
            
            builder.OwnsOne<UserRegistrationStatus>("Status", b =>
            {
                b.Property(x => x.Value).HasColumnName("StatusCode");
            });
        }
    }
}