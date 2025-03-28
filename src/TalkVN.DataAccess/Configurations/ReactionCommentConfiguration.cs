using TalkVN.Domain.Entities.PostEntities.Reaction;

using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace TalkVN.DataAccess.Configurations
{
    public class ReactionCommentConfiguration : IEntityTypeConfiguration<ReactionComment>
    {
        public void Configure(EntityTypeBuilder<ReactionComment> modelBuilder)
        {
            modelBuilder
            .HasKey(rc => new { rc.CommentId, rc.UserId });

            modelBuilder
                .HasOne(rc => rc.Comment)
                .WithMany()
                .HasForeignKey(rc => rc.CommentId)
                 .OnDelete(DeleteBehavior.Cascade);

            modelBuilder
                .HasOne(rc => rc.User)
                .WithMany()
                .HasForeignKey(rc => rc.UserId);

        }
    }
}
