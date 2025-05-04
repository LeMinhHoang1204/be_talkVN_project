using TalkVN.Domain.Entities.ChatEntities;

using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace TalkVN.DataAccess.Configurations
{
    public class TextChatParticipantConfiguration : IEntityTypeConfiguration<TextChatParticipant>
    {
        public void Configure(EntityTypeBuilder<TextChatParticipant> modelBuilder)
        {
            // TextChat Detail
            modelBuilder
            .HasKey(c => c.Id);

            modelBuilder
                .HasOne(m => m.TextChat)
                .WithMany(c => c.TextChatParticipants)
                .HasForeignKey(m => m.TextChatId);

            // Configure relationship with User
            modelBuilder
                .HasOne(m => m.User)
                .WithMany(u => u.TextChatParticipants)
                .HasForeignKey(m => m.UserId);

        }
    }
}
