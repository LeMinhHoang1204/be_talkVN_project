using Microsoft.EntityFrameworkCore.Metadata.Builders;

using TalkVN.Domain.Entities.SystemEntities.Notification;

namespace TalkVN.DataAccess.Configurations.Notification
{

    public class NotificationReceiversConfiguration : IEntityTypeConfiguration<NotificationReceivers>
    {
        public void Configure(EntityTypeBuilder<NotificationReceivers> builder)
        {
            // Primary Key
            builder.HasKey(nr => nr.Id);

            //configure relationships with GroupNotification
            builder
                .HasOne(nr => nr.GroupNotifications)
                .WithMany(gn => gn.NotificationReceivers)
                .HasForeignKey(nr => nr.GroupNotificationId);

            //configure relationships with UserApplication
            builder
                .HasOne(nr => nr.Receiver)
                .WithMany()
                .HasForeignKey(nr => nr.ReceiverId);
        }
    }
}
