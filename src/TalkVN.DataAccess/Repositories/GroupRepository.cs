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

    }
}
