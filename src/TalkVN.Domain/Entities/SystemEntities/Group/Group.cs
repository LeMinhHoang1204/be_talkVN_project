using TalkVN.Domain.Entities.SystemEntities.Notification;
using TalkVN.Domain.Entities.SystemEntities.Relationships;

namespace TalkVN.Domain.Entities.SystemEntities.Group
{
    public class Group : BaseAuditedEntity
    {
        public string Name { get; set; } // required
        public string Password { get; set; } // required if IsPrivate is true
        public bool IsPrivate { get; set; } // required
        public GroupStatus Status { get; set; } // default active
        public int MaxQuantity { get; set; } // default 20
        public string Description { get; set; } // optional
        public string? Avatar { get; set; } // optional
        public string Url { get; set; } // optional

        public string CreatorId { get; set; }

        public UserApplication Creator { get; set; } // Navigation property

        public IEnumerable<TextChat> TextChats { get; set; } //1 group co nhieu textChat
        public IEnumerable<UserGroupRole> UserGroupRoles { get; set; } //1 group co nhieu usergrouprole

        public IEnumerable<MeetingSchedule> MeetingSchedules { get; set; } //1 group co nhieu meeting schedule

        public IEnumerable<GroupNotifications> GroupNotifications { get; set; } //1 group co nhieu tag

        public IEnumerable<VoiceChatParticipant> VoiceChatParticipants { get; set; } //1 group co nhieu notification receiver
    }
}
