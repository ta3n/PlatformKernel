using Liberty.Reservation.Booking.Worker.Application.UserCases.Commands.BookingReminder;
using Liberty.Reservation.Booking.Worker.Consumers.Base;
using Liberty.SysIntegrationEvent.Events;
using MassTransit;
using MediatR;

namespace Liberty.Reservation.Booking.Worker.Consumers;

public class BookingCancellationFeeReminderConsumer(
    ILogger<BookingCancellationFeeReminderConsumer> logger,
    IMediator mediator
) : BaseConsumer<BookingCancellationFeeReminderEvent>(logger)
{
    protected override async Task ExecuteAsync(
        ConsumeContext<BookingCancellationFeeReminderEvent> context
    )
    {
        await mediator.Send(
            new BookingCancellationFeeReminderCommand { Payload = null! }
        );
    }
}
