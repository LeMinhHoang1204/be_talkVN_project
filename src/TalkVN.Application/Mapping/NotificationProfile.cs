using AutoMapper;

using TalkVN.Application.Models.Dtos.Notification;
using TalkVN.Domain.Entities.SystemEntities.Notification;

namespace TalkVN.Application.Mapping
{
    public class NotificationProfile : Profile
    {
        public NotificationProfile()
        {
            CreateMap<Notifications, NotificationDto>().ReverseMap();
            CreateMap<CommentNotifications, NotificationDto>()
                .ForMember(dest => dest.Post, opt => opt.MapFrom(p => p.Comment.Post))
                .ForPath(dest => dest.Post.Comments, opt => opt.Ignore());


            CreateMap<PostNotifications, NotificationDto>();
            CreateMap<NotificationDto, CommentNotifications>();
            CreateMap<CreateCommentNotificationDto, CommentNotifications>().ReverseMap();
            CreateMap<CreatePostNotificationDto, PostNotifications>().ReverseMap();
            CreateMap<CreateUserNotificationDto, UserNotifications>().ReverseMap();
        }
    }
}
