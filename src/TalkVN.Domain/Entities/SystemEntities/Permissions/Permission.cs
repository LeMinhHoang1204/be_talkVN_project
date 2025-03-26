namespace TalkVN.Domain.Entities.SystemEntities.Permissions
{
    public class Permission : BaseAuditedEntity
    {
        public string Name { get; set; }

        public IEnumerable<RolePermission> RolePermissions { get; set; }
    }
}
