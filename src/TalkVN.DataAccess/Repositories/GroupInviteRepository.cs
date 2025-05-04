using TalkVN.DataAccess.Data;
using TalkVN.DataAccess.Repositories.Interface;
using TalkVN.Domain.Entities.SystemEntities.Group;

namespace TalkVN.DataAccess.Repositories;

public class GroupInviteRepository : BaseRepository<GroupInvitation>, IGroupInviteRepository
{
    public GroupInviteRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<GroupInvitation?> GetUserInvitaionsByGroupId(Guid groupId, string userId)
    {
        var now = DateTime.UtcNow;
        var url = await Context.GroupInvitations
                                     .Where(inv => inv.GroupId == groupId
                                                                && inv.InviterId == userId
                                                                && inv.ExpirationDate > now)
                                     .OrderByDescending(inv => inv.CreatedOn) // optional: phòng trường hợp có nhiều bản ghi thì lấy cái mới nhất
                                     .FirstOrDefaultAsync();
        return url;
    }
}
