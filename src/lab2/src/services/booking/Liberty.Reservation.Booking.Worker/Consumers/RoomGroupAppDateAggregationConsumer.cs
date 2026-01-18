using System.Security.Cryptography;
using System.Text;
using Liberty.SysIntegrationEvent.Events;
using MassTransit;
using System.Text.Json;
using Liberty.Cache.Services;
using Liberty.Reservation.Application.Models;
using Liberty.Cache.Utils;
using Liberty.Reservation.Application.Constants;
using Liberty.MassTransit.Options;
using Liberty.SysIntegrationEvent;
using Microsoft.Extensions.Options;
using Liberty.Reservation.Booking.Worker.Application.Options;

namespace Liberty.Reservation.Booking.Worker.Consumers;

public class RoomGroupAppDateAggregationConsumer(
    ILogger<RoomGroupAppDateAggregationConsumer> logger,
    ICacheService cacheService,
    IBus bus,
    IOptions<RoomGroupAppDateOptions> setRoomOptions,
    IConfiguration configuration
) : IConsumer<RoomGroupAppDateAggregationEvent>
{
    public async Task Consume(
        ConsumeContext<RoomGroupAppDateAggregationEvent> context
    )
    {
        var jsonContext = context.Message.JsonData;

        if (string.IsNullOrWhiteSpace(jsonContext))
        {
            logger.LogWarning("Aggregation skipped: JsonData is null or empty");
            return;
        }

        RoomGroupAppDateAggregationModel? model;

        try
        {
            model = JsonSerializer.Deserialize<RoomGroupAppDateAggregationModel>(jsonContext);

            if (model is null)
            {
                logger.LogWarning("Aggregation skipped: Model invalid");
                return;
            }

            var groupedByRoom = model.Hotels
                .GroupBy(h => (h.HotelId, h.RoomId))
                .ToDictionary(g => g.Key, g => g.ToList());

            var roomHotelDict = groupedByRoom
                .Where(x => !string.IsNullOrWhiteSpace(x.Key.RoomId) && !string.IsNullOrWhiteSpace(x.Key.HotelId))
                .ToDictionary(x => x.Key.RoomId!, x => x.Key.HotelId!);

            var keyValuePairs = groupedByRoom
                .Where(x => roomHotelDict.ContainsKey(x.Key.RoomId!))
                .ToDictionary(
                    group =>
                    {
                        var roomId = group.Key.RoomId!;
                        var hotelId = group.Key.HotelId!;

                        return CacheHelper.GetCacheKeyByParameters(
                            string.Format(CacheKeys.BulkRoomGroupAppDate, roomId, hotelId, model.UserCode, model.RoomAdjustmentCode)
                        );
                    },
                    group => group.Value
                );

            await cacheService.SetBulkAsync(keyValuePairs, 0, context.CancellationToken);

            var partitionCount = setRoomOptions.Value.PartitionCount;
            var section = configuration.GetSection("MessageQueueSettings");
            var (_, rabbitMqOptions) = section.Get<MessageQueueOptions>()!;

            foreach (var (hotelId, roomId) in groupedByRoom.Select(group => group.Key))
            {
                if (string.IsNullOrWhiteSpace(hotelId) || string.IsNullOrWhiteSpace(roomId))
                {
                    continue;
                }

                var cacheKey = CacheHelper.GetCacheKeyByParameters(
                    string.Format(CacheKeys.BulkRoomGroupAppDate, roomId, hotelId, model.UserCode, model.RoomAdjustmentCode)
                );

                var shardIndex = GetQueueShardIndex(
                    hotelId,
                    roomId,
                    partitionCount
                );

                var upsertModel = new RoomGroupAppDateUpsertModel
                {
                    CacheKey = cacheKey,
                    RoomAdjustmentCode = model.RoomAdjustmentCode,
                    UserCode = model.UserCode
                };

                var upsertEvent = new RoomGroupAppDateBulkUpsertEvent();
                upsertEvent.SerializeJsonData(upsertModel);

                var endpoint = await bus.GetSendEndpoint(
                    new Uri($"{rabbitMqOptions!.Url}/{ReservationQueues.RoomGroupAppDateUpsertQueue}{shardIndex}")
                );

                await endpoint.Send(upsertEvent, context.CancellationToken);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to process RoomGroupAppDateAggregationEvent");
        }
    }

    private static int GetQueueShardIndex(
        string hotelId,
        string roomId,
        int partitionCount
    )
    {
        var composite = $"{hotelId}:{roomId}";
        var hash = SHA256.HashData(Encoding.UTF8.GetBytes(composite));

        var hashCode = 0;
        for (var i = 0; i < hash.Length; i++)
        {
            hashCode = (hashCode * 31) ^ i;
        }

        hashCode &= 0x7FFFFFFF;

        var shardIndex = hashCode % partitionCount;

        return shardIndex;
    }
}
