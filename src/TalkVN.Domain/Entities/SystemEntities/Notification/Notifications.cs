
namespace TalkVN.Domain.Entities.SystemEntities.Notification
{
    public abstract class Notifications : BaseAuditedEntity
    {
        public string Type { get; set; }
        public string Action { get; set; }

        public string Reference { get; set; }
        public string LastInteractorUserId { get; set; }
        public UserApplication LastInteractorUser { get; set; } // Navigation property
        //public UserApplication ReceiverUser { get; set; } // Navigation property

        //public IEnumerable<NotificationReceivers> NotificationReceivers { get; set; } // Navigation property
    }
}
