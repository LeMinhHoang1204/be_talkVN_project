namespace TalkVN.Domain.Entities.SystemEntities.Group;

public class JoinGroupRequest : BaseAuditedEntity
{
    public Guid GroupId { get; set; }
    public string RequestedUserId { get; set; }
    public Guid? InvitationId { get; set; }
    public string Status { get; set; }

    //navigation property
    public Group Group { get; set; }
    public UserApplication RequestedUser { get; set; }
    public GroupInvitation Invitation { get; set; }
}
