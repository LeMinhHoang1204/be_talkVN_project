using Microsoft.EntityFrameworkCore.Metadata.Builders;

using TalkVN.Domain.Entities.SystemEntities.Relationships;

namespace TalkVN.DataAccess.Configurations.Chat
{

    public class VoiceChatParticipantConfiguration : IEntityTypeConfiguration<VoiceChatParticipant>
    {
        public void Configure(EntityTypeBuilder<VoiceChatParticipant> builder)
        {
            // Primary Key
            builder.HasKey(vcp => vcp.Id);

            //configure relationships with VoiceChat
            builder
                .HasOne(vcp => vcp.VoiceChat)
                .WithMany(vc => vc.VoiceChatParticipants)
                .HasForeignKey(vcp => vcp.VoiceChatId);

            //configure relationships with Group
            builder
                .HasOne(vcp => vcp.Group)
                .WithMany(g => g.VoiceChatParticipants)
                .HasForeignKey(vcp => vcp.GroupId);


        }
    }
}
