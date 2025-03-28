using Microsoft.EntityFrameworkCore.Metadata.Builders;

using TalkVN.Domain.Entities.ChatEntities;

namespace TalkVN.DataAccess.Configurations.Chat
{

    public class VoiceChatConfiguration : IEntityTypeConfiguration<VoiceChat>
    {
        public void Configure(EntityTypeBuilder<VoiceChat> builder)
        {
            // Primary key
            builder.HasKey(vc => vc.Id);

            // Configure relationship with VoiceChatParticipant
            builder
                .HasMany(vc => vc.VoiceChatParticipants)
                .WithOne(vcp => vcp.VoiceChat)
                .HasForeignKey(vcp => vcp.VoiceChatId)
                .OnDelete(DeleteBehavior.Cascade);

            //configure relationship with UserChatRole
            builder
                .HasMany(vc => vc.UserChatRoles)
                .WithOne(ucr => ucr.VoiceChat)
                .HasForeignKey(ucr => ucr.VoiceChatId)
                .OnDelete(DeleteBehavior.Cascade);

            //configure relationship with VoiceChatPermission
            builder
                .HasMany(vc => vc.VoiceChatPermissions)
                .WithOne(vcp => vcp.VoiceChat)
                .HasForeignKey(vcp => vcp.VoiceChatId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
