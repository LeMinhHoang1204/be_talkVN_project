using TalkVN.Application.Models.Dtos.Message;

namespace TalkVN.Application.SignalR.Interface
{
    public interface IConversationNotificationService
    {
        Task SendMessage(MessageDto message);
        Task UpdateMessage(MessageDto message);
        Task DeleteMessage(MessageDto message);
    }
}
