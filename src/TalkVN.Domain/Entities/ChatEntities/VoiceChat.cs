using TalkVN.Domain.Entities.SystemEntities.Permissions;
using TalkVN.Domain.Entities.SystemEntities.Relationships;

namespace TalkVN.Domain.Entities.ChatEntities;

public class VoiceChat : BaseAuditedEntity
{
    public string Name { get; set; } // required

    public string Password { get; set; } // required if IsPrivate is true

    // public GroupStatus Status { get; set; }

    // public ChatType Type { get; set; }
    public bool IsPrivate { get; set; } // required - default false

    public int MaxQuantity { get; set; } // default 20

    public bool CanShareScreen { get; set; } // default true

    public bool CanRecord { get; set; } // default true

    public IEnumerable<VoiceChatParticipant> VoiceChatParticipants { get; set; }

    public IEnumerable<UserChatRole> UserChatRoles { get; set; }

    public IEnumerable<VoiceChatPermission> VoiceChatPermissions { get; set; } // Navigation property

}
