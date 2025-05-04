using Microsoft.EntityFrameworkCore.Metadata.Builders;

using TalkVN.Domain.Entities.SystemEntities.Permissions;

namespace TalkVN.DataAccess.Configurations.Permissions
{

    public class PermissionConfiguration : IEntityTypeConfiguration<Permission>
    {
        public void Configure(EntityTypeBuilder<Permission> modelBuilder)
        {
            // Configure primary key
            modelBuilder.HasKey(p => p.Id);

            // Configure relationship with RolePermission
            modelBuilder
                .HasMany(p => p.RolePermissions)
                .WithOne(rp => rp.Permission)
                .HasForeignKey(rp => rp.PermissionId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
