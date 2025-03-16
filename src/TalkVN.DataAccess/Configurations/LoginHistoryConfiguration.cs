using TalkVN.Domain.Entities;

using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace TalkVN.DataAccess.Configurations
{
    public class LoginHistoryConfiguration : IEntityTypeConfiguration<LoginHistory>
    {
        public void Configure(EntityTypeBuilder<LoginHistory> modelBuilder)
        {
            // Conversation Detail
            modelBuilder
            .HasKey(c => c.Id);
        }
    }
}
