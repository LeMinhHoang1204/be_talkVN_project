using System.Text.Json.Serialization;

using TalkVN.Application.Models.Dtos.User;

namespace TalkVN.Application.Models.Dtos.Group;

public class JoinGroupRequestDto : BaseResponseDto
{
    public Guid GroupId { get; set; }
    public string InvitationCode { get; set; }
    public UserDto? User { get; set; }
    public GroupInvitationDto? GroupInvitation { get; set; }
}
