using TalkVN.Domain.Entities.SystemEntities.Group;

namespace TalkVN.DataAccess.Repositories.Interface;

public interface IGroupInviteRepository : IBaseRepository<GroupInvitation>
{
    Task<GroupInvitation?> GetUserInvitaionsByGroupId(Guid groupId, string userId);
}
