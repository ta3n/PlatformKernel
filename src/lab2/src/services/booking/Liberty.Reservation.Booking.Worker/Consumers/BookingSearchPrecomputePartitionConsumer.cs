using System.Text.Json;
using Liberty.Reservation.Application.Models;
using Liberty.Reservation.Booking.Worker.Application.Models.Requests;
using Liberty.Reservation.Booking.Worker.Application.UserCases.Commands.BookingSearch;
using Liberty.Reservation.Booking.Worker.Consumers.Base;
using Liberty.Reservation.Manager.Application.Models;
using Liberty.SysIntegrationEvent.Events;
using MassTransit;
using MediatR;

namespace Liberty.Reservation.Booking.Worker.Consumers;

public class BookingSearchPrecomputePartitionConsumer(
    ILogger<BookingSearchPrecomputePartitionConsumer> logger,
    IMediator mediator
) : BaseConsumer<BookingSearchPrecomputePartitionEvent>(logger)
{
    protected override async Task ExecuteAsync(
        ConsumeContext<BookingSearchPrecomputePartitionEvent> context
    )
    {
        var jsonContext = context.Message.JsonData;
        if (string.IsNullOrWhiteSpace(jsonContext))
        {
            logger.LogWarning("PrecomputePartitionEvent skipped: JsonData is null or empty");
            return;
        }

        try
        {
            var model = JsonSerializer.Deserialize<FacilitySiteFlatAvailableModel>(
                context.Message.JsonData!
            );
            await mediator.Send(
                new BookingSearchPrecomputePartitionCommand { Payload = model! }
            );
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"Failed to process {nameof(BookingSearchPrecomputePartitionEvent)}");
        }
    }
}
