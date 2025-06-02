using TalkVN.Application.Models.Dtos.User;
using TalkVN.Domain.Enums;

namespace TalkVN.Application.Models.Dtos.Group;

public class UserGroupDto
{
    public string UserId { get; set; }

    public UserDto User { get; set; }

    public Guid GroupId { get; set; }

    public GroupStatus Status { get; set; }
}
