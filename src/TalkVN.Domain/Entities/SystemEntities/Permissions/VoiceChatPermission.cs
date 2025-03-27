using TalkVN.Domain.Entities.ChatEntities;

namespace TalkVN.Domain.Entities.SystemEntities.Permissions;

public class VoiceChatPermission : OverridePermission
{
    public Guid VoiceChatId { get; set; }

    public VoiceChatPermission()
    {
        Type = PermissionType.VoiceChat.ToString();
    }

    public VoiceChat VoiceChat { get; set; } // Navigation property
}
