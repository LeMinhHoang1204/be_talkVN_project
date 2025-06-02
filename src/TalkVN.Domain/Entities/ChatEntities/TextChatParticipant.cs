namespace TalkVN.Domain.Entities.ChatEntities
{
    public class TextChatParticipant : BaseEntity
    {
        public Guid TextChatId { get; set; }
        public string UserId { get; set; }
        public GroupStatus Status { get; set; }
        public TextChat TextChat { get; set; }
        public UserApplication User { get; set; }
    }
}

