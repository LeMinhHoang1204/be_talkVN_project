namespace TalkVN.Application.Models.Dtos.Permission
{
    public class CheckPermissionRequestDto
    {
        public string Action { get; set; }
        public Guid? ConversationId { get; set; }
    }
}