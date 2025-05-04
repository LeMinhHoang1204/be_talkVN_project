using TalkVN.Application.Models.Dtos.User;

namespace TalkVN.Application.Services.Interface
{
    public interface IUserService
    {
        Task<bool> RegisterAsync(RegisterationRequestDto registerationRequestDto);
        Task<LoginResponseDto> LoginAsync(LoginRequestDto loginRequestDto);
        Task<LoginResponseDto> LoginGoogleAsync();
        Task<RefreshTokenDto> RefreshTokenAsync(RefreshTokenDto refreshTokenDto);
        Task<bool> LogoutAsync(Guid loginHistoryId);
    }
}
