using TalkVN.Domain.Entities.SystemEntities.Permissions;
using TalkVN.Domain.Entities.SystemEntities.Relationships;

namespace TalkVN.Domain.Entities.ChatEntities;

public class VoiceChat : BaseAuditedEntity
{
    public string Name { get; set; }

    public string Password { get; set; }

    public GroupStatus Status { get; set; }

    public ChatType Type { get; set; }

    public int MaxQuantity { get; set; }

    public bool CanShareScreen { get; set; }

    public bool CanRecord { get; set; }

    public IEnumerable<VoiceChatParticipant> VoiceChatParticipants { get; set; }

    public IEnumerable<UserChatRole> UserChatRoles { get; set; }

    public IEnumerable<VoiceChatPermission> VoiceChatPermissions { get; set; } // Navigation property

}
