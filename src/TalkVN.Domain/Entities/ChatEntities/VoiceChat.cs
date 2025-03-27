namespace TalkVN.Domain.Entities.ChatEntities;

public class VoiceChat : BaseAuditedEntity
{
    public string Name { get; set; }

    public string Password { get; set; }

    public GroupStatus Status { get; set; }

    public ChatType Type { get; set; }

    public int MaxQuantity { get; set; }


}
