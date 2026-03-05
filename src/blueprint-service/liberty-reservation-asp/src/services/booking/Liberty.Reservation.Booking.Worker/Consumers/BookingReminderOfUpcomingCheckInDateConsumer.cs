using Liberty.Reservation.Booking.Worker.Application.UserCases.Commands.BookingReminder;
using Liberty.Reservation.Booking.Worker.Consumers.Base;
using Liberty.SysIntegrationEvent.Events;
using MassTransit;
using MediatR;

namespace Liberty.Reservation.Booking.Worker.Consumers;

public class BookingReminderOfUpcomingCheckInDateConsumer(
    ILogger<BookingReminderOfUpcomingCheckInDateConsumer> logger,
    IMediator mediator
) : BaseConsumer<BookingReminderOfUpcomingCheckInDateEvent>(logger)
{
    protected override async Task ExecuteAsync(
        ConsumeContext<BookingReminderOfUpcomingCheckInDateEvent> context
    )
    {
        await mediator.Send(
            new BookingReminderOfUpcomingCheckInDateCommand { Payload = null! }
        );
    }
}
