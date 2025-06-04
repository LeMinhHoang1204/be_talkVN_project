namespace TalkVN.Application.Models.Dtos.Group;

public class ActionJoinGroupRequestDto
{
    public Guid GroupId { get; set; }
    public string InvitationCode { get; set; }
}
