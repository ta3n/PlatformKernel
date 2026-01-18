using Liberty.Reservation.Booking.Worker.Application.Models.Requests;
using Liberty.Reservation.Booking.Worker.Application.UserCases.Commands.EmailSentAuditLog;
using Liberty.Reservation.Booking.Worker.Consumers.Base;
using Liberty.SysIntegrationEvent.Events;
using MassTransit;
using MediatR;

namespace Liberty.Reservation.Booking.Worker.Consumers;

public class EmailSentAuditLogConsumer(
    ILogger<EmailSentAuditLogConsumer> logger,
    IMediator mediator
) : BaseConsumer<EmailSentAuditLogEvent>(logger)
{
    protected override async Task ExecuteAsync(
        ConsumeContext<EmailSentAuditLogEvent> context
    )
    {
        var command = new EmailSentAuditLogCommand { Payload = new EmailSentAuditLogRequest(context.Message.JsonData ?? string.Empty) };

        await mediator.Send(command);
    }
}
