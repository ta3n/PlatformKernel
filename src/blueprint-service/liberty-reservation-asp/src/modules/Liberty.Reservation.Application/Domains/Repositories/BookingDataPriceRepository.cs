using Liberty.Cache.Services;
using Liberty.Reservation.Application.Constants;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;
using Liberty.Reservation.Application.Domains.Repositories.Interfaces;
using Liberty.Reservation.Application.Models;
using Liberty.Reservation.Application.Models.Requests;
using Microsoft.EntityFrameworkCore;

namespace Liberty.Reservation.Application.Domains.Repositories;

public class BookingDataPriceRepository(
    IDbContextFactory<DbContext> dbContextFactory,
    ICacheService cacheService
) : IBookingDataPriceRepository
{
    public async Task<IEnumerable<BookingMetaStandardPriceModel>> GetAllStandardPriceModelsAsync(
        long facilityId,
        long siteId,
        long[] planIds,
        long[] roomGroupIds,
        bool useCache,
        CancellationToken cancellationToken
    )
    {
        var cacheKeys = (
            from planId in planIds
            from roomGroupId in roomGroupIds
            let cacheKey = string.Format(
                CacheKeys.BookingSearchStandardPricePrefixKey,
                facilityId,
                siteId,
                planId,
                roomGroupId
            )
            let cachePattern = string.Format(
                CacheKeys.BookingSearchStandardPricePrefixKey,
                facilityId,
                siteId,
                "*",
                "*"
            )
            select (planId, roomGroupId, cacheKey, cachePattern)).ToList();

        var cachedData = new List<BookingMetaStandardPriceModel>();
        if (useCache)
        {
            cachedData =
            [
                .. cacheService.GetByPatterns<BookingMetaStandardPriceModel>(
                        [.. cacheKeys.Select(x => x.cachePattern).Distinct()]
                    )
                    .Where(
                        x => cacheKeys.Exists(
                            key => key.planId == x.PlanId
                                && key.roomGroupId == x.RoomGroupId
                        )
                    )
            ];
        }

        var missingKeys = cacheKeys
            .Where(
                x => !cachedData.Exists(
                    c => c.PlanId == x.planId && c.RoomGroupId == x.roomGroupId
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

        await using var dbContext = await dbContextFactory.CreateDbContextAsync(cancellationToken);

        var queryable = dbContext.Set<PlanRoomGroupSiteAppDateTypePriceData>()
            .AsNoTracking()
            .Where(
                planRoomGroupSiteAppDateTypePriceData =>
                    planRoomGroupSiteAppDateTypePriceData.SiteId == siteId
            )
            .Where(
                planRoomGroupSiteAppDateTypePriceData =>
                    missingPlanIds.Contains(planRoomGroupSiteAppDateTypePriceData.PlanId)
            )
            .Where(
                planRoomGroupSiteAppDateTypePriceData =>
                    missingRoomGroupIds.Contains(planRoomGroupSiteAppDateTypePriceData.RoomGroupId)
            )
            .Where(
                planRoomGroupSiteAppDateTypePriceData => planRoomGroupSiteAppDateTypePriceData.AppDateType != null
                    && !planRoomGroupSiteAppDateTypePriceData.AppDateType.IsDeleted
            )
            .Where(x => x.IsEnabled)
            .OrderBy(
                planRoomGroupSiteAppDateTypePriceData => planRoomGroupSiteAppDateTypePriceData.AppDateTypeId
            )
            .Select(
                planRoomGroupSiteAppDateTypePriceData => new BookingMetaStandardPriceModel(
                    planRoomGroupSiteAppDateTypePriceData.PlanId,
                    planRoomGroupSiteAppDateTypePriceData.RoomGroupId,
                    planRoomGroupSiteAppDateTypePriceData.AppDateTypeId,
                    planRoomGroupSiteAppDateTypePriceData.PriceData!.PersonMin,
                    planRoomGroupSiteAppDateTypePriceData.PriceData!.PersonMax,
                    planRoomGroupSiteAppDateTypePriceData.PriceData!.Price
                )
            )
            .AsSingleQuery();

        var standardPriceData = await queryable.ToListAsync(cancellationToken);

        var mergedData = standardPriceData.Union(cachedData).ToList();

        var bulkCacheData = new Dictionary<string, List<BookingMetaStandardPriceModel>>();

        var dataLookup = mergedData
            .GroupBy(
                x => new
                {
                    x.PlanId,
                    x.RoomGroupId
                }
            )
            .ToDictionary(g => g.Key, g => g.ToList());

        foreach (var (planId, roomId, cacheKey, _) in cacheKeys)
        {
            var key = new
            {
                PlanId = planId,
                RoomGroupId = roomId
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

    public async Task<IEnumerable<BookingMetaPriceDataModel>> GetAllBookingMetaPriceDataModelsAsync(
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
                CacheKeys.BookingSearchPriceDataPrefixKey,
                facilityId,
                siteId,
                planId,
                roomGroupId,
                appDateId
            )
            let cachePattern = string.Format(
                CacheKeys.BookingSearchPriceDataPrefixKey,
                facilityId,
                siteId,
                "*",
                "*",
                appDateId
            )
            select (planId, roomGroupId, appDateId, cacheKey, cachePattern)).ToList();
        var cachedData = new List<BookingMetaPriceDataModel>();
        if (search.UseCache)
        {
            cachedData =
            [
                .. cacheService.GetByPatterns<BookingMetaPriceDataModel>(
                        [.. cacheKeys.Select(x => x.cachePattern).Distinct()]
                    )
                    .Where(
                        x => cacheKeys.Exists(
                            key => key.planId == x.PlanId
                                && key.roomGroupId == x.RoomGroupId
                        )
                    )
            ];
        }

        var missingKeys = cacheKeys
            .Where(
                x => !cachedData.Exists(
                    c => c.PlanId == x.planId
                        && c.RoomGroupId == x.roomGroupId
                        && x.appDateId == c.AppDateId
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

        var standardPriceData = await GetAllStandardPriceModelsAsync(
            facilityId,
            siteId,
            planIds,
            roomGroupIds,
            true,
            cancellationToken
        );

        await using var dbContext = await dbContextFactory.CreateDbContextAsync(cancellationToken);

        var queryable = dbContext.Set<PlanRoomGroupSiteAppDatePriceData>()
            .AsNoTracking()
            .Where(
                planRoomGroupSiteAppDatePriceData
                    => planRoomGroupSiteAppDatePriceData.SiteId == siteId
            )
            .Where(
                planRoomGroupSiteAppDatePriceData =>
                    missingPlanIds.Contains(planRoomGroupSiteAppDatePriceData.PlanId)
            )
            .Where(
                planRoomGroupSiteAppDatePriceData =>
                    missingRoomGroupIds.Contains(planRoomGroupSiteAppDatePriceData.RoomGroupId)
            )
            .Where(
                planRoomGroupSiteAppDatePriceData =>
                    planRoomGroupSiteAppDatePriceData.IsEnabled
            )
            .Where(
                planRoomGroupSiteAppDatePriceData => missingAppDates.Contains(planRoomGroupSiteAppDatePriceData.DateCalendar)
            )
            .Where(
                planRoomGroupSiteAppDatePriceData => planRoomGroupSiteAppDatePriceData.PriceData != null
                    && planRoomGroupSiteAppDatePriceData.PriceData.Price != null
                    && !planRoomGroupSiteAppDatePriceData.PriceData.IsDeleted
                    && planRoomGroupSiteAppDatePriceData.PriceData.IsEnabled
            )
            .Select(
                planRoomGroupSiteAppDatePriceData => new BookingMetaPriceDataModel(
                    planRoomGroupSiteAppDatePriceData.PlanId,
                    planRoomGroupSiteAppDatePriceData.RoomGroupId,
                    planRoomGroupSiteAppDatePriceData.SiteId,
                    planRoomGroupSiteAppDatePriceData.DateCalendar,
                    planRoomGroupSiteAppDatePriceData.PriceDataId,
                    planRoomGroupSiteAppDatePriceData.PriceData!.Price,
                    planRoomGroupSiteAppDatePriceData.PriceData!.PersonMin,
                    planRoomGroupSiteAppDatePriceData.PriceData!.PersonMax
                )
            )
            .AsSingleQuery();

        var priceData = await queryable.ToListAsync(cancellationToken);

        var standardPriceLookup = new HashSet<(int?, int?)>(
            standardPriceData.Select(t => (t.PersonMin, t.PersonMax))
        );

        var filteredData = priceData
            .Where(
                x => standardPriceLookup.Contains((x.PersonMin, x.PersonMax))
            )
            .ToList();

        var mergedData = filteredData.Union(cachedData).ToList();

        var bulkCacheData = new Dictionary<string, List<BookingMetaPriceDataModel>>();

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

    public async Task<IEnumerable<BookingMetaDiscountDataModel>> GetAllBookingMetaDiscountDataModelsAsync(
        long facilityId,
        long siteId,
        long[] planIds,
        long[] roomGroupIds,
        bool useCache,
        CancellationToken cancellationToken
    )
    {
        if (roomGroupIds is not { Length: > 0 })
        {
            return [];
        }

        var cacheKeys = (
            from planId in planIds
            from roomGroupId in roomGroupIds
            let cacheKey = string.Format(
                CacheKeys.BookingSearchDiscountDataPrefixKey,
                facilityId,
                siteId,
                planId,
                roomGroupId
            )
            let cachePattern = string.Format(
                CacheKeys.BookingSearchDiscountDataPrefixKey,
                facilityId,
                siteId,
                "*",
                "*"
            )
            select (planId, roomGroupId, cacheKey, cachePattern)).ToList();

        var cachedData = new List<BookingMetaDiscountDataModel>();
        if (useCache)
        {
            cachedData =
            [
                .. cacheService.GetByPatterns<BookingMetaDiscountDataModel>(
                        [.. cacheKeys.Select(x => x.cachePattern).Distinct()]
                    )
                    .Where(
                        x => cacheKeys.Exists(
                            key => key.planId == x.PlanId
                                && key.roomGroupId == x.RoomGroupId
                        )
                    )
            ];
        }

        var missingKeys = cacheKeys
            .Where(
                x => !cachedData.Exists(
                    c => c.PlanId == x.planId && c.RoomGroupId == x.roomGroupId
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

        var standardPriceData = await GetAllStandardPriceModelsAsync(
            facilityId,
            siteId,
            planIds,
            roomGroupIds,
            true,
            cancellationToken
        );

        await using var dbContext = await dbContextFactory.CreateDbContextAsync(cancellationToken);

        var queryable = dbContext.Set<PlanRoomGroupSiteDiscountData>()
            .AsNoTracking()
            .Where(
                planRoomGroupSiteDiscountData
                    => planRoomGroupSiteDiscountData.SiteId == siteId
            )
            .Where(
                planRoomGroupSiteDiscountData
                    => missingPlanIds.Contains(planRoomGroupSiteDiscountData.PlanId)
            )
            .Where(
                planRoomGroupSiteDiscountData
                    => missingRoomGroupIds.Contains(planRoomGroupSiteDiscountData.RoomGroupId)
            )
            .Where(
                planRoomGroupSiteDiscountData
                    => planRoomGroupSiteDiscountData.DiscountData != null
                    && planRoomGroupSiteDiscountData.DiscountData.Value != null
            )
            .Select(
                planRoomGroupSiteDiscountData => new BookingMetaDiscountDataModel(
                    planRoomGroupSiteDiscountData.PlanId,
                    planRoomGroupSiteDiscountData.RoomGroupId,
                    planRoomGroupSiteDiscountData.SiteId,
                    planRoomGroupSiteDiscountData.DiscountData!.Id,
                    planRoomGroupSiteDiscountData.DiscountData!.StartPrevDay,
                    planRoomGroupSiteDiscountData.DiscountData!.EndPrevDay,
                    planRoomGroupSiteDiscountData.DiscountData!.PersonMin,
                    planRoomGroupSiteDiscountData.DiscountData!.PersonMax,
                    planRoomGroupSiteDiscountData.DiscountData!.Value,
                    planRoomGroupSiteDiscountData.DiscountData!.PriceSettingType
                )
            );

        var discountData = await queryable.ToListAsync(cancellationToken);

        var standardPriceLookup = new HashSet<(int?, int?, long, long)>(
            standardPriceData.Select(t => (t.PersonMin, t.PersonMax, t.RoomGroupId, t.PlanId))
        );

        var filteredData = discountData
            .Where(
                x => standardPriceLookup.Contains((x.PersonMin, x.PersonMax, x.RoomGroupId, x.PlanId))
            )
            .ToList();

        var mergedData = filteredData.Union(cachedData).ToList();

        var bulkCacheData = new Dictionary<string, List<BookingMetaDiscountDataModel>>();

        var dataLookup = mergedData
            .GroupBy(
                x => new
                {
                    x.PlanId,
                    x.RoomGroupId
                }
            )
            .ToDictionary(g => g.Key, g => g.ToList());

        foreach (var (planId, roomId, cacheKey, _) in cacheKeys)
        {
            var key = new
            {
                PlanId = planId,
                RoomGroupId = roomId
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
