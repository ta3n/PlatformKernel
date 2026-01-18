using Liberty.Entity;
using Liberty.Reservation.Application.Constants;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Entities.AggregateLogs;

public class BookingAggregateMailAuditLog : BaseAuditLog
{
    public MailActorTypes TriggerSource { get; set; }
    public string? Recipient { get; set; }
    public string? Subject { get; set; }
    public string? Body { get; set; }
    public MailSentStatus MailSentStatus { get; set; }
}
