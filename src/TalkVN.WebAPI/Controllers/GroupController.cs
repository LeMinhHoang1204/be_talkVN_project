using TalkVN.Application.Helpers;
using TalkVN.Application.Models;
using TalkVN.Application.Models.Dtos.Conversation;
using TalkVN.Application.Models.Dtos.Message;
using TalkVN.Application.Services.Interface;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using TalkVN.Application.Models.Dtos.Group;
using TalkVN.DataAccess.Data;
using TalkVN.DataAccess.Repositories.Interface;


namespace TalkVN.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class GroupController : ControllerBase
    {
        private readonly IGroupService _groupService;
        private readonly IGroupInvitationService _groupInvitationService;
        private readonly IClaimService _claimService;
        private readonly IUserRepository _userRepository;
        private readonly ILogger<GroupController> _logger;
        private readonly IPermissionService _permissionService;
        private readonly ApplicationDbContext _context;


        public GroupController(IGroupService groupService,
            IGroupInvitationService groupInvitationService,
            IClaimService claimService,
            ILogger<GroupController> logger,
            IPermissionService permissionService,
            ApplicationDbContext context
            )
        {
            _groupService = groupService;
            _groupInvitationService = groupInvitationService;
            _claimService = claimService;
            _logger = logger;
            _permissionService = permissionService;
            _context = context;
        }

        //get user's created groups
        [HttpGet]
        [Route("")]
        [ProducesResponseType(typeof(ApiResult<List<GroupDto>>), StatusCodes.Status200OK)] // OK với ProductResponse
        public async Task<IActionResult> GetAllGroupsAsync([FromQuery] PaginationFilter pagination)
        {
            return Ok(ApiResult<List<GroupDto>>.Success(await _groupService.GetAllGroupsAsync(pagination)));
        }

        //get group's members
        [HttpGet]
        [Route("{groupId}/members")]
        [ProducesResponseType(typeof(ApiResult<List<UserGroupRoleDto>>), StatusCodes.Status200OK)] // OK với ProductResponse
        public async Task<IActionResult> GetMembersByGroupIdAsync(Guid groupId)
        {
            return Ok(ApiResult<List<UserGroupRoleDto>>.Success(await _groupService.GetMembersByGroupIdAsync(groupId)));
        }

        // Create a new group
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

        //create invitation
        [HttpPost]
        [Route("create-invitation/{groupId}")]
        [ProducesResponseType(typeof(ApiResult<GroupInvitationDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> CreateGroupInvitationAsync(Guid groupId)
        {
            var userId = _claimService.GetUserId();
            bool canDeleteAnyMessage = await _permissionService.HasPermissionAsync(
                userId,
                TalkVN.Domain.Enums.Permissions.INVITE_TO_JOINED_GROUP.ToString(),
                groupId
            );

            if (!canDeleteAnyMessage)
            {
                return Unauthorized(new { message = "You do not have permission to invite members to this group." });
            }

            var result = await _groupInvitationService.CreateGroupInvitationAsync(groupId, userId);
            return Ok(ApiResult<GroupInvitationDto>.Success(result));
        }

        //get group info by invitation code -> called when user click on the link
        [HttpGet]
        [Route("invitation/{code}")]
        [ProducesResponseType(typeof(ApiResult<GroupDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetGroupInfoByInvitationCode(string code)
        {
            var result = await _groupService.GetGroupInfoByInvitationCodeAsync(code);
            return Ok(ApiResult<GroupDto>.Success(result));
        }

        [HttpPost]
        [Route("request-join-group")]
        public async Task<IActionResult> RequestJoinGroupAsync([FromBody] JoinGroupRequestDto dto)
        {
            var result = await _groupService.RequestJoinGroupAsync(dto);
            return Ok(ApiResult<JoinGroupRequestDto>.Success(result));
        }

        // Approve join group request
        [HttpPost]
        [Route("approve-join-request")]
        public async Task<IActionResult> ApproveJoinGroupRequestAsync([FromBody] RequestActionDto dto)
        {
            var userId = _claimService.GetUserId();
            var groupId = await _context.JoinGroupRequests
                .Where(r => r.Id == dto.JoinGroupRequestId)
                .Select(r => r.GroupId)
                .FirstOrDefaultAsync();

            if (groupId == default)
                return NotFound(new { message = "JoinGroupRequest not found." });

            bool canApproveRequest = await _permissionService.HasPermissionAsync(
                userId,
                TalkVN.Domain.Enums.Permissions.ACCEPT_REQUEST_TO_JOIN_GROUP.ToString(),
                groupId
            );

            if (!canApproveRequest)
            {
                return Unauthorized(new { message = "You do not have permission to approve join group requests." });
            }

            await _groupService.ApproveJoinGroupRequestAsync(dto);
            return Ok(ApiResult<string>.Success("Approved successfully"));
        }

        // Update User Role in Group
        [HttpPost]
        [Route("update-user-role")]
        public async Task<IActionResult> UpdateUserRoleInGroupAsync([FromBody] UpdateUserRoleInGroupDto dto)
        {
            var userId = _claimService.GetUserId();
            bool canUpdateRole = await _permissionService.HasPermissionAsync(
                userId,
                TalkVN.Domain.Enums.Permissions.UPDATE_USER_ROLE_IN_OWN_GROUP.ToString(),
                dto.GroupId
            );

            if (!canUpdateRole)
            {
                return Unauthorized(new { message = "You do not have permission to update user role in this group." });
            }

            await _groupService.UpdateUserRoleInGroupAsync(dto);
            return Ok(ApiResult<string>.Success("User role updated successfully"));
        }

        // Add OverridePermission
        [HttpPost]
        [Route("override-permission")]
        [ProducesResponseType(typeof(ApiResult<string>), StatusCodes.Status200OK)]
        public async Task<IActionResult> AddOverridePermissionAsync([FromBody] OverridePermissionDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = _claimService.GetUserId();

            var groupId = await _context.TextChats
                .Where(tc => tc.Id == dto.TextChatId) // Assuming dto.ChannelId is the TextChatId
                .Select(tc => tc.GroupId) // Assuming TextChats has a GroupId property
                .FirstOrDefaultAsync();

            if (groupId == null)
            {
                return NotFound(new { message = "Group not found." });
            }

            // Check if the user has permission to override permissions
            bool canOverridePermission = await _permissionService.HasPermissionAsync(
                userId,
                TalkVN.Domain.Enums.Permissions.OVERRIDE_PERMISSION_IN_GROUP.ToString(),
                groupId
            );

            if (!canOverridePermission)
            {
                return Unauthorized(new { message = "You do not have permission to override permissions in this group." });
            }

            // Add or update the override permission
            await _permissionService.OverridePermissionAsync(dto.UserId, dto.PermissionId, dto.TextChatId, dto.IsAllowed);

            return Ok(ApiResult<string>.Success("Override permission added/updated successfully"));
        }
    }
}
