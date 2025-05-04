namespace TalkVN.Domain.Entities.SystemEntities.Permissions
{
    public class RolePermission : BaseEntity
    {
        public string RoleId { get; set; }

        public Guid PermissionId { get; set; }

        public ApplicationRole Role { get; set; } // Navigation property
        public Permission Permission { get; set; } // Navigation property
    }
}
