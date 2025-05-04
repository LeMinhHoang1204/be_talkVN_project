using Microsoft.EntityFrameworkCore.Metadata.Builders;

using TalkVN.Domain.Entities.SystemEntities.Relationships;

namespace TalkVN.DataAccess.Configurations
{
    public class UserGroupRoleConfiguration : IEntityTypeConfiguration<UserGroupRole>
    {
        public void Configure(EntityTypeBuilder<UserGroupRole> modelBuilder)
        {
            //primary key
            modelBuilder.HasKey(ugr => ugr.Id);

            // Configure relationship with User
            modelBuilder
                .HasOne(ugr => ugr.User)
                .WithMany()
                .HasForeignKey(ugr => ugr.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure relationship with Group
            modelBuilder
                .HasOne(ugr => ugr.Group)
                .WithMany(g => g.UserGroupRoles)
                .HasForeignKey(ugr => ugr.GroupId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure relationship with Role
            modelBuilder
                .HasOne(ugr => ugr.Role)
                .WithMany(r => r.UserGroupRoles)
                .HasForeignKey(ugr => ugr.RoleId)
                .OnDelete(DeleteBehavior.Cascade);

        }
    }
}
