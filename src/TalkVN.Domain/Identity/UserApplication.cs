using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Identity;

using TalkVN.Domain.Entities.ChatEntities;
using TalkVN.Domain.Entities.SystemEntities.Group;

namespace TalkVN.Domain.Identity
{
    public class UserApplication : IdentityUser
    {
        public DateTime LastLogin { get; set; }
        [MaxLength(255)]
        public string DisplayName { get; set; }
        [MaxLength(255)]
        public string? AvatarUrl { get; set; }
        public string? MediaAvatarType { get; set; }
        public UserStatus UserStatus { get; set; }

        public IEnumerable<Message> Messages { get; set; } // Navigation property
        public IEnumerable<TextChatParticipant> TextChatParticipants { get; set; } // Navigation property
        public IEnumerable<GroupInvitation> GroupInvitations { get; set; } // Navigation property
        public IEnumerable<JoinGroupRequest> JoinGroupRequests { get; set; } // Navigation property

        public IEnumerable<Group> Groups { get; set; } // Navigation property
    }
}
