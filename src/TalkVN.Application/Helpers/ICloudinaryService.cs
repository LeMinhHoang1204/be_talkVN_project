using TalkVN.Application.Models.Dtos.Post;

using Microsoft.AspNetCore.Http;

namespace TalkVN.Application.Helpers
{
    public interface ICloudinaryService
    {
        Task<List<PostMediaDto>> PostMediaToCloudAsync(List<IFormFile> files, Guid postId);
        Task<bool> DeleteMediaFromCloudAsync(Guid postId, Guid postMediaId);
    }
}
