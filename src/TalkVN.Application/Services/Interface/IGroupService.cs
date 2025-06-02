using TalkVN.Application.Models;
using TalkVN.Application.Models.Dtos.Group;
using TalkVN.Application.Models.Dtos.User;


namespace TalkVN.Application.Services.Interface
{
    public interface IGroupService
    {
        Task<List<GroupDto>> GetAllGroupsAsync(PaginationFilter query);
        Task<GroupDto> CreateGroupAsync(RequestCreateGroupDto request);
        Task<List<UserGroupRoleDto>> GetMembersByGroupIdAsync(Guid groupId);
        Task<GroupDto> GetGroupInfoByInvitationCodeAsync(string code);
        Task<JoinGroupRequestDto> RequestJoinGroupAsync(JoinGroupRequestDto dto);
        Task ApproveJoinGroupRequestAsync(RequestActionDto dto);

        Task AddUserToChatsAsync(Guid groupId, string userId);

        Task UpdateUserRoleInGroupAsync(UpdateUserRoleInGroupDto dto);
    }
}
