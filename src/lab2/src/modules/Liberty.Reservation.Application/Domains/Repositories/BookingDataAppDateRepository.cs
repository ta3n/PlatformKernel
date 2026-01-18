using Liberty.Cache.Services;
using Liberty.Reservation.Application.Constants;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;
using Liberty.Reservation.Application.Domains.Repositories.Interfaces;
using Liberty.Reservation.Application.Models;
using Liberty.Reservation.Application.Models.Requests;
using Microsoft.EntityFrameworkCore;

namespace Liberty.Reservation.Application.Domains.Repositories;

public class BookingDataAppDateRepository(
    IDbContextFactory<DbContext> dbContextFactory,
    ICacheService cacheService
) : IBookingDataAppDateRepository
{
    public async Task<IEnumerable<BookingMetaRoomAppDateModel>> GetAllBookingMetaRoomAppDateModelsAsync(
        long facilityId,
        long siteId,
        long[] roomGroupIds,
        BookingSearchPlanRequest search,
        CancellationToken cancellationToken
    )
    {
        if (roomGroupIds is not { Length: > 0 })
        {
            return [];
        }

        var appDates = search.GetAppDates();

        var cacheKeys = (
            from roomGroupId in roomGroupIds
            from appDateId in appDates
            let cacheKey = string.Format(
                CacheKeys.BookingSearchRoomAppDatePrefixKey,
                facilityId,
                siteId,
                roomGroupId,
                appDateId
            )
            let cachePattern = string.Format(
                CacheKeys.BookingSearchRoomAppDatePrefixKey,
                facilityId,
                siteId,
                "*",
                appDateId
            )
            select (roomGroupId, appDateId, cacheKey, cachePattern)).ToList();

        var cachedData = new List<BookingMetaRoomAppDateModel>();
        if (search.UseCache)
        {
            cachedData =
            [
                .. cacheService.GetByPatterns<BookingMetaRoomAppDateModel>(
                        [.. cacheKeys.Select(x => x.cachePattern).Distinct()]
                    )
                    .Where(
                        x => cacheKeys.Exists(
                            key => key.roomGroupId == x.RoomGroupId
                        )
                    )
            ];
        }

        var missingKeys = cacheKeys
            .Where(
                x => !cachedData.Exists(
                    c => c.RoomGroupId == x.roomGroupId && x.appDateId == c.AppDateId
                )
            )
            .ToList();
        if (missingKeys is not { Count: > 0 })
        {
            return cachedData;
        }

        var missingRoomGroupIds = missingKeys
            .Select(x => x.roomGroupId)
            .Distinct()
            .ToArray();

        var missingAppDates = missingKeys
            .Select(x => x.appDateId)
            .Distinct()
            .ToArray();

        await using var dbContext = await dbContextFactory.CreateDbContextAsync(cancellationToken);

        var queryable = dbContext.Set<RoomGroupAppDate>()
            .AsNoTracking()
            .Where(roomAppDate => missingRoomGroupIds.Contains(roomAppDate.RoomGroupId))
            // .Where(roomAppDate => roomAppDate.AppDateId >= search.CheckInDate)
            // .Where(roomAppDate => roomAppDate.AppDateId <= search.CheckOutDate)
            .Where(roomAppDate => missingAppDates.Contains(roomAppDate.AppDateId))
            .OrderByDescending(roomAppDate => roomAppDate.AppDateId)
            .ThenBy(roomAppDate => roomAppDate.RoomGroupId)
            .Select(
                roomAppDate => new BookingMetaRoomAppDateModel(
                    roomAppDate.RoomGroupId,
                    roomAppDate.AppDateId,
                    roomAppDate.IsNotSelled,
                    roomAppDate.RoomGroup!.CapacityMax,
                    roomAppDate.SellNumber,
                    roomAppDate.RemainNumber,
                    roomAppDate.ReservedNumber
                )
            )
            .AsSingleQuery();

        var roomAppDates = await queryable.ToListAsync(cancellationToken);

        var mergedData = roomAppDates.Union(cachedData).ToList();

        var bulkCacheData = new Dictionary<string, List<BookingMetaRoomAppDateModel>>();

        var dataLookup = mergedData
            .GroupBy(
                x => new
                {
                    x.RoomGroupId,
                    x.AppDateId
                }
            )
            .ToDictionary(g => g.Key, g => g.ToList());

        foreach (var (roomId, appDateId, cacheKey, _) in cacheKeys)
        {
            var key = new
            {
                RoomGroupId = roomId,
                AppDateId = appDateId
            };

            if (dataLookup.TryGetValue(key, out var value) && value.Count > 0)
            {
                bulkCacheData[cacheKey] = value;
            }
        }

        if (bulkCacheData is { Count: > 0 })
        {
            await cacheService.SetBulkAsync(bulkCacheData, cancellationToken);
        }

        return mergedData;
    }

    public async Task<IEnumerable<BookingMetaPlanAppDateModel>> GetAllBookingMetaPlanAppDateModelsAsync(
        long facilityId,
        long siteId,
        long[] planIds,
        long[] roomGroupIds,
        BookingSearchPlanRequest search,
        CancellationToken cancellationToken
    )
    {
        if (roomGroupIds is not { Length: > 0 })
        {
            return [];
        }

        var appDates = search.GetAppDates();

        var cacheKeys = (
            from planId in planIds
            from roomGroupId in roomGroupIds
            from appDateId in appDates
            let cacheKey = string.Format(
                CacheKeys.BookingSearchPlanAppDatePrefixKey,
                facilityId,
                siteId,
                planId,
                roomGroupId,
                appDateId
            )
            let cachePattern = string.Format(
                CacheKeys.BookingSearchPlanAppDatePrefixKey,
                facilityId,
                siteId,
                "*",
                "*",
                appDateId
            )
            select (planId, roomGroupId, appDateId, cacheKey, cachePattern)).ToList();

        var cachedData = new List<BookingMetaPlanAppDateModel>();
        if (search.UseCache)
        {
            cachedData =
            [
                .. cacheService.GetByPatterns<BookingMetaPlanAppDateModel>(
                        [.. cacheKeys.Select(x => x.cachePattern).Distinct()]
                    )
                    .Where(
                        x => cacheKeys.Exists(
                            key => key.planId == x.PlanId && key.roomGroupId == x.RoomGroupId
                        )
                    )
            ];
        }

        var missingKeys = cacheKeys
            .Where(
                x => !cachedData.Exists(
                    c => c.PlanId == x.planId && c.RoomGroupId == x.roomGroupId && x.appDateId == c.AppDateId
                )
            )
            .ToList();

        if (missingKeys is not { Count: > 0 })
        {
            return cachedData;
        }

        var missingPlanIds = missingKeys
            .Select(x => x.planId)
            .Distinct()
            .ToArray();

        var missingRoomGroupIds = missingKeys
            .Select(x => x.roomGroupId)
            .Distinct()
            .ToArray();

        var missingAppDates = missingKeys
            .Select(x => x.appDateId)
            .Distinct()
            .ToArray();

        await using var dbContext = await dbContextFactory.CreateDbContextAsync(cancellationToken);

        var queryable = dbContext.Set<PlanRoomGroupSiteAppDate>()
            .AsNoTracking()
            .Where(planRoomGroupSiteAppDate => planRoomGroupSiteAppDate.SiteId == siteId)
            .Where(planRoomGroupSiteAppDate => missingPlanIds.Contains(planRoomGroupSiteAppDate.PlanId))
            .Where(planRoomGroupSiteAppDate => missingRoomGroupIds.Contains(planRoomGroupSiteAppDate.RoomGroupId))
            // .Where(planRoomGroupSiteAppDate => planRoomGroupSiteAppDate.DateCalendar >= search.CheckInDate)
            // .Where(planRoomGroupSiteAppDate => planRoomGroupSiteAppDate.DateCalendar <= search.CheckOutDate)
            .Where(planRoomGroupSiteAppDate => missingAppDates.Contains(planRoomGroupSiteAppDate.DateCalendar))
            .OrderByDescending(planRoomGroupSiteAppDate => planRoomGroupSiteAppDate.DateCalendar)
            .ThenBy(planRoomGroupSiteAppDate => planRoomGroupSiteAppDate.RoomGroupId)
            .Select(
                roomAppDate => new BookingMetaPlanAppDateModel(
                    roomAppDate.PlanId,
                    roomAppDate.RoomGroupId,
                    roomAppDate.SiteId,
                    roomAppDate.DateCalendar,
                    roomAppDate.UseAutoDiscount,
                    roomAppDate.PointRate
                )
            )
            .AsSingleQuery();

        var planAppDates = await queryable.ToListAsync(cancellationToken);

        var mergedData = planAppDates.Union(cachedData).ToList();

        var bulkCacheData = new Dictionary<string, List<BookingMetaPlanAppDateModel>>();

        var dataLookup = mergedData
            .GroupBy(
                x => new
                {
                    x.PlanId,
                    x.RoomGroupId,
                    x.AppDateId
                }
            )
            .ToDictionary(g => g.Key, g => g.ToList());

        foreach (var (planId, roomId, appDateId, cacheKey, _) in cacheKeys)
        {
            var key = new
            {
                PlanId = planId,
                RoomGroupId = roomId,
                AppDateId = appDateId
            };

            if (dataLookup.TryGetValue(key, out var value) && value.Count > 0)
            {
                bulkCacheData[cacheKey] = value;
            }
        }

        if (bulkCacheData is { Count: > 0 })
        {
            await cacheService.SetBulkAsync(bulkCacheData, cancellationToken);
        }

        return mergedData;
    }
}
