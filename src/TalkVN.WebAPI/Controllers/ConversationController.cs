using TalkVN.Application.Helpers;
using TalkVN.Application.Models;
using TalkVN.Application.Models.Dtos.Conversation;
using TalkVN.Application.Models.Dtos.Message;
using TalkVN.Application.Services.Interface;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
        public ConversationController(ILogger<ConversationController> logger, IConversationService conversationService, IClaimService claimService)
        {
            _logger = logger;
            _conversationService = conversationService;
            _claimService = claimService;
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
        [HttpPost]
        [Route("")]
        [ProducesResponseType(typeof(ApiResult<ConversationDto>), StatusCodes.Status200OK)] // OK với ProductResponse
        public async Task<IActionResult> CreateConversationAsync([FromBody] List<string> userIds)
        {
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
