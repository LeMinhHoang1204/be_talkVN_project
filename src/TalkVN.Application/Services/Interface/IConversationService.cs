using TalkVN.Application.Models;
using TalkVN.Application.Models.Dtos.Conversation;
using TalkVN.Application.Models.Dtos.Message;

namespace TalkVN.Application.Services.Interface
{
    public interface IConversationService
    {
        Task<List<ConversationDto>> GetAllConversationsAsync(PaginationFilter query);
        Task<ConversationDetailDto> GetConversationsByIdAsync(Guid TextChatId, int messagePageIndex, int messagePageSize);
        Task<List<ConversationDto>> GetConversationsByUserIdAsync(List<string> userIds, PaginationFilter query);
        // post
        Task<ConversationDto> CreateConversationAsync(List<string> userIds);
        //Task<ConversationDto> CreateGroupConversationAsync(CreateGroupConversationDto request);
        Task<MessageDto> SendMessageAsync(Guid TextChatId, RequestSendMessageDto request);
        // Put
        Task<MessageDto> UpdateMessageAsync(MessageDto message);
        Task<ConversationDto> UpdateConversationAsync(ConversationDto conversation);
        // Delete
        Task<ConversationDto> DeleteConversationAsync(Guid TextChatId);
        Task<MessageDto> DeleteMessageAsync(Guid messageId);
    }
}
