using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using TalkVN.Application.Services.Interface;
using TalkVN.DataAccess.Data;
using TalkVN.Domain.Entities.SystemEntities.Permissions;
using TalkVN.Domain.Identity;

namespace TalkVN.Application.Services;

// Giả sử bạn đã có các using statements cần thiết và inject AppDbContext, UserManager, RoleManager
// using Microsoft.EntityFrameworkCore;
// using TalkVN.Domain.Enums;
// using YourProject.Entities; // Namespace chứa các entity Permission, OverridePermission, UserGroupRole, RolePermission

public class PermissionService : IPermissionService // Hoặc logic tương tự trong HasPermissionHandler
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<UserApplication> _userManager;
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly ILogger<PermissionService> _logger; // Thêm Logger

    public PermissionService(ApplicationDbContext context,
                             UserManager<UserApplication> userManager,
                             RoleManager<ApplicationRole> roleManager,
                             ILogger<PermissionService> logger)
    {
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
        _logger = logger;
    }

    public async Task<bool> HasPermissionAsync(string userId, string permissionName, Guid? groupId = null, Guid? channelId = null)
    {
        _logger.LogInformation("Kiểm tra quyền: User '{UserId}', Permission '{PermissionName}', Group '{GroupId}', Channel '{ChannelId}'",
            userId, permissionName, groupId, channelId);

        // 1. Lấy thông tin Permission từ tên
        var permission = await _context.Permissions.AsNoTracking()
                                 .FirstOrDefaultAsync(p => p.Name == permissionName);
        if (permission == null)
        {
            _logger.LogWarning("Quyền '{PermissionName}' không tồn tại trong hệ thống.", permissionName);
            return false; // Quyền không tồn tại
        }

        // 2. ƯU TIÊN KIỂM TRA TRONG BẢNG OverridePermissions
        // Bảng OverridePermission của bạn có TextChatId
        // Nếu quyền này có thể áp dụng cho cả VideoChannel, bạn cần xem xét lại thiết kế của OverridePermission
        // hoặc truyền đúng channelId (TextChatId) nếu quyền đang kiểm tra là cho TextChat.
        // Hiện tại, giả sử channelId truyền vào là TextChatId nếu có.
        if (channelId.HasValue) // Override thường gắn với một ngữ cảnh cụ thể như kênh
        {
            var overridePermission = await _context.OverridePermissions.AsNoTracking()
                .FirstOrDefaultAsync(op => op.UserId == userId &&
                                            op.TextChatId == channelId.Value && // Giả sử channelId là TextChatId
                                            op.PermissionId == permission.Id &&
                                            !op.IsDeleted); // Thêm kiểm tra IsDeleted nếu có

            if (overridePermission != null)
            {
                _logger.LogInformation("Tìm thấy ghi đè quyền cho User '{UserId}' trong Channel '{ChannelId}' cho Permission '{PermissionName}'. IsAllowed: {IsAllowed}",
                    userId, channelId, permissionName, overridePermission.IsAllowed);
                return overridePermission.IsAllowed; // Trả về giá trị IsAllowed từ bảng ghi đè
            }
            _logger.LogInformation("Không tìm thấy ghi đè quyền cụ thể cho User '{UserId}' trong Channel '{ChannelId}'. Kiểm tra quyền theo vai trò.", userId, channelId);
        }
        else
        {
            _logger.LogInformation("Không có ChannelId được cung cấp, bỏ qua kiểm tra OverridePermissions (hoặc bạn có thể có logic override không cần channelId).");
        }


        // 3. NẾU KHÔNG CÓ OVERRIDE, KIỂM TRA QUYỀN DỰA TRÊN VAI TRÒ (ROLES)
        List<string> userRoleIds = new List<string>();

        if (groupId.HasValue)
        {
            // Lấy RoleId của người dùng trong nhóm cụ thể từ UserGroup -> UserGroupRole
            // Cấu trúc UserGroupRole của bạn: UserGroupId (FK đến UserGroup), RoleId
            // Cấu trúc UserGroup của bạn: UserId, GroupId
            userRoleIds = await _context.UserGroupRoles
                .AsNoTracking()
                .Where(ugr => ugr.UserGroup.UserId == userId && ugr.UserGroup.GroupId == groupId.Value && !ugr.IsDeleted) // Thêm !ugr.IsDeleted nếu có
                .Select(ugr => ugr.RoleId)
                .Distinct()
                .ToListAsync();

            _logger.LogInformation("User '{UserId}' có các RoleIds trong Group '{GroupId}': {RoleIds}", userId, groupId, string.Join(", ", userRoleIds));

        }
        // Optional: Xử lý vai trò toàn cục (ví dụ: SystemAdmin) nếu quyền không phụ thuộc groupId
        // hoặc nếu người dùng có vai trò toàn cục thì mặc định có một số quyền nhất định.
        // Ví dụ:
        var user = await _userManager.FindByIdAsync(userId);
        if (user != null)
        {
            var systemLevelRoleNames = await _userManager.GetRolesAsync(user); // Lấy tên vai trò Identity
            var systemLevelRoleIds = await _roleManager.Roles
                                            .Where(r => systemLevelRoleNames.Contains(r.Name))
                                            .Select(r => r.Id) // Id của ApplicationRole là string
                                            .ToListAsync();
            userRoleIds.AddRange(systemLevelRoleIds);
            userRoleIds = userRoleIds.Distinct().ToList(); // Loại bỏ trùng lặp nếu có
            _logger.LogInformation("User '{UserId}' có các RoleIds toàn cục (SystemAdmin,...): {SystemRoleIds}", userId, string.Join(", ", systemLevelRoleIds));
        }


        if (!userRoleIds.Any())
        {
            _logger.LogInformation("User '{UserId}' không có vai trò nào phù hợp trong ngữ cảnh (Group '{GroupId}') hoặc vai trò toàn cục để kiểm tra quyền '{PermissionName}'.", userId, groupId, permissionName);
            return false; // Người dùng không có vai trò nào phù hợp
        }

        // Kiểm tra xem có bất kỳ vai trò nào của người dùng được gán quyền yêu cầu không
        bool hasPermissionBasedOnRole = await _context.RolePermissions
            .AsNoTracking()
            .AnyAsync(rp => userRoleIds.Contains(rp.RoleId) && rp.PermissionId == permission.Id && !rp.IsDeleted); // Thêm !rp.IsDeleted

        if (hasPermissionBasedOnRole)
        {
            _logger.LogInformation("User '{UserId}' CÓ quyền '{PermissionName}' dựa trên vai trò.", userId, permissionName);
            return true;
        }

        _logger.LogInformation("User '{UserId}' KHÔNG có quyền '{PermissionName}' dựa trên vai trò.", userId, permissionName);
        return false;
    }

    public async Task OverridePermissionAsync(string userId, Guid permissionId, Guid textChatId, bool isAllowed)
    {
        var existingOverride = await _context.OverridePermissions
            .FirstOrDefaultAsync(op => op.UserId == userId &&
                                        op.PermissionId == permissionId &&
                                        op.TextChatId == textChatId);

        if (existingOverride != null)
        {
            existingOverride.IsAllowed = isAllowed;
            existingOverride.IsDeleted = false; // Đảm bảo bản ghi không bị đánh dấu xóa
            _context.OverridePermissions.Update(existingOverride);
        }
        else
        {
            var newOverride = new OverridePermission
            {
                UserId = userId,
                PermissionId = permissionId,
                TextChatId = textChatId,
                IsAllowed = isAllowed,
            };
            await _context.OverridePermissions.AddAsync(newOverride);
        }

        await _context.SaveChangesAsync();
    }
}
