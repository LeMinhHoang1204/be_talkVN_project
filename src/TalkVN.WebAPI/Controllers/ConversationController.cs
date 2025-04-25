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
        public async Task<IActionResult> CreateConversationAsync([FromBody] RequestCreateConversationDto request)
        {
            // Nếu truyền username → tìm userId
            List<string> userIds = request.UserIds ?? new();

            // Lấy userId của người tạo từ claim
            var creatorUserId = _claimService.GetUserId();
            if (!string.IsNullOrEmpty(creatorUserId))
            {
                userIds.Add(creatorUserId);
            }

            if (!string.IsNullOrWhiteSpace(request.Username))
            {
                var user = await _userRepository.GetFirstOrDefaultAsync(x => x.UserName == request.Username);
                if (user == null)
                {
                    return NotFound(ApiResult<string>.Failure(new[] { new ApiResultError(ApiResultErrorCodes.NotFound, "User Not Found") }));
                }
                userIds.Add(user.Id);
            }

            return Ok(ApiResult<ConversationDto>.Success(await _conversationService.CreateConversationAsync(userIds)));
        }
        // search a conversation by username
        [HttpPost]
        [Route("search")]
        [ProducesResponseType(typeof(ApiResult<ConversationDto>), StatusCodes.Status200OK)] // OK với ProductResponse
        public async Task<IActionResult> SearchonversationAsync([FromQuery] string Username)
        {
            // Nếu truyền username → tìm userId
            List<string> userIds = new();

            // Lấy userId của người tạo từ claim
            var creatorUserId = _claimService.GetUserId();
            if (!string.IsNullOrEmpty(creatorUserId))
            {
                Console.WriteLine("creatorID: " + creatorUserId); // checked
                userIds.Add(creatorUserId);
            }

            if (!string.IsNullOrWhiteSpace(Username))
            {
                Console.WriteLine("Username123: " + Username); // checked
                var user = await _userRepository.GetFirstOrDefaultAsync(x => x.UserName == Username); // sai o day
                if (user == null)
                {
                    return NotFound(ApiResult<string>.Failure(new[] { new ApiResultError(ApiResultErrorCodes.NotFound, "User Not Found") }));
                }
                userIds.Add(user.Id);
            }

            return Ok(ApiResult<ConversationDto>.Success(await _conversationService.CreateConversationAsync(userIds)));
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
