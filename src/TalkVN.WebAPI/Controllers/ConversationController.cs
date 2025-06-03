using System.Text.Json;

using TalkVN.Application.Helpers;
using TalkVN.Application.Models;
using TalkVN.Application.Models.Dtos.Conversation;
using TalkVN.Application.Models.Dtos.Message;
using TalkVN.Application.Services.Interface;
using TalkVN.Application.Models.Dtos.Permission;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using TalkVN.DataAccess.Data;
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
        private readonly IPermissionService _permissionService;
        private readonly ApplicationDbContext _context;
        public ConversationController(ILogger<ConversationController> logger, IConversationService conversationService, IClaimService claimService, IUserRepository userRepository, IPermissionService permissionService, ApplicationDbContext context)
        {
            _logger = logger;
            _conversationService = conversationService;
            _claimService = claimService;
            _userRepository = userRepository;
            _permissionService = permissionService;
            _context = context;
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
        public async Task<IActionResult> GetConversationsByIdAsync(Guid conversationId, [FromQuery] int messagePageIndex = 1, [FromQuery] int messagePageSize = 100)
        {
            return Ok(ApiResult<ConversationDetailDto>.Success(await _conversationService.GetConversationsByIdAsync(conversationId, messagePageIndex, messagePageSize)));
        }
        // create conversation
        [HttpPost]
        [Route("")]
        [ProducesResponseType(typeof(ApiResult<ConversationDto>), StatusCodes.Status200OK)] // OK với ProductResponse
        public async Task<IActionResult> CreateConversationAsync([FromBody] CreateConversationRequestDTO request)
        {
            return Ok(ApiResult<ConversationDto>.Success(await _conversationService.CreateConversationAsync(request.UserIds)));
        }

        // search a conversation by username
        [HttpPost]
        [Route("search")]
        [ProducesResponseType(typeof(ApiResult<ConversationDto>), StatusCodes.Status200OK)] // OK với ProductResponse
        public async Task<IActionResult> SearchConversationAsync([FromBody] SearchConversationRequest request, [FromQuery] PaginationFilter pagination)
        {
            // Nếu truyền username → tìm userId
            List<string> userIds = new();
            var creatorUserId = _claimService.GetUserId();
            foreach (var username in request.Usernames)
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
            return Ok(ApiResult<SearchConversationResponseDto>.Success(await _conversationService.GetConversationsByUserIdAsync(userIds, pagination)));
        }

        // send message
        [HttpPost]
        [Route("{conversationId}")]
        [ProducesResponseType(typeof(ApiResult<MessageDto>), StatusCodes.Status200OK)] // OK với ProductResponse
        public async Task<IActionResult> SendMessageAsync(Guid conversationId, [FromBody] RequestSendMessageDto request)
        {

            var userId = _claimService.GetUserId();

            var groupId = await _context.TextChats
                .Where(r => r.Id == conversationId)
                .Select(r => r.GroupId)
                .FirstOrDefaultAsync();

            if (groupId != null)
            {
                bool canSendMessageInGroup = await _permissionService.HasPermissionAsync(userId, TalkVN.Domain.Enums.Permissions.SEND_MESSAGES_IN_TEXT_CHANNEL.ToString(), groupId);
                if (!canSendMessageInGroup)
                {
                    return Unauthorized(new { message = "You do not have permission to send messages in this group." });
                }

                bool canSendMessageInSpecificChannel = await _permissionService.HasPermissionAsync(userId, TalkVN.Domain.Enums.Permissions.SEND_MESSAGES_IN_SPECIFIC_TEXT_CHANNEL.ToString(), conversationId);
                if (!canSendMessageInSpecificChannel)
                {
                    return Unauthorized(new { message = "You do not have permission to send messages in this specific conversation." });
                }
            }
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

        [HttpPost("check")]
        [AllowAnonymous] // or [Authorize], depending on your needs
        public async Task<IActionResult> CheckPermission([FromBody] CheckPermissionRequestDto dto)
        {
            var userId = _claimService.GetUserId();
            if (string.IsNullOrEmpty(dto.Action))
            {
                return BadRequest(new { allowed = false, reason = "Action is required" });
            }

            // Try to parse the action to your Permissions enum
            if (!Enum.TryParse<TalkVN.Domain.Enums.Permissions>(dto.Action, out var permissionEnum))
            {
                return BadRequest(new { allowed = false, reason = "Invalid permission action" });
            }

            bool allowed = false;
            string reason = null;

            if (dto.ConversationId.HasValue)
            {
                // Get groupId from conversationId (TextChat)
                var groupId = await _context.TextChats
                    .Where(tc => tc.Id == dto.ConversationId.Value)
                    .Select(tc => tc.GroupId)
                    .FirstOrDefaultAsync();

                if (groupId == null)
                {
                    return Ok(new { allowed = false, reason = "Conversation not found" });
                }

                allowed = await _permissionService.HasPermissionAsync(userId, dto.Action, groupId, dto.ConversationId.Value);
                if (!allowed)
                    reason = "You do not have this permission in this conversation";
            }
            else
            {
                // Check global/group permission (no conversation context)
                allowed = await _permissionService.HasPermissionAsync(userId, dto.Action);
                if (!allowed)
                    reason = "You do not have this permission";
            }

            return Ok(new { allowed, reason });
        }
    }
}
