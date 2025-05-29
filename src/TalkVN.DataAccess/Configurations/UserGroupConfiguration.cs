using Microsoft.EntityFrameworkCore.Metadata.Builders;

using TalkVN.Domain.Entities.SystemEntities.Relationships;

using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace TalkVN.DataAccess.Configurations
{
    public class UserGroupConfiguration : IEntityTypeConfiguration<UserGroup>
    {
        public void Configure(EntityTypeBuilder<UserGroup> modelBuilder)
        {
            //primary key
            modelBuilder.HasKey(ugr => ugr.Id);

            // Configure relationship with Group
            modelBuilder
                .HasOne(ugr => ugr.Group)
                .WithMany(g => g.UserGroups)
                .HasForeignKey(ugr => ugr.GroupId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure relationship with User
            modelBuilder
                .HasOne(ugr => ugr.User)
                .WithMany(u => u.UserGroups)
                .HasForeignKey(ugr => ugr.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            //Configure relationship with UserGroupRole
            modelBuilder
                .HasMany(ugr => ugr.UserGroupRoles)
                .WithOne(ugr => ugr.UserGroup)
                .HasForeignKey(ugr => ugr.UserGroupId)
                .OnDelete(DeleteBehavior.Cascade);

        }
    }
}
