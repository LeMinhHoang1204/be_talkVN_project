using System.Reflection;

using TalkVN.Domain.Entities;
using TalkVN.Domain.Entities.ChatEntities;
using TalkVN.Domain.Entities.PostEntities;
using TalkVN.Domain.Entities.PostEntities.Reaction;
using TalkVN.Domain.Entities.SystemEntities;
using TalkVN.Domain.Entities.SystemEntities.Notification;
using TalkVN.Domain.Entities.UserEntities;
using TalkVN.Domain.Identity;

using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

using TalkVN.Domain.Entities.SystemEntities.Group;
using TalkVN.Domain.Entities.SystemEntities.Permissions;
using TalkVN.Domain.Entities.SystemEntities.Relationships;

namespace TalkVN.DataAccess.Data
{
    public class ApplicationDbContext : IdentityDbContext<UserApplication, ApplicationRole, string>
    {
        public DbSet<UserApplication> UserApplications { get; set; }
        public DbSet<UserFollower> UserFollowers { get; set; }
        public DbSet<UserFollowerRequest> UserFollowerRequests { get; set; }
        public DbSet<TextChat> TextChats { get; set; }
        public DbSet<TextChatParticipant> TextChatParticipants { get; set; }
        public DbSet<LoginHistory> LoginHistories { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<PostMedia> PostMedias { get; set; }
        public DbSet<PostDetailTag> PostDetailTags { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<ReactionPost> ReactionPosts { get; set; }
        public DbSet<ReactionComment> ReactionComments { get; set; }
        public DbSet<UserInteraction> UserInteractions { get; set; }
        public DbSet<PostNotifications> PostNotifications { get; set; }
        public DbSet<CommentNotifications> CommentNotifications { get; set; }
        public DbSet<Profile> Profiles { get; set; }
        public DbSet<UserNotifications> UserNotifications { get; set; }

        public DbSet<Group> Groups { get; set; }

        public DbSet<UserGroupRole> UserGroupRoles { get; set; }

        public DbSet<UserGroup> UserGroups { get; set; }

        public DbSet<MeetingSchedule> MeetingSchedules { get; set; }

        public DbSet<Permission> Permissions { get; set; }

        public DbSet<ApplicationRole> ApplicationRoles { get; set; }

        public DbSet<RolePermission> RolePermissions { get; set; }

       // public DbSet<TextChatPermission> TextChatPermissions { get; set; }

        public DbSet<GroupNotifications> GroupNotifications { get; set; }

        public DbSet<NotificationReceivers> NotificationReceivers { get; set; }

        public DbSet<OverridePermission> OverridePermissions { get; set; }

        public DbSet<GroupInvitation> GroupInvitations { get; set; }

        public DbSet<JoinGroupRequest> JoinGroupRequests { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            base.OnModelCreating(builder);
        }
    }
}
