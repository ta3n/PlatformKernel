using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Liberty.Reservation.Application.Models;
using Liberty.Reservation.Booking.Worker.Application.Options;
using Liberty.SysIntegrationEvent;
using Liberty.SysIntegrationEvent.Events;
using MassTransit;
using Microsoft.Extensions.Options;

namespace Liberty.Reservation.Booking.Worker.Consumers;

public class BookingAggregateAuditLogConsumer(
    ILogger<BookingAggregateAuditLogConsumer> logger,
    IBus bus,
    IOptions<AggregateAuditLogOption> aggregateAuditLogOption
) : IConsumer<BookingAggregateAuditLogEvent>
{
    public async Task Consume(
        ConsumeContext<BookingAggregateAuditLogEvent> context
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

            var partitionCount = aggregateAuditLogOption.Value.PartitionCount;

            var shardIndex = GetQueueShardIndex(
                model.AggregateId,
                partitionCount
            );

            var partitionEvent = new BookingAuditLogPartitionEvent();
            partitionEvent.SerializeJsonData(model);

            var queueName = $"{ReservationQueues.BookingAuditLogPartitionQueue}{shardIndex}";
            var endpoint = await bus.GetSendEndpoint(new Uri($"queue:{queueName}"));

            await endpoint.Send(partitionEvent, context.CancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"Failed to process {nameof(BookingAggregateAuditLogEvent)}");
        }
    }

    private static int GetQueueShardIndex(
        long bookingId,
        int partitionCount
    )
    {
        var composite = $"{bookingId}";
        var hash = SHA256.HashData(Encoding.UTF8.GetBytes(composite));

        var hashCode = 0;
        foreach (var t in hash)
        {
            hashCode = (hashCode * 31) ^ t;
        }

        hashCode &= 0x7FFFFFFF;

        var shardIndex = hashCode % partitionCount;

        return shardIndex;
    }
}
