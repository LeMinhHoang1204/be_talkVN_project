using TalkVN.Domain.Entities.SystemEntities.Notification;

using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace TalkVN.DataAccess.Configurations.Notification
{
    public class PostNotificationConfiguration : IEntityTypeConfiguration<PostNotifications>
    {
        public void Configure(EntityTypeBuilder<PostNotifications> builder)
        {
            builder.HasKey(n => n.Id);
            // builder.HasOne(n => n.ReceiverUser)
            //     .WithMany()
            //     .HasForeignKey(n => n.ReceiverUserId);
            builder.HasOne(n => n.LastInteractorUser)
                .WithMany()
                .HasForeignKey(n => n.LastInteractorUserId);
            builder.HasOne(n => n.Post)
                .WithMany()
                .HasForeignKey(n => n.PostId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
