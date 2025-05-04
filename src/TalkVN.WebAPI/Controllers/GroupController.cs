using TalkVN.Application.Helpers;
using TalkVN.Application.Models;
using TalkVN.Application.Models.Dtos.Conversation;
using TalkVN.Application.Models.Dtos.Message;
using TalkVN.Application.Services.Interface;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using TalkVN.Application.Models.Dtos.Group;
using TalkVN.DataAccess.Repositories.Interface;


namespace TalkVN.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class GroupController : ControllerBase
    {
        private readonly IGroupService _groupService;
        private readonly IClaimService _claimService;
        private readonly IUserRepository _userRepository;
        private readonly ILogger<GroupController> _logger;

        public GroupController(IGroupService groupService,
            IClaimService claimService,
            ILogger<GroupController> logger)
        {
            _groupService = groupService;
            _claimService = claimService;
            _logger = logger;
        }

        //get user's created groups
        [HttpGet]
        [Route("")]
        [ProducesResponseType(typeof(ApiResult<List<ConversationDto>>), StatusCodes.Status200OK)] // OK với ProductResponse
        public async Task<IActionResult> GetAllGroupsAsync([FromQuery] PaginationFilter pagination)
        {
            return Ok(ApiResult<List<GroupDto>>.Success(await _groupService.GetAllGroupsAsync(pagination)));
        }

        [HttpPost]
        [Route("")]
        [ProducesResponseType(typeof(ApiResult<GroupDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> CreateGroupAsync([FromBody]RequestCreateGroupDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await _groupService.CreateGroupAsync(request);
                return Ok(ApiResult<GroupDto>.Success(result));
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
