using TalkVN.DataAccess.Repositories.Interface;
using TalkVN.Domain.Entities.UserEntities;
using TalkVN.Domain.Identity;

namespace TalkVN.DataAccess.Repositories.Interface
{
    public interface IUserFollowerRepository : IBaseRepository<UserFollower>
    {
        Task<List<UserApplication>> GetRecommendedUsersAsync(string currentUserId, int pageSize);
    }
}
