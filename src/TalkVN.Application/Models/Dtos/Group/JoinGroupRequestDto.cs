namespace TalkVN.Application.Models.Dtos.Group;

public class JoinGroupRequestDto
{
    public Guid GroupId { get; set; }
    public string InvitationCode { get; set; }
}
