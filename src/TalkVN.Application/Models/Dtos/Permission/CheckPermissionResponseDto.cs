namespace TalkVN.Application.Models.Dtos.Permission
{
    public class CheckPermissionResponseDto
    {
        public bool Allowed { get; set; }
        public string Reason { get; set; }
    }
}
