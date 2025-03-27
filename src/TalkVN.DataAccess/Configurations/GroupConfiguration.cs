using Microsoft.EntityFrameworkCore.Metadata.Builders;

using TalkVN.Domain.Entities.SystemEntities.Group;


namespace TalkVN.DataAccess.Configurations
{
    public class GroupConfiguration : IEntityTypeConfiguration<Group>
    {
        public void Configure(EntityTypeBuilder<Group> modelBuilder)
        {
            //primary key
            modelBuilder.HasKey(g => g.Id);

            // Configure relationship with User
            modelBuilder
                .HasOne(g => g.Creator)
                .WithMany(u => u.Groups)
                .HasForeignKey(g => g.CreatorId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure relationship with Conversation
            modelBuilder
                .HasMany(g => g.Conversations)
                .WithOne(c => c.Group)
                .HasForeignKey(c => c.GroupId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure relationship with UserGroupRole
            modelBuilder
                .HasMany(g => g.UserGroupRoles)
                .WithOne(ugr => ugr.Group)
                .HasForeignKey(ugr => ugr.GroupId)
                .OnDelete(DeleteBehavior.Cascade);

            //configure relationship with GroupNotification
            modelBuilder
                .HasMany(g => g.GroupNotifications)
                .WithOne(gn => gn.Group)
                .HasForeignKey(gn => gn.GroupId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
