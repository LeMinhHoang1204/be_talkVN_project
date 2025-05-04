using TalkVN.Domain.Entities.SystemEntities.Group;

namespace TalkVN.DataAccess.Repositories.Interface
{
    public interface IGroupRepository : IBaseRepository<Group>
    {
        Task<bool> IsGroupNameExistsAsync(string name);
    }
}
