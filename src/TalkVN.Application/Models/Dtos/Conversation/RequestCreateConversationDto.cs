namespace TalkVN.Application.Models.Dtos.Conversation;

public class RequestCreateConversationDto
{
    public List<string>? UserIds { get; set; }
    public string? Username { get; set; }
}
