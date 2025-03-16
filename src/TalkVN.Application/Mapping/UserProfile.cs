using AutoMapper;

using TalkVN.Application.Models.Dtos.User;
using TalkVN.Application.Models.Dtos.User.Profile;
using TalkVN.Domain.Identity;

namespace TalkVN.Application.Mapping
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<UserApplication, UserDto>();
            CreateMap<RegisterationRequestDto, UserApplication>();
            CreateMap<LoginRequestDto, UserApplication>();
            CreateMap<UserApplication, ProfileDto>();
            CreateMap<ProfileDto, TalkVN.Domain.Entities.UserEntities.Profile>();
            CreateMap<ProfileRequestDto, TalkVN.Domain.Entities.UserEntities.Profile>();
            CreateMap<TalkVN.Domain.Entities.UserEntities.Profile, ProfileDto>()
               .ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.UserApplication != null ? src.UserApplication.DisplayName : string.Empty))
               .ForMember(dest => dest.AvatarUrl, opt => opt.MapFrom(src => src.UserApplication != null ? src.UserApplication.AvatarUrl : string.Empty))
               .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.UserApplication != null ? src.UserApplication.Email : string.Empty))
               .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.UserApplication != null ? src.UserApplication.PhoneNumber : string.Empty));
            CreateMap<ProfileRequestDto, UserApplication>();
        }
    }
}
