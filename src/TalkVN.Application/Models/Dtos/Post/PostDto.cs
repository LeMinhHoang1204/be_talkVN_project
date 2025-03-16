using TalkVN.Application.Models.Dtos.Post.Comments;
using TalkVN.Application.Models.Dtos.User;

namespace TalkVN.Application.Models.Dtos.Post
{

    public class PostDto : BaseResponseDto
    {
        public string Description { get; set; } = "";
        public int ReactionCount { get; set; }
        public int CommentCount { get; set; }
        public ICollection<CommentDto> Comments { get; set; }
        public ICollection<PostMediaDto> PostMedias { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
        public bool IsReacted { get; set; }
        public UserDto UserPosted
        { get; set; }
    }
}

