namespace Liberty.Reservation.Employee.WebAPI.Application.Models.Responses;

public record EmailSentAuditLogResponse()
{
    public long Id { get; init; }
    public DateTime? SentAt { get; init; }
    public string? Recipient { get; init; }
    public string? Subject { get; init; }
    public string? Body { get; init; }
    public MailSentStatus MailSentStatus { get; init; }
    public MailActorTypes TriggerSource { get; init; }
}
