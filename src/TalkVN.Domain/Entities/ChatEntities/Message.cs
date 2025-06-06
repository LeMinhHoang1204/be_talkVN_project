public class Message : BaseAuditedEntity
{
    public Guid TextChatId { get; set; }
    public string SenderId { get; set; }
    public string MessageText { get; set; }
    public string Status { get; set; }

    public bool IsSeen { get; set; }
    public TextChat TextChat { get; set; } // Navigation property
    public UserApplication Sender { get; set; }

    // public string title {get ; set;}
    // public string content {get ; set;}
}
