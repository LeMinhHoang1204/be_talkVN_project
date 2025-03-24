using Microsoft.AspNetCore.Identity;

namespace TalkVN.Domain.Identity
{
    public class ApplicationRole : IdentityRole
    {
        public string Description { get; set; }
    }
}
