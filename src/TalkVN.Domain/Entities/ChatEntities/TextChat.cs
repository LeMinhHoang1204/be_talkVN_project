using TalkVN.Domain.Entities.ChatEntities;
using TalkVN.Domain.Entities.SystemEntities.Group;
using TalkVN.Domain.Entities.SystemEntities.Permissions;
using TalkVN.Domain.Entities.SystemEntities.Relationships;

public class TextChat : BaseAuditedEntity
{
    public string? Name { get; set; } // required
    public Guid? LastMessageId { get; set; } // Nullable to allow for no last message

    public Message? LastMessage { get; set; } // Navigation property
    public bool IsSeen { get; set; } // default false
    public int? NumOfUser { get; set; } // Not need
    public string TextChatType { get; set; } // required
    public Guid? GroupId { get; set; }

    public Group? Group { get; set; }
    public IEnumerable<Message> Messages { get; set; }
    public IEnumerable<TextChatParticipant> TextChatParticipants { get; set; }
}
