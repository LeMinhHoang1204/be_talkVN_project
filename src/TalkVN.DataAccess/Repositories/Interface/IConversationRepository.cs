using System.Linq.Expressions;

using TalkVN.DataAccess.Repositories.Interface;
using TalkVN.Domain.Identity;

namespace TalkVN.DataAccess.Repositories.Interface
{
    public interface IConversationRepository : IBaseRepository<TextChat>
    {
        Task<List<TextChat>> GetConversationByUserIdAsync(string userId);
        Task<TextChat?> IsConversationExisted(string userSenderId, string userReceiverId);

    }
}
