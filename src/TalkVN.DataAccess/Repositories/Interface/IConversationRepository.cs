using TalkVN.DataAccess.Repositories.Interface;

namespace TalkVN.DataAccess.Repositories.Interface
{
    public interface IConversationRepository : IBaseRepository<TextChat>
    {
        Task<List<TextChat>> GetConversationByUserIdAsync(string userId);
        Task<TextChat?> IsConversationExisted(string userSenderId, string userReceiverId);
    }
}
