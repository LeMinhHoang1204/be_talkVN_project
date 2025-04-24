namespace TalkVN.Domain.Entities.SystemEntities.Group;

public class GroupInvitation : BaseAuditedEntity
{
    public Guid GroupId { get; set; }
    public string InviterId { get; set; }
    public string InvitationCode { get; set; }
    public DateTime ExpirationDate { get; set; }

    // Navigation properties
    public Group Group { get; set; }
    public UserApplication Inviter { get; set; }
    public IEnumerable<JoinGroupRequest> JoinGroupRequests { get; set; } // List of users invited to the group
}
