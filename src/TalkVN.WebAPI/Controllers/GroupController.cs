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
        private readonly IGroupInvitationService _groupInvitationService;
        private readonly IClaimService _claimService;
        private readonly IUserRepository _userRepository;
        private readonly IEmailService _emailService;
        private readonly ILogger<GroupController> _logger;

        public GroupController(IGroupService groupService,
            IGroupInvitationService groupInvitationService,
            IClaimService claimService,
            ILogger<GroupController> logger,
            IEmailService emailService,
            IUserRepository userRepository)
        {
            _groupService = groupService;
            _groupInvitationService = groupInvitationService;
            _userRepository = userRepository;
            _claimService = claimService;
            _logger = logger;
            _emailService = emailService;
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

        [HttpPost]
        [Route("approve-join-request")]
        public async Task<IActionResult> ApproveJoinGroupRequestAsync([FromBody] RequestActionDto dto)
        {
            await _groupService.ApproveJoinGroupRequestAsync(dto);
            return Ok(ApiResult<string>.Success("Approved successfully"));
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
            //TODO: edit send mail controller
            await _emailService.SendEmailAsync(user.Email, emailSubject, emailBody);
            return Ok(ApiResult<string>.Success($"Invitation sent successfully to {user.Email}"));
        }

    }
}
