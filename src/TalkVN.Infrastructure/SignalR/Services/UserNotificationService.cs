using TalkVN.Application.Models.Dtos.Conversation;
using TalkVN.Application.Models.Dtos.Notification;
using TalkVN.Application.SignalR.Interface;
using TalkVN.Infrastructure.SignalR.Helpers;
using TalkVN.Infrastructure.SignalR.Hubs;
using TalkVN.Infrastructure.SignalR.Hubs.InterfaceClient;

using Microsoft.AspNetCore.SignalR;

namespace TalkVN.Infrastructure.SignalR.Services
{
    public class UserNotificationService : IUserNotificationService
    {
        private readonly IHubContext<UserHub, IUserClient> _hubContext;
        public UserNotificationService(IHubContext<UserHub, IUserClient> hubContext) => this._hubContext = hubContext;

        public async Task AddConversation(ConversationDto conversation, string userSenderId)
        {
            await _hubContext.Clients.Group(HubRoom.UserHubJoinRoom(userSenderId)).AddConversation(conversation);
            foreach (var uid in conversation.UserReceiverIds)
            {
                await _hubContext.Clients.Group(HubRoom.UserHubJoinRoom(uid)).AddConversation(conversation);
            }
        }
        public async Task UpdateConversation(ConversationDto conversation, string userSenderId)
        {
            await _hubContext.Clients.Group(HubRoom.UserHubJoinRoom(userSenderId)).UpdateConversation(conversation);
            foreach (var uid in conversation.UserReceiverIds)
            {
                await _hubContext.Clients.Group(HubRoom.UserHubJoinRoom(uid)).UpdateConversation(conversation);
            }
        }
        public async Task DeleteConversation(ConversationDto conversation, string userSenderId)
        {
            await _hubContext.Clients.Group(HubRoom.UserHubJoinRoom(userSenderId)).DeleteConversation(conversation);
            foreach (var uid in conversation.UserReceiverIds)
            {
                await _hubContext.Clients.Group(HubRoom.UserHubJoinRoom(uid)).DeleteConversation(conversation);
            }
        }
        public async Task NewNotification(NotificationDto notificationDto)
        {
            await _hubContext.Clients.Group(HubRoom.UserHubJoinRoom(notificationDto.ReceiverUserId)).AddNotification(notificationDto);

        }
        public async Task UpdateNotification(NotificationDto notificationDto)
        {
            await _hubContext.Clients.Group(HubRoom.UserHubJoinRoom(notificationDto.ReceiverUserId)).UpdateNotification(notificationDto);
        }
    }
}
