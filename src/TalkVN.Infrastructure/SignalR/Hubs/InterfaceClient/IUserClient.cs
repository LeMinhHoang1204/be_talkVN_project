using TalkVN.Application.Models.Dtos.Conversation;
using TalkVN.Application.Models.Dtos.Notification;

namespace TalkVN.Infrastructure.SignalR.Hubs.InterfaceClient
{
    public interface IUserClient
    {
        Task UpdateConversation(ConversationDto conversation);
        Task AddConversation(ConversationDto conversation);
        Task DeleteConversation(ConversationDto conversation);
        // Other events
        Task NotifyNewFollower(string followerId, string followedUserId);
        Task AddNotification(NotificationDto notification);
        Task UpdateNotification(NotificationDto notification);




    }
}
