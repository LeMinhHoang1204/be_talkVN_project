using AutoMapper;

using TalkVN.Application.MachineLearning.Models;
using TalkVN.Application.Models.Dtos.Post;
using TalkVN.Application.Models.Dtos.Post.CreatePost;
using TalkVN.Domain.Entities.PostEntities;

namespace TalkVN.Application.Mapping
{
    public class PostProfile : Profile
    {
        public PostProfile()
        {
            CreateMap<Post, PostDto>().ForMember(dest => dest.UserPosted, opt => opt.MapFrom(src => src.User));
            CreateMap<CreatePostRequestDto, Post>();
            CreateMap<UpdatePostRequestDto, Post>();
            CreateMap<CreatePostMediaRequestDto, PostMedia>();
            CreateMap<PostMedia, PostMediaDto>();
            CreateMap<PostMediaDto, PostMedia>();
            // CreateMap<ResponseRecommendationModel, PostDto>()
                // .ConvertUsing(src => src.Post);

            //

        }
    }
}
