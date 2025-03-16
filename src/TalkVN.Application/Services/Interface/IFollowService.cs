using TalkVN.Application.Models;
using TalkVN.Application.Models.Dtos.User.Follow;

namespace TalkVN.Application.Services.Interface
{
    public interface IFollowService
    {
        Task<FollowDto> ToggleFollowAsync(string otherUserId);
        Task<List<FollowDto>> GetAllFollowingsAsync(string userId);
        Task<List<FollowDto>> GetAllFollowerAsync(string userId);
        Task<List<FollowDto>> GetRecommendFollowAsync(PaginationFilter filter);
    }
}
