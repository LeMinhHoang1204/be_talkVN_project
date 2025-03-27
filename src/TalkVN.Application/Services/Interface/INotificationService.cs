using TalkVN.Application.Models;
using TalkVN.Application.Models.Dtos.Notification;

namespace TalkVN.Application.Services.Interface
{
    public interface INotificationService
    {
        Task<List<NotificationDto>> GetAllNotificationsAsync(PaginationFilter filter);
        /*        Task CreateOrUpdateNotificationAsync(NotificationDto notificationDto, List<string> notificationUser);
                Task CreateNotificationAsync(NotificationDto notificationDto, List<string> notificationUser);*/
     //   Task CreateOrUpdatePostNotificationAsync(CreatePostNotificationDto createPostNotificationDto);
        Task CreateOrUpdateUserNotificationAsync(CreateUserNotificationDto createUserNotification);
       // Task CreateOrUpdateCommentNotificationAsync(CreateCommentNotificationDto createUserNotification);

    }
}
