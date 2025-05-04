namespace TalkVN.Domain.Entities.SystemEntities.Notification
{
    public class CommentNotifications : Notifications
    {
        public Guid CommentId { get; set; }
        public Comment Comment { get; set; } // Navigation property
        public CommentNotifications()
        {
            Type = NotificationType.Comment.ToString();
        }
    }
}
