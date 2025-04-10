using TalkVN.Application.Models.Dtos.Message;
using TalkVN.Application.SignalR.Interface;
using TalkVN.Infrastructure.SignalR.Helpers;
using TalkVN.Infrastructure.SignalR.Hubs;
using TalkVN.Infrastructure.SignalR.Hubs.InterfaceClient;

using Microsoft.AspNetCore.SignalR;

namespace TalkVN.Infrastructure.SignalR.Services
{
    public class ConversationNotificationService : IConversationNotificationService
    {
        private readonly IHubContext<ConversationHub, IConversationClient> _hubContext;
        public ConversationNotificationService(IHubContext<ConversationHub, IConversationClient> hubContext) => this._hubContext = hubContext;
        public async Task SendMessage(MessageDto message)
        {
            await _hubContext.Clients.Group(HubRoom.ConversationHubJoinRoom(message.TextChatId)).NewMessage(message);
        }

        public async Task UpdateMessage(MessageDto message)
        {
            await _hubContext.Clients.Group(HubRoom.ConversationHubJoinRoom(message.TextChatId)).UpdateMessage(message);
        }
        public async Task DeleteMessage(MessageDto message)
        {
            await _hubContext.Clients.Group(HubRoom.ConversationHubJoinRoom(message.TextChatId)).DeleteMessage(message);
        }
    }
}
