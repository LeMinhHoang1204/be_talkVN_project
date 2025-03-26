using Microsoft.EntityFrameworkCore.Metadata.Builders;

using TalkVN.Domain.Identity;

namespace TalkVN.DataAccess.Configurations.Permissions
{

    public class RoleConfiguration : IEntityTypeConfiguration<ApplicationRole>
    {
        public void Configure(EntityTypeBuilder<ApplicationRole> builder)
        {
            builder.HasKey(x => x.Id);

            // Configure relationship with RolePermission
            builder.HasMany(x => x.RolePermissions)
                .WithOne(x => x.Role)
                .HasForeignKey(x => x.RoleId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
