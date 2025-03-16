using TalkVN.Application.Models.Dtos.Message;
using TalkVN.Application.Models.Dtos.User;

namespace TalkVN.Application.Models.Dtos.Conversation
{
    public class ConversationDetailDto : BaseResponseDto
    {
        public List<UserDto> UserReceivers { get; set; }
        public List<string> UserReceiverIds { get; set; }
        public List<MessageDto> Messages { get; set; }

    }
}
