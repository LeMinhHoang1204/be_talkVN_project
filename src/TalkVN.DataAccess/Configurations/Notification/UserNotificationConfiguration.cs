using TalkVN.Domain.Entities.SystemEntities.Notification;

using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace TalkVN.DataAccess.Configurations.Notification
{
    public class UserNotificationConfiguration : IEntityTypeConfiguration<UserNotifications>
    {
        public void Configure(EntityTypeBuilder<UserNotifications> builder)
        {
            builder.HasKey(n => n.Id);
            builder
                .HasOne(n => n.ReceiverUser)
                .WithMany()
                .HasForeignKey(n => n.ReceiverUserId);
            builder.
                HasOne(n => n.LastInteractorUser)
                .WithMany()
                .HasForeignKey(n => n.LastInteractorUserId);
        }
    }
}
