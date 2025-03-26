using TalkVN.Domain.Entities.SystemEntities.Relationships;

namespace TalkVN.Domain.Entities.SystemEntities.Group
{
    public class Group : BaseAuditedEntity
    {
        public string Name { get; set; }
        public string Password { get; set; }
        public bool IsPrivate { get; set; }
        public GroupStatus Status { get; set; }
        public int MaxQuantity { get; set; }
        public string Description { get; set; }
        public string Avatar { get; set; }

        public string CreatorId { get; set; }

        public UserApplication Creator { get; set; } // Navigation property

        public IEnumerable<Conversation> Conversations { get; set; } //1 group co nhieu conversation
        public IEnumerable<UserGroupRole> UserGroupRoles { get; set; } //1 group co nhieu usergrouprole
    }
}
