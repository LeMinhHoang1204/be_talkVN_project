namespace TalkVN.Domain.Entities.SystemEntities.Relationships
{
    public class UserGroup : BaseEntity
    {
        public string UserId { get; set; }

        public Guid GroupId { get; set; }

        public GroupStatus Status { get; set; }

        public string AcceptedBy { get; set; }

        public string InvitedBy { get; set; }

        public Group.Group Group { get; set; } // Navigation property

        public UserApplication User { get; set; } // Navigation property

        public IEnumerable<UserGroupRole> UserGroupRoles { get; set; }
    }
}
