using AutoMapper;

using TalkVN.Application.Exceptions;
using TalkVN.Application.Helpers;
using TalkVN.Application.Localization;
using TalkVN.Application.Models;
using TalkVN.Application.Models.Dtos.Conversation;
using TalkVN.Application.Models.Dtos.Message;
using TalkVN.Application.Models.Dtos.User;
using TalkVN.Application.Services.Interface;
using TalkVN.Application.SignalR.Interface;
using TalkVN.DataAccess.Repositories.Interface;
using TalkVN.DataAccess.Repositories.Interface;
using TalkVN.Domain.Entities.ChatEntities;
using TalkVN.Domain.Enums;

using Microsoft.EntityFrameworkCore;

namespace TalkVN.Application.Services
{
    public class ConversationService : IConversationService
    {
        private readonly IConversationRepository _conversationRepository;
        private readonly IBaseRepository<TextChatParticipant> _conversationDetailRepository;
        private readonly IBaseRepository<Message> _messageRepository;
        private readonly IMapper _mapper;
        private readonly IClaimService _claimService;
        private readonly IUserRepository _userRepository;
        private readonly IUserNotificationService _userNotificationService;
        private readonly IConversationNotificationService _conversationNotificationService;
        public ConversationService(IConversationRepository conversationRepository
            , IUserRepository userRepository
            , IRepositoryFactory repositoryFactory
            , IMapper mapper
            , IClaimService claimService
            , IConversationNotificationService conversationNotificationService
            , IUserNotificationService userNotificationService
            )
        {
            _userRepository = userRepository;
            _conversationRepository = conversationRepository;
            _conversationDetailRepository = repositoryFactory.GetRepository<TextChatParticipant>();
            _messageRepository = repositoryFactory.GetRepository<Message>();
            _mapper = mapper;
            _claimService = claimService;
            this._conversationNotificationService = conversationNotificationService;
            _userNotificationService = userNotificationService;
        }

        public async Task<List<ConversationDto>> GetAllConversationsAsync(PaginationFilter paginationFilter)
        {
            var userId = _claimService.GetUserId();
            var paginationResponse = await _conversationRepository
                                    .GetAllAsync(p => p.TextChatParticipants.Any(p => p.UserId == userId)
                                    , p => p.OrderByDescending(c => c.UpdatedOn), paginationFilter.PageIndex, paginationFilter.PageSize
                                    , query => query.Include(c => c.LastMessage)
                                                    .Include(c => c.TextChatParticipants));
            List<ConversationDto> response = new();
            foreach (TextChat c in paginationResponse.Items)
            {
                var receiverIds = c.TextChatParticipants.Where(u => u.UserId != userId).Select(p => p.UserId);
                var receiverUsers = await _userRepository.GetAllAsync(p => receiverIds.Contains(p.Id));
                ConversationDto conversationDto = new();
                conversationDto.LastMessage = _mapper.Map<MessageDto>(c.LastMessage);
                conversationDto.IsSeen = c.IsSeen;
                conversationDto.UserReceivers = _mapper.Map<List<UserDto>>(receiverUsers);
                conversationDto.Id = c.Id;
                conversationDto.UserReceiverIds = receiverIds.ToList();
                response.Add(conversationDto);
            }
            return response;
        }
        public async Task<ConversationDetailDto> GetConversationsByIdAsync(Guid conversationId, int messagePageIndex, int messagePageSize)
        {
            var userId = _claimService.GetUserId();
            var conversation = await _conversationRepository
                .GetFirstOrDefaultAsync(p => p.Id == conversationId, query => query
                .Include(c => c.TextChatParticipants));
            var receiverIds = conversation.TextChatParticipants.Where(u => u.UserId != userId).Select(p => p.UserId);
            var receiverUsers = await _userRepository.GetAllAsync(p => receiverIds.Contains(p.Id));
            ConversationDetailDto response = new();
            response.UserReceivers = _mapper.Map<List<UserDto>>(receiverUsers);
            response.Id = conversation.Id;
            response.UserReceiverIds = receiverIds.ToList();
            var responsePagination = await _messageRepository
                .GetAllAsync(p => p.IsDeleted == false && p.ConversationId == conversationId
                            , p => p.OrderBy(p => p.CreatedOn)
                            , messagePageIndex, messagePageSize
                            );
            response.Messages = _mapper.Map<List<MessageDto>>(responsePagination.Items);
            return response;

        }

        public async Task<ConversationDto> CreateConversationAsync(List<string> userIds)
        {
            var senderId = _claimService.GetUserId();
            if (userIds.Count < 2)
            {
                throw new InvalidModelException(ValidationTexts.NotValidate.Format(userIds.GetType(), userIds));
            }
            if (userIds.Count == 2)
            {
                var conversationExisted = await _conversationRepository.IsConversationExisted(userIds[0], userIds[1]);
                if (conversationExisted != null)
                {
                    var conversationExistedDto = _mapper.Map<ConversationDto>(conversationExisted);
                    var otherUser = userIds[0] == senderId ? userIds[1] : userIds[0];
                    conversationExistedDto.UserReceivers = _mapper.Map<List<UserDto>>(await _userRepository.GetAllAsync(p => p.Id == otherUser));
                    return conversationExistedDto;
                }
            }
            TextChat textChat = new TextChat()
            {
                IsDeleted = false,
                LastMessageId = null,
                NumOfUser = userIds.Count,
                ConversationType = userIds.Count == 2 ? TextChatType.Person.ToString() : TextChatType.Group.ToString(),
            };
            await _conversationRepository.AddAsync(textChat);
            List<TextChatParticipant> TextChatParticipants = new();
            foreach (var user in userIds)
            {
                TextChatParticipants.Add(new TextChatParticipant()
                {
                    ConversationId = textChat.Id,
                    UserId = user,
                });
            }
            await _conversationDetailRepository.AddRangeAsync(TextChatParticipants);
            var conversationDto = _mapper.Map<ConversationDto>(textChat);
            conversationDto.LastMessage = null;
            conversationDto.UserReceiverIds = userIds;
            conversationDto.UserReceivers = _mapper.Map<List<UserDto>>(await _userRepository.GetAllAsync(p => userIds.Contains(p.Id)));
            await _userNotificationService.AddConversation(conversationDto, senderId);
            return conversationDto;
        }
        public async Task<MessageDto> SendMessageAsync(Guid conversationId, RequestSendMessageDto request)
        {
            var senderId = _claimService.GetUserId();
            Message message = _mapper.Map<Message>(request);
            message.SenderId = senderId;
            message.ConversationId = conversationId;
            message.Status = MessageStatus.NORMAL;
            var conversation = await _conversationRepository.GetFirstOrDefaultAsync(p => p.Id == conversationId, p => p.Include(p => p.TextChatParticipants));
            if (conversation == null)
            {
                throw new NotFoundException(ValidationTexts.NotFound.Format(typeof(TextChat), conversationId));
            }
            await _messageRepository.AddAsync(message);
            conversation.LastMessageId = message.Id;
            await _conversationRepository.UpdateAsync(conversation);
            ConversationDto conversationDto = _mapper.Map<ConversationDto>(conversation);
            conversationDto.UserReceiverIds = conversation.TextChatParticipants.Where(p => p.UserId != senderId).Select(p => p.UserId).ToList();
            await _conversationNotificationService.SendMessage(_mapper.Map<MessageDto>(message));
            await _userNotificationService.UpdateConversation(conversationDto, senderId);
            return _mapper.Map<MessageDto>(message);
        }
        public async Task<MessageDto> UpdateMessageAsync(MessageDto messageDto)
        {
            var senderId = _claimService.GetUserId();
            Message message = await _messageRepository.GetFirstOrDefaultAsync(p => p.Id == messageDto.Id);
            var conversation = await _conversationRepository.GetFirstOrDefaultAsync(p => p.Id == messageDto.ConversationId, p => p.Include(p => p.TextChatParticipants));
            message.MessageText = messageDto.MessageText;
            message.Status = MessageStatus.EDITED;
            ConversationDto conversationDto = _mapper.Map<ConversationDto>(conversation);
            conversationDto.UserReceiverIds = conversation.TextChatParticipants.Where(p => p.UserId != senderId).Select(p => p.UserId).ToList();
            await _messageRepository.UpdateAsync(message);
            await _conversationNotificationService.UpdateMessage(_mapper.Map<MessageDto>(message));
            await _userNotificationService.UpdateConversation(conversationDto, senderId);
            return _mapper.Map<MessageDto>(message);
        }
        public async Task<ConversationDto> UpdateConversationAsync(ConversationDto conversationDto)
        {
            var senderId = _claimService.GetUserId();
            TextChat textChat = await _conversationRepository.GetFirstOrDefaultAsync(p => p.Id == conversationDto.Id);
            if (textChat == null)
            {
                throw new NotFoundException(ValidationTexts.NotFound.Format("TextChat", conversationDto.Id));
            }
            // Cập nhật từng thuộc tính từ DTO
            _mapper.Map(conversationDto, textChat);
            await _conversationRepository.UpdateAsync(textChat);
            await _userNotificationService.UpdateConversation(conversationDto, senderId);
            return _mapper.Map<ConversationDto>(textChat);
        }
        public async Task<ConversationDto> DeleteConversationAsync(Guid conversationId)
        {
            var senderId = _claimService.GetUserId();
            TextChat textChat = await _conversationRepository.GetFirstOrDefaultAsync(p => p.Id == conversationId, p => p.Include(p => p.TextChatParticipants));
            textChat.IsDeleted = true;
            ConversationDto conversationDto = _mapper.Map<ConversationDto>(textChat);
            conversationDto.UserReceiverIds = textChat.TextChatParticipants.Where(p => p.UserId != senderId).Select(p => p.UserId).ToList();
            await _conversationRepository.UpdateAsync(textChat);
            await _userNotificationService.DeleteConversation(conversationDto, senderId);
            return _mapper.Map<ConversationDto>(textChat);
        }
        public async Task<MessageDto> DeleteMessageAsync(Guid messageId)
        {
            var senderId = _claimService.GetUserId();
            Message message = await _messageRepository.GetFirstOrDefaultAsync(p => p.Id == messageId);
            message.Status = MessageStatus.UNSENT;
            await _messageRepository.UpdateAsync(message);
            TextChat textChat = await _conversationRepository.GetFirstOrDefaultAsync(p => p.Id == message.Id, p => p.Include(p => p.TextChatParticipants));
            ConversationDto conversationDto = _mapper.Map<ConversationDto>(textChat);
            conversationDto.UserReceiverIds = textChat.TextChatParticipants.Where(p => p.UserId != senderId).Select(p => p.UserId).ToList();
            await _conversationNotificationService.DeleteMessage(_mapper.Map<MessageDto>(message));
            await _userNotificationService.UpdateConversation(conversationDto, senderId);
            return _mapper.Map<MessageDto>(message);
        }

    }
}
