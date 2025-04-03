using TalkVN.Domain.Entities.SystemEntities.Relationships;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace TalkVN.DataAccess.Configurations
{

    public class UserChatRoleConfiguration : IEntityTypeConfiguration<UserChatRole>
    {
        public void Configure(EntityTypeBuilder<UserChatRole> builder)
        {
            // Primary Key
            builder.HasKey(ucr => ucr.Id);

            //configure relationships with TextChat
            builder
                .HasOne(ucr => ucr.TextChat)
                .WithMany(tc => tc.UserChatRoles)
                .HasForeignKey(ucr => ucr.TextChatId);

            //configure relationships with User
            builder
                .HasOne(ucr => ucr.User)
                .WithMany()
                .HasForeignKey(ucr => ucr.UserId);

            //configure relationships with Role
            builder
                .HasOne(ucr => ucr.Role)
                .WithMany(r => r.UserChatRoles)
                .HasForeignKey(ucr => ucr.RoleId);

            //configure relationships with VoiceChat
            builder
                .HasOne(ucr => ucr.VoiceChat)
                .WithMany(vc => vc.UserChatRoles)
                .HasForeignKey(ucr => ucr.VoiceChatId);
        }
    }
}
