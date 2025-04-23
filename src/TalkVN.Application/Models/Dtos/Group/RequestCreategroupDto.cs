namespace TalkVN.Application.Models.Dtos.Group;

public class RequestCreateGroupDto
{
    public string Name { get; set; }
    public bool IsPrivate { get; set; }
    public string? Password { get; set; }
    public string? Description { get; set; }
    public int MaxQuantity { get; set; }

}
