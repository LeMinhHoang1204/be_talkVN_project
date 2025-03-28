namespace TalkVN.Domain.Entities.PostEntities.Reaction
{
    public class ReactionComment : BaseAuditedEntity
    {
        public Guid CommentId { get; set; }
        public string UserId { get; set; }
        public Comment Comment { get; set; } // Navigation property
        public UserApplication User { get; set; } // Navigation property
    }
}
