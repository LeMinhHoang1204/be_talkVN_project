using TalkVN.Infrastructure.SignalR.Helpers;
using TalkVN.Infrastructure.SignalR.Hubs.InterfaceClient;

using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace TalkVN.Infrastructure.SignalR.Hubs
{
    public class ConversationHub : Hub<IConversationClient>
    {
        private readonly ILogger<ConversationHub> _logger;
        public ConversationHub(ILogger<ConversationHub> logger)
        {
            _logger = logger;
        }
        public override Task OnConnectedAsync() => base.OnConnectedAsync();
        public async Task JoinConversationGroup(Guid TextChatId)
        {
            var roomId = HubRoom.ConversationHubJoinRoom(TextChatId);
            await Groups.AddToGroupAsync(Context.ConnectionId, roomId);
            await Clients.Group(roomId).ConversationJoined($"{Context.ConnectionId} has joined the room {roomId}");
        }
        public async Task SendOffer(Guid TextChatId, string sdp)
        {
            var roomId = HubRoom.ConversationHubJoinRoom(TextChatId);
            await Clients.OthersInGroup(roomId).ReceiveOffer(Context.ConnectionId, sdp);
        }

        public async Task SendAnswer(Guid TextChatId, string sdp)
        {
            var roomId = HubRoom.ConversationHubJoinRoom(TextChatId);
            await Clients.OthersInGroup(roomId).ReceiveAnswer(Context.ConnectionId, sdp);
        }

        public async Task SendIceCandidate(Guid TextChatId, string candidate)
        {
            var roomId = HubRoom.ConversationHubJoinRoom(TextChatId);
            await Clients.OthersInGroup(roomId).ReceiveIceCandidate(Context.ConnectionId, candidate);
        }
        public async Task StartCall(Guid TextChatId)
        {
            var roomId = HubRoom.ConversationHubJoinRoom(TextChatId);
            await Clients.OthersInGroup(roomId).ReceiveCall(TextChatId);
        }

    }
}
