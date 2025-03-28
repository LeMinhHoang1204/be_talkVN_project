using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace TalkVN.DataAccess.Configurations
{
    public class TextChatConfiguration : IEntityTypeConfiguration<TextChat>
    {
        public void Configure(EntityTypeBuilder<TextChat> modelBuilder)
        {
            // Configure primary key
            modelBuilder.HasKey(c => c.Id);

            // Configure relationship with LastMessage
            modelBuilder
                .HasOne(c => c.LastMessage)
                .WithMany()
                .HasForeignKey(c => c.LastMessageId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure relationship with Messages
            modelBuilder
                .HasMany(c => c.Messages)
                .WithOne(m => m.TextChat)
                .HasForeignKey(m => m.ConversationId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure relationship with TextChatParticipants
            modelBuilder
                .HasMany(c => c.TextChatParticipants)
                .WithOne(cd => cd.TextChat)
                .HasForeignKey(cd => cd.ConversationId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure relationship with Group
            modelBuilder
                .HasOne(c => c.Group)
                .WithMany(g => g.TextChats)
                .HasForeignKey(c => c.GroupId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
