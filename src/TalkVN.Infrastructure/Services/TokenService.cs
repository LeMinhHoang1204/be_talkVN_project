using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

using TalkVN.Application.Exceptions;
using TalkVN.Application.Helpers;
using TalkVN.Domain.Common;
using TalkVN.Domain.Identity;
using TalkVN.Infrastructure.ConfigSetting;

using Microsoft.IdentityModel.Tokens;

namespace TalkVN.Infrastructure.Services
{
    public class TokenService : ITokenService
    {
        private readonly JWTConfigSetting _jWTConfigSetting;
        public TokenService(JWTConfigSetting jWTConfigSetting)
        {
            _jWTConfigSetting = jWTConfigSetting;
        }
        public (string token, int validDays) GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using var randomNumberGenerator = RandomNumberGenerator.Create();
            randomNumberGenerator.GetBytes(randomNumber);
            return (Convert.ToBase64String(randomNumber), _jWTConfigSetting.RefreshTokenValidityInDays);
        }
        public string GenerateAccessToken(UserApplication user, IEnumerable<string> roles, LoginHistory loginHistory)
        {
            var key = Encoding.ASCII.GetBytes(_jWTConfigSetting.SecretKey);

            var claims = new[]
            {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(JWTConfigSetting.LoginHistoryIdClaimType, loginHistory.Id.ToString()),
            };
            // Thêm các claim cho từng role của user
            var tokenHandler = new JwtSecurityTokenHandler();

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = _jWTConfigSetting?.Issuer,
                Audience = _jWTConfigSetting?.Audience,
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddDays(_jWTConfigSetting.TokenValidityInDays),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }

        public async Task<ClaimsIdentity> GetPrincipalFromExpiredToken(string? token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                ValidIssuer = _jWTConfigSetting.Issuer,
                ValidAudience = _jWTConfigSetting.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jWTConfigSetting.SecretKey)),
                ValidateLifetime = false
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var validateResult = await tokenHandler.ValidateTokenAsync(token, tokenValidationParameters);

            if (!validateResult.IsValid)
            {
                throw new InvalidModelException(ErrorDescription.InvalidAccessOrRefreshToken);
            }

            var securityToken = validateResult.SecurityToken;

            if (securityToken is not JwtSecurityToken jwtSecurityToken || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                throw new SecurityTokenException("Invalid token");

            return validateResult.ClaimsIdentity;

        }
    }
}
