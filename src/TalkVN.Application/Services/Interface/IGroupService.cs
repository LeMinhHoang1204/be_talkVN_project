using TalkVN.Application.Models;
using TalkVN.Application.Models.Dtos.Group;
using TalkVN.Application.Models.Dtos.User;


namespace TalkVN.Application.Services.Interface
{
    public interface IGroupService
    {
        Task<List<GroupDto>> GetAllGroupsAsync(PaginationFilter query);
        Task<GroupDto> CreateGroupAsync(RequestCreateGroupDto request);
        //Task<List<UserDto>> GetMembersByGroupIdAsync(string groupId);
    }
}
