using TalkVN.Application.MachineLearning.Models;
using TalkVN.DataAccess.Repositories.Interface;
using TalkVN.Domain.Entities.SystemEntities;

namespace TalkVN.DataAccess.Repositories.Interface
{
    public interface IUserInteractionRepository : IBaseRepository<UserInteraction>
    {
        Task<List<UserInteractionModelItem>> GetUserInteractionModelForTraining(int PageSize);
    }
}
