using TalkVN.Application.MachineLearning.Models;
using TalkVN.DataAccess.Data;
using TalkVN.DataAccess.Repositories.Interface;
using TalkVN.Domain.Entities.SystemEntities;
using TalkVN.Domain.Enums;

namespace TalkVN.DataAccess.Repositories
{
    public class UserInteractionRepository : BaseRepository<UserInteraction>, IUserInteractionRepository
    {
        public UserInteractionRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<List<UserInteractionModelItem>> GetUserInteractionModelForTraining(int pageSize)
        {
            var data = await DbSet
                .OrderByDescending(p => p.InteractionDate)
                .Include(p => p.Post)
                .Take(pageSize)
                .ToListAsync(); // Lấy dữ liệu từ database trước

            var userModelItems = data
                .GroupBy(p => new
                {
                    p.UserId,
                    p.PostId,
                    PostDescription = p.Post.Description
                })
                .Select(g => new UserInteractionModelItem
                {
                    UserId = g.Key.UserId,
                    PostId = g.Key.PostId.ToString(),
                    PostDescription = g.Key.PostDescription,
                    TotalPoint = g.Sum(x => InteractionTypePoint.GetInteractionPoint(x))
                })
                .ToList();
            return userModelItems;
        }
    }
}
