namespace TalkVN.Application.Models.Dtos.Conversation;

public class SearchConversationResponseDto
{
    public List<ConversationDto> Conversations { get; set; }
    public List<string> UserIds { get; set; }
}
