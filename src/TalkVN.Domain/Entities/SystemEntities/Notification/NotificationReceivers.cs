namespace TalkVN.Domain.Entities.SystemEntities.Notification
{
    public class NotificationReceivers : BaseAuditedEntity
    {
        public Guid GroupNotificationId { get; set; }
        public string ReceiverId { get; set; }
        public bool IsRead { get; set; }

        public GroupNotifications GroupNotifications { get; set; } // Navigation property
        public UserApplication Receiver { get; set; } // Navigation property
    }
}
