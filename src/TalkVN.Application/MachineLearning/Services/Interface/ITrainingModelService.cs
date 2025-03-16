using TalkVN.Application.MachineLearning.Models;
using TalkVN.Domain.Entities.PostEntities;

namespace TalkVN.Application.MachineLearning.Services.Interface
{
    public interface ITrainingModelService
    {
        List<ResponseRecommendationModel> GetRecommendationPostModel(string userId, List<UserInteractionModelItem> userInteractions, List<Post> postReccomendation);
    }
}
