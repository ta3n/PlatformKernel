using Liberty.SysIntegrationEvent.Events;
using MassTransit;
using Newtonsoft.Json;
using Liberty.Cache.Services;
using Polly;
using Liberty.Reservation.Application.Models.Responses;
using Liberty.Reservation.Application.Domains.Services.Interfaces;
using Liberty.Reservation.Application.Models;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Manager.Application.Domains.Services.Interfaces;
using static Liberty.Reservation.Application.Models.Responses.SetRoomsResponse;
using Liberty.Reservation.Application.Constants;

namespace Liberty.Reservation.Booking.Worker.Consumers;

public class RoomGroupAppDateUpsertConsumer(
    ILogger<RoomGroupAppDateUpsertConsumer> logger,
    IBookingRoomAppDateService bookingRoomAppDateService,
    IRoomAdjustmentStatusService roomAdjustmentStatusService,
    IAdjustmentResultService adjustmentResultService,
    ICacheService cacheService,
    IAppDateService appDateService
) : IConsumer<RoomGroupAppDateBulkUpsertEvent>
{
    private const int MaxRetryAttempts = 3;
    private const int RetryDelayMilliseconds = 500;

    public async Task Consume(
        ConsumeContext<RoomGroupAppDateBulkUpsertEvent> context
    )
    {
        var jsonContext = context.Message.JsonData;

        if (string.IsNullOrWhiteSpace(jsonContext))
        {
            logger.LogWarning("Upsert skipped: JsonData is null or empty");
            return;
        }

        RoomGroupAppDateUpsertModel? model = null;

        try
        {
            model = JsonConvert.DeserializeObject<RoomGroupAppDateUpsertModel>(jsonContext);

            if (model is null || string.IsNullOrWhiteSpace(model.CacheKey))
            {
                logger.LogWarning("Upsert skipped: Deserialized model or CacheKey is null or empty");
                return;
            }

            var redisKey = model.CacheKey;
            var userCode = model.UserCode;
            var roomAdjustmentCode = model.RoomAdjustmentCode;

            var retryPolicy = Policy
                .Handle<Exception>()
                .WaitAndRetryAsync(
                    MaxRetryAttempts,
                    attempt => TimeSpan.FromMilliseconds(RetryDelayMilliseconds * attempt),
                    (
                        ex,
                        time,
                        retryCount,
                        _
                    ) =>
                    {
                        logger.LogWarning(ex, "Retry {RetryCount} for key: {RedisKey} after {Time}", retryCount, redisKey, time);
                    }
                );

            await retryPolicy.ExecuteAsync(
                async () =>
                {
                    var hotelModels = await cacheService.GetAsync<List<HotelModel>>(redisKey!, context.CancellationToken);

                    if (hotelModels is not { Count: > 0 })
                    {
                        logger.LogWarning("No HotelModel found in Redis for key: {RedisKey}", redisKey);
                        return;
                    }

                    var allDates = hotelModels
                        .SelectMany(h => h.GetDates())
                        .Distinct()
                        .ToList();

                    var notCreatedAppDateIds = await appDateService.FindNotCreatedAppDatesAsync(
                        [.. allDates],
                        context.CancellationToken
                    );

                    if (notCreatedAppDateIds is { Count: > 0 })
                    {
                        await bookingRoomAppDateService.BulkUpsertAppDateAsync(
                            notCreatedAppDateIds
                        );
                    }

                    var setRoomModel = new SetRoomModel { Hotels = [.. hotelModels.Select(h => new SetRoomModel.Hotel(h, h.GetDates()))] };

                    var (updatedModel, updateData, addData) =
                        await bookingRoomAppDateService.UpdateSetRoomsAsync(setRoomModel, context.CancellationToken);

                    var roomAdjustmentStatus = await roomAdjustmentStatusService
                        .GetRoomAdjustmentStatusByCodeAsync(roomAdjustmentCode, context.CancellationToken);

                    if (updatedModel.Hotels is not { Count: > 0 })
                    {
                        logger.LogWarning("No hotels found in updatedModel");
                        return;
                    }

                    var hotel = updatedModel.Hotels[0];

                    if (!updatedModel.CanUpdate)
                    {
                        await AdjustmentMessageAsync(hotel, roomAdjustmentStatus.Id, false, hotel.FailureReason, context.CancellationToken);

                        await cacheService.RemoveAsync(redisKey);

                        return;
                    }

                    await bookingRoomAppDateService.BulkUpsertRoomGroupAppDateAsync(
                        [.. updateData, .. addData]
                    );

                    logger.LogInformation(
                        "Successfully upserted RoomGroupAppDate: Updated={UpdateCount}, Added={AddCount}",
                        updateData.Count,
                        addData.Count
                    );

                    await AdjustmentMessageAsync(hotel, roomAdjustmentStatus.Id, true, null, context.CancellationToken);

                    await cacheService.RemoveAsync(redisKey);

                    var facilityCacheKeys = hotelModels
                        .Select(x => x.HotelId)
                        .Distinct()
                        .Select(x => $"*{string.Format(CacheKeys.FacilityPrefixKey, x)}*")
                        .ToArray();

                    await cacheService.RemoveByPatternsAsync(
                        true,
                        [
                            .. facilityCacheKeys,
                            $"*{string.Format(CacheKeys.KakusanGetAllBookingQueryPrefixKey, userCode, string.Empty)}*",
                            $"*{string.Format(CacheKeys.KakusanGetAllRoomQueryPrefixKey, userCode, string.Empty)}*",
                            $"*{string.Format(CacheKeys.KakusanGetAllRoomTypeQueryPrefixKey, userCode, string.Empty)}*"
                        ]
                    );
                }
            );
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to process RoomGroupAppDateBulkUpsertEvent for key: {RedisKey}", model?.CacheKey);
        }
    }

    private async Task AdjustmentMessageAsync(
        SetRoomModel.Hotel hotel,
        long roomAdjustmentStatusId,
        bool isSuccess,
        FailureReason? reason,
        CancellationToken cancellationToken = default
    )
    {
        var message = new AdjustmentResult
        {
            HotelId = hotel.HotelId!,
            RoomId = hotel.RoomId!,
            RoomAdjustmentStatusId = roomAdjustmentStatusId,
            IsSuccess = isSuccess,
            Reason = reason
        };

        await adjustmentResultService.CreateAsync(message, true, cancellationToken);
    }
}
