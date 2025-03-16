using AutoMapper;

using TalkVN.Application.Models.Dtos.Conversation;

namespace TalkVN.Application.Mapping
{
    public class ConversationProfile : Profile
    {
        public ConversationProfile()
        {
            CreateMap<Conversation, ConversationDto>();
            CreateMap<Conversation, ConversationDetailDto>();
        }
    }
}
