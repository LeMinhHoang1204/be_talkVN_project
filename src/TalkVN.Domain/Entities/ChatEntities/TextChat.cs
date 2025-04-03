using TalkVN.Domain.Entities.ChatEntities;
using TalkVN.Domain.Entities.SystemEntities.Group;
using TalkVN.Domain.Entities.SystemEntities.Permissions;
using TalkVN.Domain.Entities.SystemEntities.Relationships;

public class TextChat : BaseAuditedEntity
{
    public Guid? LastMessageId { get; set; }
    //public string userId1 { get; set; }
    //public string userId2 { get; set; }

    public Message? LastMessage { get; set; }
    public bool IsSeen { get; set; }
    public int NumOfUser { get; set; }
    public string TextChatType { get; set; }

    public Guid? GroupId { get; set; }


    public Group? Group { get; set; }
    public IEnumerable<Message> Messages { get; set; }
    public IEnumerable<TextChatParticipant> TextChatParticipants { get; set; }
    public IEnumerable<TextChatPermission> TextChatPermissions { get; set; } // Navigation property

    public IEnumerable<UserChatRole> UserChatRoles { get; set; }
}
