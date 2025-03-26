using Microsoft.EntityFrameworkCore.Metadata.Builders;

using TalkVN.Domain.Entities.SystemEntities.Relationships;

namespace TalkVN.DataAccess.Configurations;

public class MeetingScheduleConfiguration : IEntityTypeConfiguration<MeetingSchedule>
{
    public void Configure(EntityTypeBuilder<MeetingSchedule> modelBuilder)
    {
        //primary key
        modelBuilder.HasKey(ms => ms.Id);

        // Configure relationship with User
        modelBuilder
            .HasOne(ms => ms.Creator)
            .WithMany()
            .HasForeignKey(ms => ms.CreatorId)
            .OnDelete(DeleteBehavior.Cascade);


    }
}
