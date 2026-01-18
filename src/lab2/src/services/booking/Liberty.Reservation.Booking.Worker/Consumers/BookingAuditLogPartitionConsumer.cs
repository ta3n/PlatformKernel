using System.Text.Json;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.AggregateLogs;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Application.Domains.Services.Interfaces;
using Liberty.Reservation.Application.Models;
using Liberty.Reservation.Application.Models.Responses;
using Liberty.Reservation.Booking.Worker.Application.Models;
using Liberty.SysIntegrationEvent.Events;
using MassTransit;

namespace Liberty.Reservation.Booking.Worker.Consumers;

public class BookingAuditLogPartitionConsumer(
    ILogger<BookingAuditLogPartitionConsumer> logger,
    IBookingAggregateAuditLogService bookingAggregateAuditLogService
) : IConsumer<BookingAuditLogPartitionEvent>
{
    private const string CreateEvent = "BookingCreatedEvent";

    public async Task Consume(
        ConsumeContext<BookingAuditLogPartitionEvent> context
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
            var model = JsonSerializer.Deserialize<BookingAuditLogModel>(jsonContext);
            if (model is null)
            {
                logger.LogWarning("Aggregation skipped: Model invalid");
                return;
            }

            var bookingId = model.ExistReserveId ?? model.AggregateId;
            var reservationLastest = await bookingAggregateAuditLogService.FindLastestReservationAsync(bookingId);
            var requestLast = string.Empty;
            var auditLogs = new List<BookingAuditLogResponse>();

            if (reservationLastest is not null)
            {
                if (model.ExistReserveId is not null)
                {
                    if (reservationLastest.EventType == CreateEvent)
                    {
                        var requestCreate = JsonSerializer.Deserialize<PayloadOfRequestCreate>(reservationLastest.Request!);
                        var adjustModel = new PayloadOfRequestAdjust
                        {
                            Payload = requestCreate?.Payload?.Adjust! with
                            {
                                PlanQuestions = requestCreate.Payload.PlanQuestions,
                                OptionsQuestions = requestCreate.Payload.OptionsQuestions
                            }
                        };
                        requestLast = JsonSerializer.Serialize(adjustModel);
                    }
                    else
                    {
                        requestLast = reservationLastest.Request!;
                    }
                }

                auditLogs = reservationLastest.ChangedFields is not null
                    ? JsonSerializer.Deserialize<List<BookingAuditLogResponse>>(reservationLastest.ChangedFields)
                    : null;
            }

            var request = model.Request!;
            var changedJson = new List<ChangeOfBookingAuditLogResponse>();
            if (model.ExistReserveId is not null)
            {
                var requestAdjust = JsonSerializer.Deserialize<PayloadOfRequestAdjust>(request);
                var oldRequest = JsonSerializer.Deserialize<PayloadOfRequestAdjust>(requestLast);
                changedJson.AddRange(
                    await bookingAggregateAuditLogService.DiffRequestBookingAuditLogsAsync(
                        requestAdjust?.Payload!,
                        oldRequest?.Payload!,
                        model.LanguageCode,
                        context.CancellationToken
                    )
                );
            }

            var bookingAuditLog = new BookingAuditLogResponse(
                AppDate.GetId(DateTime.UtcNow),
                model.UserCode,
                changedJson
            ) { ChangeDate = DateTime.UtcNow };
            auditLogs!.Add(bookingAuditLog);

            var log = new BookingAggregateAuditLog
            {
                AggregateCode = model.AggregateCode,
                AggregateId = model.AggregateId,
                EventType = model.EventType,
                Request = request,
                ChangedFields = JsonSerializer.Serialize(auditLogs),
                UserCode = model.UserCode,
                FacilityId = model.FacilityId,
                SiteId = model.SiteId
            };

            await bookingAggregateAuditLogService.CreateAsync(log, true, context.CancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"Failed to process {nameof(BookingAuditLogPartitionEvent)}");
        }
    }
}
