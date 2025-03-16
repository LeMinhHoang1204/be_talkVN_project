using TalkVN.Application.Models.Dtos.Conversation;
using TalkVN.Application.Models.Dtos.Notification;

namespace TalkVN.Application.SignalR.Interface
{
    public interface IUserNotificationService
    {
        Task UpdateConversation(ConversationDto conversation, string userSenderId);
        Task AddConversation(ConversationDto conversation, string userSenderId);
        Task DeleteConversation(ConversationDto conversation, string userSenderId);
        Task NewNotification(NotificationDto notificationDto);
        Task UpdateNotification(NotificationDto notificationDto);

    }
}
