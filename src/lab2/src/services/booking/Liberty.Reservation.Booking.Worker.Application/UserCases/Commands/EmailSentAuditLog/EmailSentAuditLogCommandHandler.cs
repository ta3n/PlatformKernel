using Liberty.Reservation.Application.Contexts.DataContexts.Entities.AggregateLogs;
using Liberty.Reservation.Application.Domains.Services.Interfaces;
using Newtonsoft.Json;

namespace Liberty.Reservation.Booking.Worker.Application.UserCases.Commands.EmailSentAuditLog;

public class EmailSentAuditLogCommandHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IBookingAggregateMailAuditLogService bookingAggregateMailAuditLogService
) : ActionCommandHandlerBase<EmailSentAuditLogCommand, bool>(unitOfWork, mapper)
{
    protected override async Task<bool> HandleAsync(
        EmailSentAuditLogCommand request,
        CancellationToken cancellationToken
    )
    {
        var jsonContext = request.Payload.JsonData;
        var message = JsonConvert.DeserializeObject<MessageDto>(jsonContext);

        if (message is null)
        {
            return false;
        }

        var triggerSource = message.IsReminderEmail
            ? MailActorTypes.Manager
            : message.TriggerSource;

        var emailAuditLog = new BookingAggregateMailAuditLog
        {
            TriggerSource = triggerSource,
            Recipient = string.Join(";", message.Tos),
            Subject = message.Subject,
            Body = message.Body,
            MailSentStatus = message.MailSentStatus,
            EventType = message.EventType
        };

        await bookingAggregateMailAuditLogService.CreateAsync(emailAuditLog, true, cancellationToken);

        return true;
    }

    private sealed record MessageDto(
        long BookingId,
        string[] Tos,
        string Subject,
        string Body,
        bool IsSendFax,
        string FaxNumber,
        string? FromDisplayName,
        MailActorTypes TriggerSource,
        string? EventType
    )
    {
        public MailSentStatus MailSentStatus { get; set; } = MailSentStatus.Success;
        public bool IsReminderEmail { get; set; } = false;
    };
}
