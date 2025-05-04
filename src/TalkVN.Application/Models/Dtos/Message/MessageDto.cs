namespace TalkVN.Application.Models.Dtos.Message
{
    public class MessageDto : BaseResponseDto
    {
        public string MessageText { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime UpdateOn { get; set; }
        public string SenderId { get; set; }
        public Guid TextChatId { get; set; }
        public string Status { get; set; }
    }
}
