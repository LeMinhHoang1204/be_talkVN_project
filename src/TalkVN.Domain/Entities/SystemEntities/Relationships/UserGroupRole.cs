namespace TalkVN.Domain.Entities.SystemEntities.Relationships
{
    public class UserGroupRole : BaseAuditedEntity
    {
        public string UserId { get; set; }

        public Guid GroupId { get; set; }

        public string RoleId { get; set; }

        public string AcceptedBy { get; set; }

        public string InvitedBy { get; set; }

        public ApplicationRole Role { get; set; } // Navigation property
        public Group.Group Group { get; set; } // Navigation property
        public UserApplication User { get; set; } // Navigation property
    }
}
