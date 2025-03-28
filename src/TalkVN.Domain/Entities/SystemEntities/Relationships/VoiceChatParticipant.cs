using TalkVN.Domain.Entities.ChatEntities;
using TalkVN.Domain.Entities.SystemEntities.Permissions;

namespace TalkVN.Domain.Entities.SystemEntities.Relationships;

public class VoiceChatParticipant : BaseEntity
{
    public Guid GroupId { get; set; }

    public Guid VoiceChatId { get; set; }

    public ParticipantStatus Status { get; set; }

    public VoiceChat VoiceChat { get; set; } // Navigation property
    public Group.Group Group { get; set; } // Navigation property

}
