namespace TalkVN.Domain.Entities.SystemEntities.Relationships;

public class MeetingSchedule : BaseAuditedEntity
{
    public string CreatorId { get; set; }

    public Guid GroupId { get; set; }

    public DateTime StartTime { get; set; }

    public int Duration { get; set; }

    public Group.Group Group { get; set; } // Navigation property
    public UserApplication Creator { get; set; } // Navigation property

}
