using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OnlineStore.Modules.Identity.Domain.Users;

namespace OnlineStore.Modules.Identity.Infrastructure.Domain.User
{
    public class PermissionEntityTypeConfiguration: IEntityTypeConfiguration<Identity.Domain.Users.Role>
    {
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            builder.Property(x => x.Name).HasColumnName("RoleName");
            builder.OwnsMany<Permission>("Permissions", b =>
            {
                b.Property(x => x.Name).HasColumnName("PermissionName");
                b.HasKey("RoleName", "PermissionName");
            });
        }
    }
}