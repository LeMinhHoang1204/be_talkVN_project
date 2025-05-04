using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;

using TalkVN.Application.Models;
using TalkVN.Application.Models.Dtos.User;
using TalkVN.Application.Services.Interface;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using TalkVN.Application.Services.Caching;

namespace TalkVN.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<UserController> _logger;
        private readonly ICachingService _cacheService;
        public UserController(IUserService userService, ILogger<UserController> logger, ICachingService cacheService)
        {
            _userService = userService;
            _logger = logger;
            _cacheService = cacheService;
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

        [HttpGet("login-google")]
        [AllowAnonymous]
        public async Task LoginGoogle()
        {
            var redirectUrl = Url.Action("GoogleResponse", "User", null, Request.Scheme);
            var props = new AuthenticationProperties
            {
                RedirectUri = redirectUrl,
            };
            await HttpContext.ChallengeAsync(GoogleDefaults.AuthenticationScheme, props);
        }

        [HttpGet("login-google/response", Name = "GoogleResponse")]
        [AllowAnonymous]
        public async Task<IActionResult> GoogleResponse()
        {
            Console.WriteLine("Google response");
            // return Ok(ApiResult<LoginResponseDto>.Success(await _userService.LoginGoogleAsync()));

            LoginResponseDto loginResponseDto = await this._userService.LoginGoogleAsync();

            // generate a new auth code
            var authCode = Guid.NewGuid().ToString("N");

            // cache the auth code, default timespan have 1 hour
            await _cacheService.GetOrSetAsync(authCode, () => Task.FromResult(loginResponseDto));

            // redirect to the client with the auth code
            // local environment
            // return Redirect($"http://localhost:3000/auth/google/callback?authCode={authCode}");

            // production environment
            return Redirect($"https://talkvn.vercel.app/auth/google/callback?authCode={authCode}");
        }

        [HttpPost("exchange-authcode")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ApiResult<LoginResponseDto>), StatusCodes.Status200OK)] // OK với ProductResponse
        public async Task<IActionResult> ExchangeAuthCode([FromQuery] string authCode)
        {
            LoginResponseDto loginResponseDto = await _cacheService.Get<LoginResponseDto>(authCode);

            if (loginResponseDto == null)
            {
                return BadRequest("Invalid or expired auth code.");
            }

            return Ok(ApiResult<LoginResponseDto>.Success(loginResponseDto));
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
