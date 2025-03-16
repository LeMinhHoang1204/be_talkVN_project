using AutoMapper;

using TalkVN.Application.Models.Dtos.Post.Comments;
using TalkVN.Domain.Entities.PostEntities;

namespace TalkVN.Application.Mapping
{
    public class CommentProfile : Profile
    {
        public CommentProfile()
        {
            CreateMap<Comment, CommentDto>();
            CreateMap<CommentRequestDto, Comment>();
        }
    }
}
