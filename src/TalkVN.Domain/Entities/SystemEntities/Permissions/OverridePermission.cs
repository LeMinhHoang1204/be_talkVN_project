namespace TalkVN.Domain.Entities.SystemEntities.Permissions;

public abstract class OverridePermission : BaseAuditedEntity
{
    public string UserId { get; set; }

    public Guid PermissionId { get; set; }

    public bool IsAllowed { get; set; }
    public string Type { get; set; }

    public Permission Permission { get; set; }

    public UserApplication User { get; set; }
}
