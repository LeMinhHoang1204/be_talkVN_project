using Microsoft.EntityFrameworkCore.Metadata.Builders;

using TalkVN.Domain.Entities.SystemEntities.Group;

namespace TalkVN.DataAccess.Configurations;

public class GroupInviteConfiguration : IEntityTypeConfiguration<GroupInvitation>
{
    public void Configure(EntityTypeBuilder<GroupInvitation> builder)
    {
        // Configure primary key
        builder.HasKey(g => g.Id);

        // Configure relationship with Group
        builder
            .HasOne(g => g.Group)
            .WithMany(g => g.GroupInvitations)
            .HasForeignKey(g => g.GroupId)
            .OnDelete(DeleteBehavior.Cascade);

        //Configure relationship with JoinGroupRequest
        builder
            .HasMany(g => g.JoinGroupRequests)
            .WithOne(jgr => jgr.Invitation)
            .HasForeignKey(g => g.InvitationId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure relationship with Inviter
        builder
            .HasOne(g => g.Inviter)
            .WithMany(u => u.GroupInvitations)
            .HasForeignKey(g => g.InviterId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
