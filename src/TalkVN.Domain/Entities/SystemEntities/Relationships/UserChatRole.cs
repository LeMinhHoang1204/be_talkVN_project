using TalkVN.Domain.Entities.ChatEntities;

namespace TalkVN.Domain.Entities.SystemEntities.Relationships;

public class UserChatRole : BaseEntity
{
    public Guid ConversationId { get; set; }

    public string UserId { get; set; }

    public Guid VoiceChatId { get; set; }

    public string RoleId { get; set; }

    public Conversation Conversation { get; set; }
    public UserApplication User { get; set; }
    public ApplicationRole Role { get; set; }
    public VoiceChat VoiceChat { get; set; }
}
