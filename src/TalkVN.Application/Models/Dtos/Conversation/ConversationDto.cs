using TalkVN.Application.Models.Dtos.Message;
using TalkVN.Application.Models.Dtos.User;

namespace TalkVN.Application.Models.Dtos.Conversation
{
    public class ConversationDto : BaseResponseDto
    {
        public List<UserDto> UserReceivers { get; set; }
        public MessageDto? LastMessage { get; set; }
        public List<string> UserReceiverIds { get; set; }
        public bool IsSeen { get; set; } = false;
    }
}
