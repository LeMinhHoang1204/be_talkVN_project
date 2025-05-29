namespace TalkVN.Domain.Entities.SystemEntities.Relationships
{
    public class UserGroupRole : BaseEntity
    {
        public Guid UserGroupId { get; set; }

        public string RoleId { get; set; }


        public UserGroup UserGroup { get; set; } // Navigation property
        public ApplicationRole Role { get; set; } // Navigation property
    }
}
