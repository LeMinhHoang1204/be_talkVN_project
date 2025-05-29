namespace TalkVN.Domain.Entities.SystemEntities.Permissions;

public class OverridePermission : BaseAuditedEntity
{
    public string UserId { get; set; }

    public Guid TextChatId { get; set; }

    public Guid PermissionId { get; set; }

    public bool IsAllowed { get; set; }

    public Permission Permission { get; set; }

    public TextChat TextChat { get; set; }

    public UserApplication User { get; set; }
}
