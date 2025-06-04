using TalkVN.Application.Models;
using TalkVN.Application.Models.Dtos.Conversation;
using TalkVN.Application.Models.Dtos.Group;
using TalkVN.Application.Models.Dtos.User;


namespace TalkVN.Application.Services.Interface
{
    public interface IGroupService
    {
        Task<List<GroupDto>> GetAllGroupsAsync(PaginationFilter query);
        Task<GroupDto> CreateGroupAsync(RequestCreateGroupDto request);

        Task<List<UserGroupDto>> GetMembersByGroupIdAsync(Guid groupId);
        Task<GroupDto> GetGroupInfoByInvitationCodeAsync(string code);

        Task<List<TextChatDto>> GetAllTextChatsByGroupIdAsync(Guid groupId, PaginationFilter query);

        Task<List<UserDto>> GetUsersByUsernamesAsync(List<string> usernames, PaginationFilter query);

        Task<ActionJoinGroupRequestDto> RequestJoinGroupAsync(ActionJoinGroupRequestDto dto);
        Task ApproveJoinGroupRequestAsync(RequestActionDto dto);

        Task RejectJoinGroupRequestAsync(RequestActionDto dto);
        Task AddUserToChatsAsync(Guid groupId, string userId);
        Task<List<GroupDto>> GetUserJoinedGroupsAsync(PaginationFilter query);

        Task UpdateUserRoleInGroupAsync(UpdateUserRoleInGroupDto dto);
        Task<List<JoinGroupRequestDto>> GetJoinGroupRequestsByGroupIdAsync(Guid groupId);
    }
}
