using Microsoft.EntityFrameworkCore.Metadata.Builders;

using TalkVN.Domain.Entities.SystemEntities.Group;

namespace TalkVN.DataAccess.Configurations;

public class JoinGroupRequestConfiguration : IEntityTypeConfiguration<JoinGroupRequest>
{
    public void Configure(EntityTypeBuilder<JoinGroupRequest> modelBuilder)
    {
        //primary key
        modelBuilder.HasKey(jgr => jgr.Id);

        // Configure relationship with Group
        modelBuilder
            .HasOne(jgr => jgr.Group)
            .WithMany(g => g.JoinGroupRequests)
            .HasForeignKey(jgr => jgr.GroupId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure relationship with User
        modelBuilder
            .HasOne(jgr => jgr.RequestedUser)
            .WithMany(u => u.JoinGroupRequests)
            .HasForeignKey(jgr => jgr.RequestedUserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure relationship with GroupInvitation
        modelBuilder
            .HasOne(jgr => jgr.Invitation)
            .WithMany(gi => gi.JoinGroupRequests)
            .HasForeignKey(jgr => jgr.InvitationId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
