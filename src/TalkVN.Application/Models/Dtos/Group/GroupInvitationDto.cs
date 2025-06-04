using TalkVN.Application.Models.Dtos.User;
using TalkVN.Domain.Identity;

namespace TalkVN.Application.Models.Dtos.Group;

public class GroupInvitationDto : BaseResponseDto
{
    public string InvitationCode { get; set; }
    public string InvitationUrl { get; set; } // talkvn-id-vn/invitation/abcxyz
    public DateTime ExpirationDate { get; set; }
    public DateTime CreatedDate { get; set; }
    public Guid GroupId { get; set; }
    public string InviterId { get; set; }

    public UserDto Inviter { get; set; } // Navigation property to the inviter user
}
