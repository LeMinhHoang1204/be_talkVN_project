using Microsoft.EntityFrameworkCore.Metadata.Builders;

using TalkVN.Domain.Entities.SystemEntities.Notification;

namespace TalkVN.DataAccess.Configurations.Notification
{

    public class GroupNotificationConfiguration : IEntityTypeConfiguration<GroupNotifications>
    {
        public void Configure(EntityTypeBuilder<GroupNotifications> builder)
        {
           // builder.ToTable("GroupNotifications");
            // Primary key
            builder.HasKey(x => x.Id);

            builder
                .HasOne(x => x.Group)
                .WithMany(g => g.GroupNotifications)
                .HasForeignKey(x => x.GroupId)
                .OnDelete(DeleteBehavior.Cascade);

            //config relationship with NotificationReceivers
            builder
                .HasMany(x => x.NotificationReceivers)
                .WithOne(nr => nr.GroupNotifications)
                .HasForeignKey(nr => nr.GroupNotificationId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
