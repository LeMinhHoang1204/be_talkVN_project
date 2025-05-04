namespace TalkVN.Domain.Entities.SystemEntities.Notification
{
    public class UserNotifications : Notifications
    {
        public string ReceiverUserId { get; set; }
        public UserApplication ReceiverUser { get; set; } // Navigation property
        public UserNotifications()
        {
            Type = NotificationType.User.ToString();
        }
    }
}
