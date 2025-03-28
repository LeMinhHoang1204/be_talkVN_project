using TalkVN.Application.Models;
using TalkVN.Application.Models.Dtos.User;
using TalkVN.Application.Services.Interface;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TalkVN.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<UserController> _logger;
        public UserController(IUserService userService, ILogger<UserController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        [HttpPost("register")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ApiResult<bool>), StatusCodes.Status200OK)] // OK với ProductResponse
        public async Task<IActionResult> RegisterUser([FromBody] RegisterationRequestDto registerationRequestDto)
        {
            return Ok(ApiResult<bool>.Success(await _userService.RegisterAsync(registerationRequestDto)));
        }
        [HttpPost("login")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ApiResult<LoginResponseDto>), StatusCodes.Status200OK)] // OK với ProductResponse
        public async Task<IActionResult> LoginUser([FromBody] LoginRequestDto loginRequestDto)
        {
            return Ok(ApiResult<LoginResponseDto>.Success(await _userService.LoginAsync(loginRequestDto)));
        }
        [HttpPost("logout")]
        [ProducesResponseType(typeof(ApiResult<bool>), StatusCodes.Status200OK)] // OK với ProductResponse
        public async Task<IActionResult> LogoutUser([FromBody] Guid loginHistoryId)
        {
            return Ok(ApiResult<bool>.Success(await _userService.LogoutAsync(loginHistoryId)));
        }
        [HttpPost("refreshToken")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ApiResult<RefreshTokenDto>), StatusCodes.Status200OK)] // OK với ProductResponse
        public async Task<IActionResult> RefreshTokenAsync([FromBody] RefreshTokenDto refreshTokenDto)
        {
            return Ok(ApiResult<RefreshTokenDto>.Success(await _userService.RefreshTokenAsync(refreshTokenDto)));
        }
    }
}
