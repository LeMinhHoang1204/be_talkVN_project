using TalkVN.Domain.Entities.SystemEntities.Notification;

using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace TalkVN.DataAccess.Configurations.Notification
{
    public class CommentNotificationConfiguration : IEntityTypeConfiguration<CommentNotifications>
    {
        public void Configure(EntityTypeBuilder<CommentNotifications> builder)
        {
            builder.HasKey(n => n.Id);
            // builder.HasOne(n => n.ReceiverUser)
            //     .WithMany()
            //     .HasForeignKey(n => n.ReceiverUserId);
            builder.HasOne(n => n.LastInteractorUser)
                .WithMany()
                .HasForeignKey(n => n.LastInteractorUserId);
            builder.HasOne(n => n.Comment)
                .WithMany()
                .HasForeignKey(n => n.CommentId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
