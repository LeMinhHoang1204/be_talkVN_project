using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace TalkVN.DataAccess.Configurations
{
    public class MessageConfiguration : IEntityTypeConfiguration<Message>
    {
        public void Configure(EntityTypeBuilder<Message> modelBuilder)
        {
            // Configure primary key
            modelBuilder.HasKey(m => m.Id);

            // Configure relationship with TextChat
            modelBuilder
                .HasOne(m => m.TextChat)
                .WithMany(c => c.Messages)
                .HasForeignKey(m => m.ConversationId);

            // Configure relationship with Sender
            modelBuilder
                .HasOne(m => m.Sender)
                .WithMany(s => s.Messages)
                .HasForeignKey(m => m.SenderId);
        }
    }
}
