using TalkVN.Application.Models.Dtos.Post;

namespace TalkVN.Application.MachineLearning.Models
{
    public class ResponseRecommendationModel
    {
        public float Score { get; set; }
        public PostDto? Post { get; set; }
    }
}
