using Microsoft.AspNetCore.Identity;

using TalkVN.Domain.Entities.SystemEntities.Permissions;
using TalkVN.Domain.Entities.SystemEntities.Relationships;

namespace TalkVN.Domain.Identity
{
    public class ApplicationRole : IdentityRole
    {
       public string Description { get; set; }

       public IEnumerable<RolePermission> RolePermissions { get; set; } // Navigation property

       public IEnumerable<UserGroupRole> UserGroupRoles { get; set; } // Navigation property

       public IEnumerable<UserChatRole> UserChatRoles { get; set; } // Navigation property
    }
}
