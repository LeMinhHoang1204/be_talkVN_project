namespace TalkVN.Application.Models.Dtos.Group;

public class SendGroupInviteDto
{
    public Guid GroupId { get; set; }
    public string TargetUserId { get; set; }
    public string SenderUserId { get; set; }
}
