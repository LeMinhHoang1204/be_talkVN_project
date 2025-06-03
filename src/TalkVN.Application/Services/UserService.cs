using System.Security.Claims;

using AutoMapper;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;

using TalkVN.Application.Exceptions;
using TalkVN.Application.Helpers;
using TalkVN.Application.Localization;
using TalkVN.Application.Models.Dtos.User;
using TalkVN.Application.Services.Interface;
using TalkVN.DataAccess.Repositories.Interface;
using TalkVN.DataAccess.Repositories.Interface;
using TalkVN.Domain.Entities;
using TalkVN.Domain.Identity;

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authentication.Google;

using TalkVN.Application.Models.Dtos.Group;
using TalkVN.Domain.Entities.SystemEntities.Relationships;

namespace TalkVN.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IBaseRepository<LoginHistory> _loginHistoryRepository;
        private readonly ILogger<UserService> _logger;
        private readonly IClaimService _claimService;
        private UserManager<UserApplication> _userManager;
        private RoleManager<ApplicationRole> _roleManager;
        private ITokenService _tokenService;
        private IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IBaseRepository<UserGroupRole> _userGroupRoleRepository;
        private readonly IBaseRepository<UserGroup> _userGroupRepository;

        public UserService(ILogger<UserService> logger
            , IUserRepository userRepository
            , IRepositoryFactory repositoryFactory
            , UserManager<UserApplication> userManager
            , RoleManager<ApplicationRole> roleManager
            , ITokenService tokenService
            , IMapper mapper
            , IClaimService claimService
            , IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _userRepository = userRepository;
            _loginHistoryRepository = repositoryFactory.GetRepository<LoginHistory>();
            _userManager = userManager;
            _roleManager = roleManager;
            _tokenService = tokenService;
            _mapper = mapper;
            _claimService = claimService;
            _httpContextAccessor = httpContextAccessor;
            _userGroupRoleRepository = repositoryFactory.GetRepository<UserGroupRole>();
            _userGroupRepository = repositoryFactory.GetRepository<UserGroup>();
        }

        public async Task<LoginResponseDto> LoginAsync(LoginRequestDto loginRequestDto)
        {
            UserApplication user = await _userRepository.GetFirstOrDefaultAsync(p => p.UserName == loginRequestDto.UserName);
            bool isValid = await _userManager.CheckPasswordAsync(user, loginRequestDto.Password);
            if (user == null)
            {
                throw new NotFoundException(ValidationTexts.NotFound.Format(loginRequestDto.GetType(), loginRequestDto.UserName));
            }
            if (!isValid)
            {
                throw new InvalidModelException(ValidationTexts.NotValidate.Format(user.GetType(), "Password"));
            }
            // if user is found, Generate JWT Token
            var roles = await _userManager.GetRolesAsync(user);
            LoginHistory loginHistory = new();
            loginHistory.Id = Guid.NewGuid();
            loginHistory.LoginTime = DateTime.Now;
            loginHistory.UserId = user.Id;
            loginHistory.IsDeleted = false;
            var accessToken = _tokenService.GenerateAccessToken(user, roles, loginHistory);
            var refreshToken = GenerateRefreshToken(loginHistory);
            await _loginHistoryRepository.AddAsync(loginHistory);

            UserDto userDto = _mapper.Map<UserDto>(user);
            /*   new()
           {
               Email = user.Email,
               PhoneNumber = user.PhoneNumber,
               FirstName = user.FirstName,
               LastName = user.LastName,
               Id = user.Id
           };*/
            LoginResponseDto loginResponse = new()
            {
                RefreshToken = refreshToken,
                AccessToken = accessToken,
                User = userDto,
                LoginHistoryId = loginHistory.Id
            };
            _logger.Log(LogLevel.Information, $"User {loginHistory.UserId} login at ${loginHistory.LoginTime}");
            return loginResponse;
        }

        public async Task<LoginResponseDto> LoginGoogleAsync()
        {
            var result = await _httpContextAccessor.HttpContext.AuthenticateAsync(IdentityConstants.ExternalScheme);

            if (!result.Succeeded || result?.Principal == null)
            {
                throw new InvalidModelException("Google login failed.");
            }

            var email = result.Principal.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(email))
            {
                throw new InvalidModelException("Cannot get email from Google.");
            }

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                user = new UserApplication
                {
                    Email = email,
                    UserName = email,
                    DisplayName = result.Principal.FindFirstValue(ClaimTypes.Name),
                    AvatarUrl = result.Principal.FindFirstValue("picture") ?? "", // optional
                    EmailConfirmed = true,
                    UserStatus = Domain.Enums.UserStatus.Public,
                };

                var createResult = await _userManager.CreateAsync(user);
                if (!createResult.Succeeded)
                {
                    throw new InvalidModelException("User creation failed.");
                }
            }

            var roles = await _userManager.GetRolesAsync(user);

            LoginHistory loginHistory = new()
            {
                Id = Guid.NewGuid(),
                LoginTime = DateTime.UtcNow,
                UserId = user.Id,
                IsDeleted = false,
            };

            var accessToken = _tokenService.GenerateAccessToken(user, roles, loginHistory);
            var refreshToken = GenerateRefreshToken(loginHistory);
            await _loginHistoryRepository.AddAsync(loginHistory);

            var userDto = _mapper.Map<UserDto>(user);

            return new LoginResponseDto
            {
                RefreshToken = refreshToken,
                AccessToken = accessToken,
                User = userDto,
                LoginHistoryId = loginHistory.Id
            };
        }

        public async Task<bool> LogoutAsync(Guid loginHistoryId)
        {
            LoginHistory loginHistory = await _loginHistoryRepository.GetFirstOrDefaultAsync(p => p.Id == loginHistoryId);
            loginHistory.LogoutTime = DateTime.Now;
            loginHistory.RefreshToken = null;
            loginHistory.RefreshTokenExpiryTime = null;
            await _loginHistoryRepository.UpdateAsync(loginHistory);
            _logger.Log(LogLevel.Information, $"User {loginHistory.UserId} logout at ${loginHistory.LogoutTime}");

            return true;
        }

        public async Task<RefreshTokenDto> RefreshTokenAsync(RefreshTokenDto request)
        {

            var claimIdentity = await _tokenService.GetPrincipalFromExpiredToken(request.AccessToken)
                    ?? throw new NotFoundException(request.AccessToken, typeof(ClaimsIdentity));
            string username = claimIdentity.Name;
            var user = await _userManager.FindByNameAsync(username)
                ?? throw new NotFoundException(username, typeof(UserApplication));
            var loginHistoryIdAsString = this._claimService.GetLoginHistoryId(claimIdentity);
            if (string.IsNullOrWhiteSpace(loginHistoryIdAsString))
            {
                throw new NotFoundException(loginHistoryIdAsString, typeof(LoginHistory));
            }
            var loginHistoryId = Guid.Parse(loginHistoryIdAsString);
            var roles = await _userManager.GetRolesAsync(user);

            var loginHistory = await this._loginHistoryRepository.GetFirstOrDefaultAsync(x => x.Id == loginHistoryId
                                                && x.UserId == user.Id
                                                && !x.IsDeleted);
            if (loginHistory.RefreshToken != request.RefreshToken || loginHistory.RefreshTokenExpiryTime <= DateTime.UtcNow)
            {
                throw new InvalidModelException(ErrorTexts.InvalidAccessOrRefreshToken);
            }
            var newAccessToken = _tokenService.GenerateAccessToken(user, roles, loginHistory);

            var (newRefreshTokenResult, _) = _tokenService.GenerateRefreshToken();

            loginHistory.RefreshToken = newRefreshTokenResult;

            await this._loginHistoryRepository.UpdateAsync(loginHistory);
            return new RefreshTokenDto()
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshTokenResult,
            };
        }

        public async Task<bool> RegisterAsync(RegisterationRequestDto registerationRequestDto)
        {
            var user = await _userRepository.GetFirstOrDefaultAsync(p => p.Email == registerationRequestDto.Email);
            if (user != null)
                throw new ConflictException(ValidationTexts.Conflict.Format(user.GetType(), user.UserName));
            var loginResponseDto = new LoginResponseDto();
            UserApplication newUser = new()
            {
                UserName = registerationRequestDto.Email,
                Email = registerationRequestDto.Email,
                DisplayName = registerationRequestDto.LastName + registerationRequestDto.FirstName,
                PhoneNumber = registerationRequestDto.PhoneNumber,
                NormalizedEmail = registerationRequestDto.Email.ToUpper(),
                AvatarUrl = "",
                UserStatus = Domain.Enums.UserStatus.Public,
            };
            var result = await _userManager.CreateAsync(newUser, registerationRequestDto.Password); // Hash password by .net identity
            if (result.Succeeded)
            {
                return true;
            }
            else
            {
                throw new InvalidModelException(result.ToString());
            }
        }
        private string GenerateRefreshToken(LoginHistory loginHistory)
        {
            var (refreshToken, validDays) = _tokenService.GenerateRefreshToken();
            loginHistory.RefreshToken = refreshToken;
            loginHistory.RefreshTokenExpiryTime = DateTime.Now.Add(TimeSpan.FromDays(validDays));
            return refreshToken;
        }

        public async Task<bool> AddUserGroupRoleAsync(UpdateUserRoleInGroupDto dto, string userId)
        {
            var role = await _roleManager.FindByIdAsync(dto.RoleId.ToString());
            if (role == null)
            {
                throw new NotFoundException("Role not found");
            }

            // Find the user group role
            var userGroupRole =
                await _userGroupRoleRepository.GetFirstOrDefaultAsync(x =>
                    x.UserGroup.UserId == userId && x.UserGroup.GroupId == dto.GroupId && x.RoleId == role.Id
                );

            if (userGroupRole == null)
            {
                // create a new user group role if it does not exist

                // get the role by name
                var newRole = await _roleManager.FindByNameAsync("Member");

                if (newRole == null)
                {
                    throw new NotFoundException("Role not found");
                }

                var UserGroup = await _userGroupRepository.GetFirstOrDefaultAsync(
                    x => x.GroupId == dto.GroupId && x.UserId == userId
                );

                if (UserGroup == null)
                {
                    throw new NotFoundException("User group not found");
                }

                var newUserGroupRole = new UserGroupRole
                {
                    Id = Guid.NewGuid(),
                    UserGroupId = UserGroup.Id,
                    RoleId = role.Id
                };
                await _userGroupRoleRepository.AddAsync(newUserGroupRole);
                return true;
            }
            return false;
        }
    }
}
