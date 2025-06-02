using TalkVN.Domain.Entities.SystemEntities.Group;

namespace TalkVN.DataAccess.Repositories.Interface
{
    public interface IGroupRepository : IBaseRepository<Group>
    {
        Task<bool> IsGroupNameExistsAsync(string name);
        Task<Group> GetGroupByInvitationCode(string code);

        Task<List<TextChat>> GetAllTextChatsByGroupIdAsync(Guid groupId);
        Task<List<Group>> GetUserJoinedGroupsAsync(string userId);
    }
}
