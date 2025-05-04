using Microsoft.EntityFrameworkCore.Metadata.Builders;

using TalkVN.Domain.Entities.SystemEntities.Permissions;

namespace TalkVN.DataAccess.Configurations.Permissions;

public class TextChatPermissionConfiguration : IEntityTypeConfiguration<TextChatPermission>
{
    public void Configure(EntityTypeBuilder<TextChatPermission> builder)
    {
      //  builder.ToTable("TextChatPermissions");

        builder.HasKey(x => x.Id);

        //configure relationship with TextChat
        builder
            .HasOne(x => x.TextChat)
            .WithMany(u => u.TextChatPermissions)
            .HasForeignKey(x => x.TextChatId)
            .OnDelete(DeleteBehavior.Cascade);

        //Configure relationship with User
        builder
            .HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        //configure relationship with Permission
        builder
            .HasOne(x => x.Permission)
            .WithMany()
            .HasForeignKey(x => x.PermissionId)
            .OnDelete(DeleteBehavior.Cascade);

    }
}
