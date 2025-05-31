// ApplicationDbSeeder.cs
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging; // Thêm using này
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using TalkVN.DataAccess.Data;
using TalkVN.Domain.Entities.SystemEntities.Permissions;
using TalkVN.Domain.Enums;
using TalkVN.Domain.Identity; // Namespace chứa enum Permissions và Role
// Giả sử bạn có các Entity sau:
// using YourProject.Data; // Namespace chứa ApplicationDbContext
// using YourProject.Models; // Namespace chứa ApplicationRole, Permission, RolePermission

public static class ApplicationDbSeeder
{
    // --- SEED PERMISSIONS ---
    public static async Task SeedPermissionsAsync(ApplicationDbContext context, ILogger logger)
    {
        logger.LogInformation("Seeding Permissions...");
        var permissionNamesFromEnum = Enum.GetNames(typeof(TalkVN.Domain.Enums.Permissions));
        bool hasNewPermissions = false;

        foreach (var permissionName in permissionNamesFromEnum)
        {
            if (!await context.Permissions.AnyAsync(p => p.Name == permissionName)) // Permissions là DbSet<Permission> trong ApplicationDbContext
            {
                context.Permissions.Add(new Permission // Permission là Entity class của bạn
                {
                    Id = Guid.NewGuid(), // Đảm bảo Permission.Id là Guid
                    Name = permissionName,
                });
                hasNewPermissions = true;
                logger.LogInformation($"Permission '{permissionName}' will be added.");
            }
        }

        if (hasNewPermissions)
        {
            await context.SaveChangesAsync();
            logger.LogInformation("New permissions saved to database.");
        }
        else
        {
            logger.LogInformation("No new permissions to add.");
        }
    }

    // --- SEED ROLES ---
    public static async Task SeedRolesAsync(RoleManager<ApplicationRole> roleManager, ILogger logger)
    {
        logger.LogInformation("Seeding Roles...");
        var roleNamesFromEnum = Enum.GetNames(typeof(TalkVN.Domain.Enums.Role));
        foreach (var roleName in roleNamesFromEnum)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                var newRole = new ApplicationRole
                {
                    Name = roleName,
                    NormalizedName = roleName.ToUpperInvariant(), // Quan trọng cho Identity
                    Description = $"Vai trò: {roleName}"
                };
                var result = await roleManager.CreateAsync(newRole);
                if (result.Succeeded)
                {
                    logger.LogInformation($"Role '{roleName}' created successfully.");
                }
                else
                {
                    logger.LogError($"Error creating role '{roleName}': {string.Join(", ", result.Errors.Select(e => e.Description))}");
                }
            }
        }
        logger.LogInformation("Roles seeding finished.");
    }

    // --- SEED ROLE PERMISSIONS ---
    public static async Task SeedRolePermissionsAsync(ApplicationDbContext context, RoleManager<ApplicationRole> roleManager, ILogger logger)
    {
        logger.LogInformation("Seeding RolePermissions...");
        bool hasNewRolePermissions = false;

        // Gán quyền cho từng vai trò
        // Helper function để gán nhiều quyền cho một vai trò
        async Task AssignPermissionsToRole(string roleName, List<TalkVN.Domain.Enums.Permissions> permissionsToAssign)
        {
            var role = await roleManager.FindByNameAsync(roleName);
            if (role == null)
            {
                logger.LogWarning($"Role '{roleName}' not found. Skipping permission assignment.");
                return;
            }

            foreach (var pEnum in permissionsToAssign)
            {
                string pName = pEnum.ToString();
                var permission = await context.Permissions.FirstOrDefaultAsync(p => p.Name == pName);
                if (permission != null)
                {
                    if (!await context.RolePermissions.AnyAsync(rp => rp.RoleId == role.Id && rp.PermissionId == permission.Id))
                    {
                        context.RolePermissions.Add(new RolePermission { RoleId = role.Id, PermissionId = permission.Id });
                        hasNewRolePermissions = true;
                        logger.LogInformation($"Assigning permission '{pName}' to role '{roleName}'.");
                    }
                }
                else
                {
                    logger.LogWarning($"Permission '{pName}' not found in DB. Cannot assign to role '{roleName}'.");
                }
            }
        }

        // Định nghĩa quyền cho Member
        var memberPermissions = new List<TalkVN.Domain.Enums.Permissions> {
            Permissions.VIEW_USER_PROFILES, Permissions.CREATE_GROUP, Permissions.JOIN_GROUP,
            Permissions.READ_MESSAGES_IN_TEXT_CHANNEL, Permissions.SEND_MESSAGES_IN_TEXT_CHANNEL,
            Permissions.JOIN_VIDEO_CHANNEL_IN_JOINED_GROUP, Permissions.READ_MESSAGES_IN_SPECIFIC_TEXT_CHANNEL,
            Permissions.SEND_MESSAGES_IN_SPECIFIC_TEXT_CHANNEL, Permissions.EDIT_OWN_MESSAGE_IN_SPECIFIC_TEXT_CHANNEL,
            Permissions.DELETE_OWN_MESSAGE_IN_SPECIFIC_TEXT_CHANNEL, Permissions.INVITE_TO_JOINED_GROUP
        };
        await AssignPermissionsToRole(TalkVN.Domain.Enums.Role.Member.ToString(), memberPermissions);

        // Định nghĩa quyền cho Moderator (bao gồm quyền của Member + thêm)
        var moderatorPermissions = new List<TalkVN.Domain.Enums.Permissions>(memberPermissions) { // Kế thừa từ Member
            Permissions.BAN_USER_FROM_JOINED_GROUP, Permissions.UNBAN_USER_FROM_JOINED_GROUP,
            Permissions.EDIT_JOINED_TEXT_CHANNEL_SETTINGS, Permissions.DELETE_JOINED_TEXT_CHANNEL,
            Permissions.EDIT_JOINED_VIDEO_CHANNEL_SETTINGS, Permissions.DELETE_JOINED_VIDEO_CHANNEL,
            Permissions.DELETE_ANY_MESSAGE_IN_SPECIFIC_TEXT_CHANNEL, Permissions.MUTE_MEMBER_IN_JOINED_GROUP,
            Permissions.UNMUTE_MEMBER_IN_JOINED_GROUP, Permissions.BAN_MEMBER_FROM_USING_CAMERA_IN_JOINED_GROUP,
            Permissions.UNBAN_MEMBER_FROM_USING_CAMERA_IN_JOINED_GROUP, Permissions.MUTE_MEMBER_IN_JOINED_SPECIFIC_VIDEO_CHANNEL,
            Permissions.UNMUTE_MEMBER_IN_JOINED_SPECIFIC_VIDEO_CHANNEL, Permissions.TURN_OFF_VIDEO_MEMBER_IN_JOINED_SPECIFIC_VIDEO_CHANNEL,
            Permissions.BAN_MEMBER_FROM_USING_CAMERA_IN_JOINED_SPECIFIC_VIDEO_CHANNEL,
            Permissions.UNBAN_MEMBER_FROM_USING_CAMERA_IN_JOINED_SPECIFIC_VIDEO_CHANNEL
        };
        await AssignPermissionsToRole(TalkVN.Domain.Enums.Role.Moderator.ToString(), moderatorPermissions);

        // Định nghĩa quyền cho GroupOwner (bao gồm quyền của Moderator + thêm)
        var groupOwnerPermissions = new List<TalkVN.Domain.Enums.Permissions>(moderatorPermissions) { // Kế thừa từ Moderator
            Permissions.EDIT_OWN_GROUP, Permissions.DELETE_OWN_GROUP, Permissions.INVITE_TO_OWN_GROUP,
            Permissions.ACCEPT_REQUEST_TO_JOIN_GROUP, Permissions.DECLINE_REQUEST_TO_JOIN_GROUP,
            Permissions.BAN_USER_FROM_OWN_GROUP, Permissions.UNBAN_USER_FROM_OWN_GROUP,
            Permissions.CREATE_TEXT_CHANNEL_IN_GROUP, Permissions.EDIT_OWN_TEXT_CHANNEL_SETTINGS, Permissions.DELETE_OWN_TEXT_CHANNEL,
            Permissions.CREATE_VIDEO_CHANNEL_IN_GROUP, Permissions.EDIT_OWN_VIDEO_CHANNEL_SETTINGS, Permissions.DELETE_OWN_VIDEO_CHANNEL,
            Permissions.MUTE_MEMBER_IN_OWN_GROUP, Permissions.UNMUTE_MEMBER_IN_OWN_GROUP,
            Permissions.BAN_MEMBER_FROM_USING_CAMERA_IN_OWN_GROUP, Permissions.UNBAN_MEMBER_FROM_USING_CAMERA_IN_OWN_GROUP,
            Permissions.MUTE_MEMBER_IN_OWN_SPECIFIC_VIDEO_CHANNEL, Permissions.UNMUTE_MEMBER_IN_OWN_SPECIFIC_VIDEO_CHANNEL,
            Permissions.TURN_OFF_VIDEO_MEMBER_IN_OWN_SPECIFIC_VIDEO_CHANNEL,
            Permissions.BAN_MEMBER_FROM_USING_CAMERA_IN_OWN_SPECIFIC_VIDEO_CHANNEL,
            Permissions.UNBAN_MEMBER_FROM_USING_CAMERA_IN_OWN_SPECIFIC_VIDEO_CHANNEL
        };

        await AssignPermissionsToRole(TalkVN.Domain.Enums.Role.GroupOwner.ToString(), groupOwnerPermissions);


        // Định nghĩa quyền cho SystemAdmin (tất cả các quyền)
        var allPermissionEnums = Enum.GetValues(typeof(TalkVN.Domain.Enums.Permissions)).Cast<TalkVN.Domain.Enums.Permissions>().ToList();
        await AssignPermissionsToRole(TalkVN.Domain.Enums.Role.SystemAdmin.ToString(), allPermissionEnums);


        if (hasNewRolePermissions)
        {
            await context.SaveChangesAsync();
            logger.LogInformation("New role permissions saved to database.");
        }
        else
        {
            logger.LogInformation("No new role permissions to add.");
        }
    }

    // Hàm tổng hợp để gọi tất cả các hàm seeding
    public static async Task SeedAllAsync(ApplicationDbContext context, RoleManager<ApplicationRole> roleManager, ILogger logger)
    {
        logger.LogInformation("Starting database seeding process...");
        await SeedPermissionsAsync(context, logger);
        await SeedRolesAsync(roleManager, logger);
        // Quan trọng: Phải seed Permissions và Roles TRƯỚC KHI seed RolePermissions
        await SeedRolePermissionsAsync(context, roleManager, logger);
        logger.LogInformation("Database seeding process finished.");
    }
}
