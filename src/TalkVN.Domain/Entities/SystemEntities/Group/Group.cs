﻿using TalkVN.Domain.Entities.SystemEntities.Notification;
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
        public string Url { get; set; }

        public string CreatorId { get; set; }

        public UserApplication Creator { get; set; } // Navigation property

        public IEnumerable<TextChat> TextChats { get; set; } //1 group co nhieu textChat
        public IEnumerable<UserGroupRole> UserGroupRoles { get; set; } //1 group co nhieu usergrouprole

        public IEnumerable<MeetingSchedule> MeetingSchedules { get; set; } //1 group co nhieu meeting schedule

        public IEnumerable<GroupNotifications> GroupNotifications { get; set; } //1 group co nhieu tag

        public IEnumerable<VoiceChatParticipant> VoiceChatParticipants { get; set; } //1 group co nhieu notification receiver
    }
}
