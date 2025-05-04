namespace TalkVN.Infrastructure.SignalR.Helpers
{
    public class HubRoom
    {
        public static string ConversationHubJoinRoom(Guid TextChatId)
        {
            return $"{HubEnum.Conversation.ToString()}: ${TextChatId}";
        }
        public static string UserHubJoinRoom(string userId)
        {
            return $"{HubEnum.User.ToString()}: ${userId}";
        }
    }
}
