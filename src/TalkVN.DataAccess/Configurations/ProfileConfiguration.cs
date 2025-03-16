using TalkVN.Domain.Entities.UserEntities;

using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace TalkVN.DataAccess.Configurations
{
    public class ProfileConfiguration : IEntityTypeConfiguration<Profile>
    {
        public ProfileConfiguration() { }

        public void Configure(EntityTypeBuilder<Profile> builder)
        {
            builder
                .HasOne(p => p.UserApplication)
                .WithOne()
                .HasForeignKey<Profile>(p => p.UserApplicationId);
        }
    }
}
