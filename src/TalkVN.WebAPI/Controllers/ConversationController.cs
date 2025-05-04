using System.Text.Json;

using TalkVN.Application.Helpers;
using TalkVN.Application.Models;
using TalkVN.Application.Models.Dtos.Conversation;
using TalkVN.Application.Models.Dtos.Message;
using TalkVN.Application.Services.Interface;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using TalkVN.DataAccess.Repositories.Interface;

namespace TalkVN.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ConversationController : ControllerBase
    {
        private readonly ILogger<ConversationController> _logger;
        private readonly IConversationService _conversationService;
        private readonly IClaimService _claimService;
        private readonly IUserRepository _userRepository;
        public ConversationController(ILogger<ConversationController> logger, IConversationService conversationService, IClaimService claimService, IUserRepository userRepository)
        {
            _logger = logger;
            _conversationService = conversationService;
            _claimService = claimService;
            _userRepository = userRepository;
        }

        [HttpGet]
        [Route("")]
        [ProducesResponseType(typeof(ApiResult<List<ConversationDto>>), StatusCodes.Status200OK)] // OK với ProductResponse
        public async Task<IActionResult> GetAllConversationsAsync([FromQuery] PaginationFilter pagination)
        {
            return Ok(ApiResult<List<ConversationDto>>.Success(await _conversationService.GetAllConversationsAsync(pagination)));
        }

        [HttpGet]
        [Route("{conversationId}")]
        [ProducesResponseType(typeof(ApiResult<ConversationDetailDto>), StatusCodes.Status200OK)] // OK với ProductResponse
        public async Task<IActionResult> GetConversationsByIdAsync(Guid conversationId, [FromQuery] int messagePageIndex = 0, [FromQuery] int messagePageSize = 100)
        {
            return Ok(ApiResult<ConversationDetailDto>.Success(await _conversationService.GetConversationsByIdAsync(conversationId, messagePageIndex, messagePageSize)));
        }
        // create conversation
        [HttpPost]
        [Route("")]
        [ProducesResponseType(typeof(ApiResult<ConversationDto>), StatusCodes.Status200OK)] // OK với ProductResponse
        public async Task<IActionResult> CreateConversationAsync([FromBody] List<string> userIds)
        {
            return Ok(ApiResult<ConversationDto>.Success(await _conversationService.CreateConversationAsync(userIds)));
        }
        // search a conversation by username
        [HttpGet]
        [Route("search")]
        [ProducesResponseType(typeof(ApiResult<ConversationDto>), StatusCodes.Status200OK)] // OK với ProductResponse
        public async Task<IActionResult> SearchConversationAsync([FromQuery] List<string> Usernames, [FromQuery] PaginationFilter pagination)
        {
            // Nếu truyền username → tìm userId
            List<string> userIds = new();
            var creatorUserId = _claimService.GetUserId();
            foreach (var username in Usernames)
            {
                if (string.IsNullOrWhiteSpace(username))
                    continue;

                var user = await _userRepository.GetFirstOrDefaultAsync(x => x.UserName == username);
                if (user == null)
                {
                    return NotFound(ApiResult<string>.Failure(new[]
                    {
                        new ApiResultError(ApiResultErrorCodes.NotFound, $"User '{username}' not found")
                    }));
                }

                if (creatorUserId == user.Id)
                {
                    return BadRequest(ApiResult<string>.Failure(new[]
                    {
                        new ApiResultError(ApiResultErrorCodes.NotFound, "You cannot search yourself")
                    }));
                }

                userIds.Add(user.Id);
            }

            if (!userIds.Any())
            {
                return BadRequest(ApiResult<string>.Failure(new[]
                {
                    new ApiResultError(ApiResultErrorCodes.ModelValidation, "No valid usernames provided")
                }));
            }

            var conversations = await _conversationService.GetConversationsByUserIdAsync(userIds, pagination);

            var response = new SearchConversationResponseDto
            {
                Conversations = conversations,
                UserIds = userIds
            };

            return Ok(ApiResult<SearchConversationResponseDto>.Success(response));
        }

        [HttpPost]
        [Route("{conversationId}")]
        [ProducesResponseType(typeof(ApiResult<MessageDto>), StatusCodes.Status200OK)] // OK với ProductResponse
        public async Task<IActionResult> SendMessageAsync(Guid conversationId, [FromBody] RequestSendMessageDto request)
        {
            return Ok(ApiResult<MessageDto>.Success(await _conversationService.SendMessageAsync(conversationId, request)));
        }
        [HttpPut]
        [Route("{conversationId}")]
        [ProducesResponseType(typeof(ApiResult<ConversationDto>), StatusCodes.Status200OK)] // OK với ProductResponse
        public async Task<IActionResult> UpdateConversationAsync(Guid conversationId, [FromBody] ConversationDto request)
        {
            return Ok(ApiResult<ConversationDto>.Success(await _conversationService.UpdateConversationAsync(request)));
        }
        [HttpPut]
        [Route("{conversationId}/messages/{messageId}")]
        [ProducesResponseType(typeof(ApiResult<MessageDto>), StatusCodes.Status200OK)] // OK với ProductResponse
        public async Task<IActionResult> UpdateMessageAsync(Guid conversationId, Guid messageId, [FromBody] MessageDto messageDto)
        {
            return Ok(ApiResult<MessageDto>.Success(await _conversationService.UpdateMessageAsync(messageDto)));
        }
        [HttpDelete]
        [Route("{conversationId}")]
        [ProducesResponseType(typeof(ApiResult<ConversationDto>), StatusCodes.Status200OK)] // OK với ProductResponse
        public async Task<IActionResult> DeleteConversationAsync(Guid conversationId)
        {
            return Ok(ApiResult<ConversationDto>.Success(await _conversationService.DeleteConversationAsync(conversationId)));
        }
        [HttpDelete]
        [Route("{conversationId}/messages/{messageId}")]
        [ProducesResponseType(typeof(ApiResult<MessageDto>), StatusCodes.Status200OK)] // OK với ProductResponse
        public async Task<IActionResult> DeleteConversationAsync(Guid conversationId, Guid messageId)
        {
            return Ok(ApiResult<MessageDto>.Success(await _conversationService.DeleteMessageAsync(messageId)));
        }
    }
}
