using TalkVN.DataAccess.Repositories.Interface;

namespace TalkVN.DataAccess.Repositories.Interface
{
    public interface IConversationRepository : IBaseRepository<Conversation>
    {
        Task<List<Conversation>> GetConversationByUserIdAsync(string userId);
        Task<Conversation?> IsConversationExisted(string userSenderId, string userReceiverId);
    }
}
