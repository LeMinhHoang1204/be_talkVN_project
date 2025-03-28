namespace TalkVN.Domain.Entities.PostEntities
{
    public class PostMedia : BaseEntity
    {
        public Guid PostId { get; set; }
        public string MediaType { get; set; }
        public string MediaUrl { get; set; }
        public int MediaOrder { get; set; }
        public string Description { get; set; }
        public Post Post { get; set; } // Navigation property
    }
}
