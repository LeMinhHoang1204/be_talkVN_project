using AutoMapper;

using TalkVN.Application.Models.Dtos.Message;

namespace TalkVN.Application.Mapping
{
    public class MessageProfile : Profile
    {
        public MessageProfile()
        {
            CreateMap<RequestSendMessageDto, Message>();
            CreateMap<Message, MessageDto>();
        }
    }
}
