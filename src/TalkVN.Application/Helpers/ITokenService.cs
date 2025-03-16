using System.Security.Claims;

using TalkVN.Domain.Entities;
using TalkVN.Domain.Identity;

namespace TalkVN.Application.Helpers
{
    public interface ITokenService
    {
        (string token, int validDays) GenerateRefreshToken();

        Task<ClaimsIdentity> GetPrincipalFromExpiredToken(string? token);

        string GenerateAccessToken(UserApplication user, IEnumerable<string> roles, LoginHistory loginHistory);
    }
}
