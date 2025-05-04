using Microsoft.EntityFrameworkCore.Metadata.Builders;

using TalkVN.Domain.Entities.SystemEntities.Permissions;

namespace TalkVN.DataAccess.Configurations.Permissions;

public class VoiceChatPermissionConfiguration : IEntityTypeConfiguration<VoiceChatPermission>
{
    public void Configure(EntityTypeBuilder<VoiceChatPermission> builder)
    {
       // builder.ToTable("VoiceChatPermissions");

        builder.HasKey(x => x.Id);

        //configure relationship with VoiceChat
        builder
            .HasOne(x => x.VoiceChat)
            .WithMany(u => u.VoiceChatPermissions)
            .HasForeignKey(x => x.VoiceChatId)
            .OnDelete(DeleteBehavior.Cascade);

        //Configure relationship with User
        builder
            .HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        //configure relationship with Permission
        builder
            .HasOne(x => x.Permission)
            .WithMany()
            .HasForeignKey(x => x.PermissionId)
            .OnDelete(DeleteBehavior.Cascade);

    }
}
