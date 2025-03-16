using System.Security.Claims;

namespace TalkVN.Application.Helpers
{
    public interface IClaimService
    {
        string GetUserId();
        string GetUserName();
        string GetLoginHistoryId(ClaimsIdentity? claimsIdentity = null);

    }
}
