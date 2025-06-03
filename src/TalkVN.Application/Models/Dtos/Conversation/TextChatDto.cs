using TalkVN.Application.Models.Dtos.Message;
using TalkVN.Application.Models.Dtos.User;

namespace TalkVN.Application.Models.Dtos.Conversation;

public class TextChatDto
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string TextChatType { get; set; }
    public Guid? GroupId { get; set; }
    public MessageDto? LastMessage { get; set; }
    public string CreatedBy { get; set; }
    public List<UserDto>? UserReceivers { get; set; }
}
