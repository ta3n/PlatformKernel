using Liberty.Reservation.Booking.Worker.Application.UserCases.Commands.BookingSearch;
using Liberty.Reservation.Booking.Worker.Consumers.Base;
using Liberty.SysIntegrationEvent.Events;
using MassTransit;
using MediatR;

namespace Liberty.Reservation.Booking.Worker.Consumers;

public class BookingSearchPrecomputeConsumer(
    ILogger<BookingSearchPrecomputeConsumer> logger,
    IMediator mediator
) : BaseConsumer<BookingSearchPrecomputeEvent>(logger)
{
    protected override async Task ExecuteAsync(
        ConsumeContext<BookingSearchPrecomputeEvent> context
    )
    {
        await mediator.Send(
            new BookingSearchPrecomputeCommand { Payload = null! }
        );
    }
}
