namespace TalkVN.Domain.Entities.SystemEntities.Permissions;

public class TextChatPermission : OverridePermission
{
    public Guid TextChatId { get; set; }

    public Conversation TextChat { get; set; }

    public TextChatPermission()
    {
        Type = PermissionType.TextChat.ToString();
    }
}
