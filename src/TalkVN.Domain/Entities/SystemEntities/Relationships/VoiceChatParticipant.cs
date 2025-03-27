namespace TalkVN.Domain.Entities.SystemEntities.Relationships;

public class VoiceChatParticipant : BaseEntity
{
    public Guid GroupId { get; set; }

    public Guid VoiceChatId { get; set; }

    public ParticipantStatus Status { get; set; }
}
