using TalkVN.Application.Models.Dtos.User;
using TalkVN.Domain.Enums;

namespace TalkVN.Application.Models.Dtos.Group
{
    public class GroupDto : BaseResponseDto
    {
        // public Guid Id { get; set; }
        public string Name { get; set; }
        public bool IsPrivate { get; set; }
        public string? Avatar { get; set; }
        public string? Url  { get; set; }
        public string? Description { get; set; }
        public GroupStatus Status { get; set; }
        public int MaxQuantity { get; set; }
        public UserDto Creator { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }
    }
}
