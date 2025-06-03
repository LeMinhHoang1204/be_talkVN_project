namespace TalkVN.Domain.Enums
{
    public enum Permissions
    {
        //======================================================================
        // Quyền cơ bản cho MEMBER (Thành viên)
        //======================================================================

        // Quyền chung & Nhóm
        VIEW_USER_PROFILES,
        CREATE_GROUP,               // Thành viên có thể tự tạo nhóm mới
        JOIN_GROUP,                 // Tham gia vào một nhóm
        INVITE_TO_JOINED_GROUP,     // Mời người khác vào nhóm mình đã tham gia

        // Quyền chung trên Kênh Text Chat trong Nhóm
        READ_MESSAGES_IN_TEXT_CHANNEL,      // Đọc tin nhắn chung trong các kênh text của nhóm
        SEND_MESSAGES_IN_TEXT_CHANNEL,      // Gửi tin nhắn chung trong các kênh text của nhóm

        // Quyền chung trên Kênh Video Call trong Nhóm
        JOIN_VIDEO_CHANNEL_IN_JOINED_GROUP, // Tham gia kênh video trong nhóm đã tham gia

        // Quyền trên Kênh Text Chat Cụ Thể (khi đã có quyền truy cập kênh đó)
        READ_MESSAGES_IN_SPECIFIC_TEXT_CHANNEL,
        SEND_MESSAGES_IN_SPECIFIC_TEXT_CHANNEL,
        EDIT_OWN_MESSAGE_IN_SPECIFIC_TEXT_CHANNEL,
        DELETE_OWN_MESSAGE_IN_SPECIFIC_TEXT_CHANNEL,

        //======================================================================
        // Quyền bổ sung cho MODERATOR (Điều phối viên)
        // (Bao gồm tất cả quyền của Member)
        // Các quyền này thường áp dụng cho các nhóm mà Moderator điều phối (_JOINED_GROUP)
        //======================================================================

        // Quản lý nhóm
        OVERRIDE_PERMISSION_IN_GROUP, // Ghi đè quyền trong nhóm đã tham gia

        // Quản lý thành viên trong nhóm (JOINED_GROUP)
        BAN_USER_FROM_JOINED_GROUP,
        UNBAN_USER_FROM_JOINED_GROUP,

        // Quản lý kênh text (JOINED_GROUP context)
        EDIT_JOINED_TEXT_CHANNEL_SETTINGS,
        DELETE_JOINED_TEXT_CHANNEL,

        // Quản lý kênh video (JOINED_GROUP context)
        EDIT_JOINED_VIDEO_CHANNEL_SETTINGS,
        DELETE_JOINED_VIDEO_CHANNEL,

        // Kiểm duyệt tin nhắn (trong kênh cụ thể mà Moderator quản lý)
        DELETE_ANY_MESSAGE_IN_SPECIFIC_TEXT_CHANNEL,

        // Kiểm duyệt thành viên chung trong nhóm (JOINED_GROUP)
        MUTE_MEMBER_IN_JOINED_GROUP,
        UNMUTE_MEMBER_IN_JOINED_GROUP,
        BAN_MEMBER_FROM_USING_CAMERA_IN_JOINED_GROUP,
        UNBAN_MEMBER_FROM_USING_CAMERA_IN_JOINED_GROUP,

        // Kiểm duyệt thành viên trong kênh video cụ thể (JOINED_GROUP context)
        MUTE_MEMBER_IN_JOINED_SPECIFIC_VIDEO_CHANNEL,
        UNMUTE_MEMBER_IN_JOINED_SPECIFIC_VIDEO_CHANNEL,
        TURN_OFF_VIDEO_MEMBER_IN_JOINED_SPECIFIC_VIDEO_CHANNEL,
        BAN_MEMBER_FROM_USING_CAMERA_IN_JOINED_SPECIFIC_VIDEO_CHANNEL,
        UNBAN_MEMBER_FROM_USING_CAMERA_IN_JOINED_SPECIFIC_VIDEO_CHANNEL,

        //======================================================================
        // Quyền bổ sung cho GROUP OWNER (Chủ Nhóm)
        // (Bao gồm tất cả quyền của Member và Moderator)
        // Các quyền này thường áp dụng cho các nhóm mà họ sở hữu (_OWN_GROUP)
        //======================================================================

        // Quản lý nhóm (OWN_GROUP)
        EDIT_OWN_GROUP,
        DELETE_OWN_GROUP,
        INVITE_TO_OWN_GROUP, // KHÔNG CẦN
        ACCEPT_REQUEST_TO_JOIN_GROUP,
        DECLINE_REQUEST_TO_JOIN_GROUP,

        // Quản lý thành viên trong nhóm (OWN_GROUP)
        BAN_USER_FROM_OWN_GROUP,
        UNBAN_USER_FROM_OWN_GROUP,
        UPDATE_USER_ROLE_IN_OWN_GROUP, // Cập nhật vai trò của người dùng trong nhóm

        // Tạo và quản lý kênh trong nhóm (OWN_GROUP)
        CREATE_TEXT_CHANNEL_IN_GROUP,
        EDIT_OWN_TEXT_CHANNEL_SETTINGS,
        DELETE_OWN_TEXT_CHANNEL,
        CREATE_VIDEO_CHANNEL_IN_GROUP,
        EDIT_OWN_VIDEO_CHANNEL_SETTINGS,
        DELETE_OWN_VIDEO_CHANNEL,

        // Kiểm duyệt thành viên chung trong nhóm (OWN_GROUP)
        MUTE_MEMBER_IN_OWN_GROUP,
        UNMUTE_MEMBER_IN_OWN_GROUP,
        BAN_MEMBER_FROM_USING_CAMERA_IN_OWN_GROUP,
        UNBAN_MEMBER_FROM_USING_CAMERA_IN_OWN_GROUP,

        // Kiểm duyệt thành viên trong kênh video cụ thể (OWN_GROUP context)
        MUTE_MEMBER_IN_OWN_SPECIFIC_VIDEO_CHANNEL,
        UNMUTE_MEMBER_IN_OWN_SPECIFIC_VIDEO_CHANNEL,
        TURN_OFF_VIDEO_MEMBER_IN_OWN_SPECIFIC_VIDEO_CHANNEL,
        BAN_MEMBER_FROM_USING_CAMERA_IN_OWN_SPECIFIC_VIDEO_CHANNEL,
        UNBAN_MEMBER_FROM_USING_CAMERA_IN_OWN_SPECIFIC_VIDEO_CHANNEL,

        //======================================================================
        // SYSTEM ADMIN sẽ có tất cả các quyền trên.
        //======================================================================
    }
}
