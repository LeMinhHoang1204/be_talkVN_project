namespace TalkVN.Domain.Entities.SystemEntities.Notification
{
    public class PostNotifications : Notifications
    {
        public Guid PostId { get; set; }
        public Post Post { get; set; } // Navigation property
        public PostNotifications()
        {
            Type = NotificationType.Post.ToString();
        }
    }
}
