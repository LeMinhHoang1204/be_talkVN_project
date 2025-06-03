using TalkVN.Domain.Enums;

namespace TalkVN.Application.Models.Dtos.Group;

public class UpdateUserRoleInGroupDto
{
    public Guid GroupId { get; set; }

    public string UserId { get; set; }

    public string RoleId { get; set; }
}
