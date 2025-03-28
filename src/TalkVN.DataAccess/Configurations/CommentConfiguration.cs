using TalkVN.Domain.Entities.PostEntities;

using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace TalkVN.DataAccess.Configurations
{
    public class CommentConfiguration : IEntityTypeConfiguration<Comment>
    {
        public void Configure(EntityTypeBuilder<Comment> modelBuilder)
        {
            // Configure primary key
            modelBuilder.HasKey(c => c.Id);

            // Configure relationship with Post
            modelBuilder
                .HasOne(c => c.Post)
                .WithMany(p => p.Comments)
                .HasForeignKey(c => c.PostId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure relationship with ParentComment
            modelBuilder
                .HasOne(c => c.ParentComment)
                .WithMany(c => c.ReplyComments)
                .HasForeignKey(c => c.ParentCommentId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure relationship with UserPosted
            modelBuilder
                .HasOne(c => c.UserPosted)
                .WithMany()
                .HasForeignKey(c => c.UserPostedId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
