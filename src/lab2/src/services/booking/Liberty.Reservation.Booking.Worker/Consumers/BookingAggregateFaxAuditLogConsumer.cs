using System.Text.Json;
using Liberty.Reservation.Application.Models;
using Liberty.Reservation.Application.Models.Requests;
using Liberty.Reservation.Booking.Worker.Application.UserCases.Commands.BookingAggregateFaxAuditLog;
using Liberty.Reservation.Booking.Worker.Consumers.Base;
using Liberty.Reservation.Manager.Application.Domains.Repositories.Interfaces;
using Liberty.SysIntegrationEvent.Events;
using MassTransit;
using MediatR;

namespace Liberty.Reservation.Booking.Worker.Consumers;

public class BookingAggregateFaxAuditLogConsumer(
    ILogger<BookingAggregateFaxAuditLogConsumer> logger,
    IMediator mediator,
    IReservationRepository reservationRepository
) : BaseConsumer<BookingAggregateFaxAuditLogEvent>(logger)
{
    protected override async Task ExecuteAsync(
        ConsumeContext<BookingAggregateFaxAuditLogEvent> context
    )
    {
        var jsonContext = context.Message.JsonData;

        if (string.IsNullOrWhiteSpace(jsonContext))
        {
            logger.LogWarning("Aggregation skipped: JsonData is null or empty");
            return;
        }

        try
        {
            var model = JsonSerializer.Deserialize<SaveFaxAuditLogModel>(jsonContext);

            if (model is null)
            {
                logger.LogWarning("Aggregation skipped: Model invalid");
                return;
            }

            var reservationCode = await reservationRepository.GetCodeByIdAsync(model.BookingId);

            if (string.IsNullOrEmpty(reservationCode))
            {
                logger.LogWarning("Reservation not found for BookingId: {BookingId}", model.BookingId);
                return;
            }

            var request = new SaveBookingAggregateFaxAuditLogRequest(
                model.FaxNumber,
                model.Subject,
                model.Result,
                model.IsSuccess,
                reservationCode,
                model.Body
            );

            await mediator.Send(
                new SaveBookingAggregateFaxAuditLogCommand { Payload = request }
            );
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to process BookingAggregateFaxAuditLogEvent");
        }
    }
}
