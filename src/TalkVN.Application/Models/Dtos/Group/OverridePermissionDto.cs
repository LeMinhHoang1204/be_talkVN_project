namespace TalkVN.Application.Models.Dtos.Group;

public class OverridePermissionDto
{
    public string UserId { get; set; }
    public Guid PermissionId { get; set; }
    public Guid TextChatId { get; set; }
    public bool IsAllowed { get; set; }
}
