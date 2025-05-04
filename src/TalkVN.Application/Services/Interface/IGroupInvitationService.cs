using TalkVN.Application.Models.Dtos.Group;

namespace TalkVN.Application.Services.Interface;

public interface IGroupInvitationService
{
    Task<GroupInvitationDto> CreateGroupInvitationAsync(Guid groupId, string userId);
}
