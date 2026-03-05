using Liberty.Cache.Options;
using Liberty.Cache.Services;
using Liberty.Cache.Utils;
using Liberty.Reservation.Application.Constants;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Application.Domains.Services.Interfaces;
using Liberty.Reservation.Application.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace Liberty.Reservation.Application.Domains.Services;

public class BookingHoldManagementService(
    ILogger<BookingHoldManagementService> logger,
    IOptions<CacheOptions> cacheOptions,
    ICacheService cacheService,
    IBookingInventoryService bookingInventoryService
) : IBookingHoldManagementService
{
    private const string HoldRoomLuaScript = "BookingHoldRoom";
    private const string ReleaseHoldLuaScript = "BookingReleaseHold";
    private const string SumHoldLuaScript = "BookingSumHold";

    private IDatabase Redis => cacheService.GetDatabase();

    public async Task<bool> TryHoldRoomAsync(
        BookingHoldCheckModel model,
        CancellationToken cancellationToken = default
    )
    {
        var maxPerDay = await GetAvailabilityForUserAsync(
            model,
            cancellationToken
        );
        if (maxPerDay is not { Count: > 0 } || maxPerDay.Values.Min() < model.NumberOfRooms)
        {
            return false;
        }

        var dateList = maxPerDay.ToList();

        var keys = new List<RedisKey>();

        keys.AddRange(
            dateList.SelectMany(
                date => new[]
                {
                    (RedisKey)GetBookingHoldKey(model, date.Key), // global key
                    (RedisKey)GetUserHoldKey(model, date.Key) // user key
                }
            )
        );

        var args = new List<RedisValue>
        {
            model.NumberOfRooms,
            model.HoldTimeInSeconds,
            dateList.Count
        };
        args.AddRange(dateList.Select(d => (RedisValue)d.Value));

        var result = (int)await Redis.ScriptEvaluateAsync(
            LuaScriptLoader.Load(HoldRoomLuaScript),
            [..keys],
            [.. args]
        );
        return result == 1;
    }

    public async Task ReleaseHoldAsync(
        BookingHoldCheckModel model,
        CancellationToken cancellationToken = default
    )
    {
        var dateList = Enumerable.Range(0, model.NumberOfNights)
            .Select(x => AppDate.GetId(model.CheckInDateTime.AddDays(x)))
            .ToList();

        var keys = new List<RedisKey>();
        keys.AddRange(
            dateList.SelectMany(
                date => new[]
                {
                    (RedisKey)GetBookingHoldKey(model, date), // global key
                    (RedisKey)GetUserHoldKey(model, date) // user key
                }
            )
        );

        var args = new RedisValue[] { model.NumberOfRooms, dateList.Count };

        // Use Lua script that handles missing keys gracefully
        await Redis.ScriptEvaluateAsync(
            LuaScriptLoader.Load(ReleaseHoldLuaScript),
            [.. keys],
            args
        );

        logger.LogInformation(
            "{Action} - Booking hold released for {Facility} {Site} {Plan} {RoomGroup} {CheckIn} {UserCode} {BookingTempCode}",
            nameof(BookingHoldManagementService),
            model.FacilityId,
            model.SiteId,
            model.PlanId,
            model.RoomId,
            model.CheckInDate,
            model.UserCode,
            model.BookingTempCode
        );
    }

    public async Task<Dictionary<long, int>> GetAvailabilityForUserAsync(
        BookingHoldCheckModel model,
        CancellationToken cancellationToken = default
    )
    {
        var availability = new Dictionary<long, int>();

        var roomInventory = (await bookingInventoryService.GetRoomInventoryAsync(
            model,
            cancellationToken
        )).ToList();

        var dateIndex = 0;
        while (dateIndex < model.NumberOfNights)
        {
            var date = AppDate.GetId(
                model.CheckInDateTime.AddDays(dateIndex)
            );

            var roomInventoryForDate = roomInventory.Find(
                x => x.AppDateId == date
            );

            var max = roomInventoryForDate?.MaxQuantity ?? 0;
            var reserved = roomInventoryForDate?.ReservedNumber ?? 0;
            var held = await GetHeldQuantityForDateAsync(model, date);

            availability[date] = max - reserved - held;

            dateIndex++;
        }

        return availability;
    }

    public async Task<Dictionary<long, int>> CheckAvailabilityForGuestConfirmAsync(
        BookingHoldCheckModel model,
        CancellationToken cancellationToken = default
    )
    {
        var availability = new Dictionary<long, int>();

        var roomInventory = (await bookingInventoryService.GetRoomInventoryAsync(
            model,
            cancellationToken
        )).ToList();

        var dateIndex = 0;
        while (dateIndex < model.NumberOfNights)
        {
            var date = AppDate.GetId(
                model.CheckInDateTime.AddDays(dateIndex)
            );

            var roomInventoryForDate = roomInventory.Find(
                x => x.AppDateId == date
            );

            var max = roomInventoryForDate?.MaxQuantity ?? 0;
            var reserved = roomInventoryForDate?.ReservedNumber ?? 0;

            availability[date] = max - reserved;

            dateIndex++;
        }

        return availability;
    }

    private string GetBookingHoldKey(
        BookingHoldCheckModel holdCheckModel,
        long date
    )
    {
        return cacheOptions.Value.InstanceName
            + string.Format(
                CacheKeys.BookingHoldPrefixKey,
                holdCheckModel.FacilityId,
                holdCheckModel.SiteId,
                holdCheckModel.PlanId,
                holdCheckModel.RoomId,
                date
            );
    }

    private string GetUserHoldKey(
        BookingHoldCheckModel holdCheckModel,
        long date
    )
    {
        var userKey = "Unknown";

        if (!string.IsNullOrEmpty(holdCheckModel.UserCode))
        {
            userKey = holdCheckModel.UserCode.Replace("-", string.Empty);
        }

        return cacheOptions.Value.InstanceName
            + string.Format(
                CacheKeys.BookingHoldWithUserPrefixKey,
                holdCheckModel.FacilityId,
                holdCheckModel.SiteId,
                holdCheckModel.PlanId,
                holdCheckModel.RoomId,
                date,
                userKey.ToUpper(),
                holdCheckModel.BookingTempCode
            );
    }

    private async Task<int> GetHeldQuantityForDateAsync(
        BookingHoldCheckModel model,
        long date
    )
    {
        var pattern = GetBookingHoldKey(model, date) + ":*";

        var result = await Redis.ScriptEvaluateAsync(
            LuaScriptLoader.Load(SumHoldLuaScript),
            [],
            [pattern]
        );

        return (int)result;
    }
}
