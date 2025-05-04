namespace TalkVN.Domain.Entities.SystemEntities.Notification
{
    public class GroupNotifications : Notifications
    {
        public Guid GroupId { get; set; }

        public Group.Group Group { get; set; }

        public GroupNotifications()
        {
            Type = NotificationType.Group.ToString();
        }

        public IEnumerable<NotificationReceivers> NotificationReceivers { get; set; } // Navigation property
    }
}
