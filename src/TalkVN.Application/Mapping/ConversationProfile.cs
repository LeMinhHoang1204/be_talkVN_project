using AutoMapper;

using TalkVN.Application.Models.Dtos.Conversation;

namespace TalkVN.Application.Mapping
{
    public class ConversationProfile : Profile
    {
        public ConversationProfile()
        {
            CreateMap<TextChat, ConversationDto>();
            CreateMap<TextChat, ConversationDetailDto>();
        }
    }
}
