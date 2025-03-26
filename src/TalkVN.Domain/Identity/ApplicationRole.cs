using Microsoft.AspNetCore.Identity;

using TalkVN.Domain.Entities.SystemEntities.Permissions;

namespace TalkVN.Domain.Identity
{
    public class ApplicationRole : IdentityRole
    {
       public string Description { get; set; }

       public IEnumerable<RolePermission> RolePermissions { get; set; } // Navigation property
    }
}
