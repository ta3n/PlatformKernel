namespace Liberty.Reservation.Application.Contexts.DataContexts.Entities.Contents;

public class MessageContent
{
    public List<MessageData> MessageData { get; set; } = [];
}

public class MessageData
{
    public string? Code { get; set; }
    public string? Name { get; set; }
    public string? Caption { get; set; }
    public string? Message { get; set; }
    public string? Approach { get; set; }
    public bool IsNotFound { get; set; }
}
