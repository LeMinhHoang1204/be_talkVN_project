using TalkVN.DataAccess.Data;
using TalkVN.DataAccess.Repositories.Interface;
using TalkVN.Domain.Entities.SystemEntities.Group;

namespace TalkVN.DataAccess.Repositories
{
    public class GroupRepository : BaseRepository<Group>, IGroupRepository
    {
        public GroupRepository(ApplicationDbContext context) : base(context)
        {
        }
        public async Task<bool> IsGroupNameExistsAsync(string name)
        {
            return await Context.Groups.AnyAsync(g => g.Name == name);
        }

        public async Task<Group> GetGroupByInvitationCode(string code)
        {
            var group = await Context.GroupInvitations
                .Include(i => i.Group)
                .FirstOrDefaultAsync(i => i.InvitationCode == code && i.ExpirationDate >= DateTime.UtcNow);

            if (group == null)
                throw new Exception("Invalid link or expired invitation code");
            return group.Group;
        }

        public async Task<List<TextChat>> GetAllTextChatsByGroupIdAsync(Guid groupId)
        {
            return await Context.TextChats
                .Where(tc => tc.GroupId == groupId)
                .ToListAsync();
        }

    }
}
