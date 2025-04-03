using TalkVN.DataAccess.Data;
using TalkVN.DataAccess.Repositories.Interface;
using TalkVN.Domain.Enums;

namespace TalkVN.DataAccess.Repositories
{
    public class ConversationRepository : BaseRepository<TextChat>, IConversationRepository
    {
        public ConversationRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<List<TextChat>> GetConversationByUserIdAsync(string userId)
        {
            var listConversation = await (from convDetail in Context.TextChatParticipants
                                          join conv in Context.TextChats on convDetail.TextChatId equals conv.Id
                                          where convDetail.UserId == userId && conv.IsDeleted == false
                                          select conv)
                                          .Include(
                                            c => c.TextChatParticipants
                                            )
                                          .Include(c => c.LastMessage)
                                          .AsNoTracking()
                                          .ToListAsync();
            return listConversation;
        }

        public async Task<TextChat?> IsConversationExisted(string userSenderId, string userReceiverId)
        {
            TextChat? hasConversation = await Context.TextChatParticipants
                .Where(cd2 => cd2.UserId == userReceiverId && cd2.TextChat.TextChatType == TextChatType.Person.ToString())
                .SelectMany(cd2 => Context.TextChatParticipants
                .Where(cd1 => cd1.UserId == userSenderId && cd1.TextChatId == cd2.TextChatId))
                .Select(cd1 => cd1.TextChat)
                .SingleOrDefaultAsync();
            if (hasConversation != null)
                hasConversation.LastMessage = await Context.Messages.SingleOrDefaultAsync(p => p.Id == hasConversation.LastMessageId);
            return hasConversation;
        }
    }
}
