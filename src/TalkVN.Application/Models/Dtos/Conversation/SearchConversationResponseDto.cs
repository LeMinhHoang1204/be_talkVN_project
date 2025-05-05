using TalkVN.Application.Models.Dtos.User;

namespace TalkVN.Application.Models.Dtos.Conversation;

public class SearchConversationResponseDto
{
    public List<ConversationDto> Conversations { get; set; }
    public List<UserDto> SearchedUsers { get; set; }
}
