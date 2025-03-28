using TalkVN.Application.Models;
using TalkVN.Application.Models.Dtos.User.Follow;
using TalkVN.Application.Models.Dtos.User.Profile;
using TalkVN.Application.Services.Interface;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TalkVN.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProfileController : ControllerBase
    {
        private readonly IProfileService _profileService;
        private readonly IFollowService _followService;
        public ProfileController(IProfileService profileService, IFollowService followService)
        {
            _profileService = profileService;
            _followService = followService;
        }
        [HttpGet]
        [Route("")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ApiResult<List<ProfileDto>>), StatusCodes.Status200OK)] // OK với ProductResponse
        public async Task<IActionResult> GetAllProfilesAsync([FromQuery] ProfileSearchQueryDto query)
        {
            return Ok(ApiResult<List<ProfileDto>>.Success(await _profileService.GetAllProfilesAsync(query)));
        }
        [HttpGet]
        [Route("{userId}")]
        [ProducesResponseType(typeof(ApiResult<ProfileDto>), StatusCodes.Status200OK)] // OK với ProductResponse

        public async Task<IActionResult> GetProfileByIdAsync(Guid userId)
        {
            return Ok(ApiResult<ProfileDto>.Success(await _profileService.GetProfileByIdAsync(userId)));
        }
        [HttpPost]
        [Route("")]
        [ProducesResponseType(typeof(ApiResult<ProfileDto>), StatusCodes.Status200OK)] // OK với ProductResponse

        public async Task<IActionResult> CreateProfileeAsync([FromBody] ProfileRequestDto request)
        {
            return Ok(ApiResult<ProfileDto>.Success(await _profileService.CreateProfileAsync(request)));
        }
        [HttpPut]
        [Route("{userId}")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ApiResult<ProfileDto>), StatusCodes.Status200OK)] // OK với ProductResponse
        public async Task<IActionResult> GetProfileByIdAsync(Guid userId, [FromBody] ProfileRequestDto Profile)
        {
            return Ok(ApiResult<ProfileDto>.Success(await _profileService.UpdateProfileAsync(userId, Profile)));
        }
        [HttpGet]
        [Route("{userId}/followers")]
        [ProducesResponseType(typeof(ApiResult<List<FollowDto>>), StatusCodes.Status200OK)] // OK với ProductResponse
        public async Task<IActionResult> GetAllFollowerAsync(Guid userId)
        {
            return Ok(ApiResult<List<FollowDto>>.Success(await _followService.GetAllFollowerAsync(userId.ToString())));
        }
        [HttpGet]
        [Route("{userId}/followings")]
        [ProducesResponseType(typeof(ApiResult<List<FollowDto>>), StatusCodes.Status200OK)] // OK với ProductResponse
        public async Task<IActionResult> GetAllFollowingsAsync(Guid userId)
        {
            return Ok(ApiResult<List<FollowDto>>.Success(await _followService.GetAllFollowingsAsync(userId.ToString())));
        }
        [HttpPut]
        [Route("toggle-follow/{otherUserId}")]
        [ProducesResponseType(typeof(ApiResult<List<FollowDto>>), StatusCodes.Status200OK)] // OK với ProductResponse
        public async Task<IActionResult> ToggleFollowAsync(string otherUserId)
        {
            return Ok(ApiResult<FollowDto>.Success(await _followService.ToggleFollowAsync(otherUserId)));
        }
        [HttpGet]
        [Route("get-reccommend-user")]
        [ProducesResponseType(typeof(ApiResult<List<FollowDto>>), StatusCodes.Status200OK)] // OK với ProductResponse
        public async Task<IActionResult> GetRecommendFollowAsync([FromQuery] PaginationFilter filter)
        {
            return Ok(ApiResult<List<FollowDto>>.Success(await _followService.GetRecommendFollowAsync(filter)));
        }
    }
}
