using Microsoft.AspNetCore.Http;

namespace TalkVN.Application.Models.Dtos.Post.CreatePost
{
    public class CreatePostRequestDto
    {
        public List<IFormFile> Files { get; set; }
        public string Description { get; set; }
    }
}
