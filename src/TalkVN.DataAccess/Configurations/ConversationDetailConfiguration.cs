using TalkVN.Domain.Entities.ChatEntities;

using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace TalkVN.DataAccess.Configurations
{
    public class ConversationDetailConfiguration : IEntityTypeConfiguration<ConversationDetail>
    {
        public void Configure(EntityTypeBuilder<ConversationDetail> modelBuilder)
        {
            // Conversation Detail
            modelBuilder
            .HasKey(c => c.Id);

            modelBuilder
                .HasOne(m => m.Conversation)
                .WithMany(c => c.ConversationDetails)
                .HasForeignKey(m => m.ConversationId);

            // Configure relationship with User
            modelBuilder
                .HasOne(m => m.User)
                .WithMany(u => u.ConversationDetails)
                .HasForeignKey(m => m.UserId);

        }
    }
}
