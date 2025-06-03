namespace TalkVN.Application.Services.Interface;

public interface IPermissionService
{
    Task<bool> HasPermissionAsync(string userId, string permissionName, Guid? groupId = null, Guid? channelId = null);

    Task OverridePermissionAsync(string userId, Guid permissionId, Guid textChatId, bool isAllowed);
}
