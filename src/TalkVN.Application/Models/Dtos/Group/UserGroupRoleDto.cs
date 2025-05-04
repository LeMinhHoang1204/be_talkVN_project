using TalkVN.Application.Models.Dtos.User;

namespace TalkVN.Application.Models.Dtos.Group
{
    public class UserGroupRoleDto
    {
        public string UserId { get; set; }
        public UserDto User { get; set; }

        public Guid GroupId { get; set; }

        public string RoleId { get; set; }

        public string AcceptedBy { get; set; }

        public string InvitedBy { get; set; }
    }
}
