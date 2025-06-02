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
using TalkVN.Application.Models.Dtos.User;
using TalkVN.DataAccess.Repositories.Interface;
using TalkVN.Domain.Common;


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
        private readonly IEmailService _emailService;
        private readonly ILogger<GroupController> _logger;
        private readonly IPermissionService _permissionService;
        private readonly ApplicationDbContext _context;


        public GroupController(IGroupService groupService,
            IGroupInvitationService groupInvitationService,
            IClaimService claimService,
            ILogger<GroupController> logger,
            IEmailService emailService,
            IUserRepository userRepository,
            IPermissionService permissionService,
            ApplicationDbContext context
            )
        {
            _groupService = groupService;
            _groupInvitationService = groupInvitationService;
            _userRepository = userRepository;
            _claimService = claimService;
            _logger = logger;
            _permissionService = permissionService;
            _context = context;
            _emailService = emailService;
        }

        // [HttpGet]
        // [Route("get-joined-groups")]
        // [ProducesResponseType(typeof(ApiResult<List<GroupDto>>), StatusCodes.Status200OK)] // OK với ProductResponse
        // public async Task<IActionResult> GetUserJoinedGroupsAsync([FromQuery] PaginationFilter pagination)
        // {
        //     var pagedGroups = await _groupService.GetUserJoinedGroupsAsync(pagination);
        //     if (pagedGroups == null || !pagedGroups.Any())
        //         return NotFound("No groups found for the user");
        //     return Ok(ApiResult<List<GroupDto>>.Success(pagedGroups));
        // }

        //get user's created groups
        [HttpGet]
        [Route("get-user-created-groups")]
        [ProducesResponseType(typeof(ApiResult<List<GroupDto>>), StatusCodes.Status200OK)] // OK với ProductResponse
        public async Task<IActionResult> GetAllGroupsAsync([FromQuery] PaginationFilter pagination)
        {
            return Ok(ApiResult<List<GroupDto>>.Success(await _groupService.GetAllGroupsAsync(pagination)));
        }

        //get group's members
        [HttpGet]
        [Route("get-members")]
        [ProducesResponseType(typeof(ApiResult<List<UserGroupDto>>), StatusCodes.Status200OK)] // OK với ProductResponse
        public async Task<IActionResult> GetMembersByGroupIdAsync(Guid groupId)
        {
            return Ok(ApiResult<List<UserGroupDto>>.Success(await _groupService.GetMembersByGroupIdAsync(groupId)));
        }


        [HttpGet("search-by-usernames")]
        public async Task<IActionResult> GetUsersByUsernamesAsync([FromQuery] string usernames, [FromQuery] PaginationFilter pagination)
        {
            if (string.IsNullOrWhiteSpace(usernames))
                return BadRequest("Usernames query is required.");

            var usernameList = usernames
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .ToList();

            var pagedUsers = await _groupService.GetUsersByUsernamesAsync(usernameList, pagination);

            return Ok(ApiResult<List<UserDto>>.Success(pagedUsers));
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
        [Route("create-invitation")]
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
        [HttpPut]
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

        [HttpPut]
        [Route("reject-join-request")]
        public async Task<IActionResult> RejectJoinGroupRequestAsync([FromBody] RequestActionDto dto)
        {
            await _groupService.RejectJoinGroupRequestAsync(dto);
            return Ok(ApiResult<string>.Success("Rejected successfully"));
        }

        [HttpPost]
        [Route("send-invite")]
        public async Task<IActionResult> SendEmailAsync([FromBody] SendGroupInviteDto request)
        {
            if(request == null || !ModelState.IsValid)
                return BadRequest(ModelState);
            var user = await _userRepository.GetFirstAsync(tar => tar.Id == request.TargetUserId);
            if (string.IsNullOrEmpty(user.Email) || user == null)
                return NotFound("User not found or email missing");
            var invite = await _groupInvitationService.CreateGroupInvitationAsync(request.GroupId, request.SenderUserId);

            var inviter = await _userRepository.GetFirstAsync(u => u.Id == request.SenderUserId);
            var inviterName = inviter?.UserName ?? "Someone";

            var emailSubject = $"You've been invited to join a group!";
            Console.WriteLine($"Sending email to: {user.Email}");
            var emailBody = $"Hi {user.UserName},\n\n" +
                            $"{inviterName} has invited to join the group.\n\n" +
                            $"Click the link to join: {invite.InvitationUrl}\n\n" +
                            $"This invitation expires on: {invite.ExpirationDate:yyyy-MM-dd}";
            await _emailService.SendEmailAsync(user.Email, emailSubject, emailBody);
            return Ok(ApiResult<string>.Success($"Invitation sent successfully to {user.Email}"));
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
