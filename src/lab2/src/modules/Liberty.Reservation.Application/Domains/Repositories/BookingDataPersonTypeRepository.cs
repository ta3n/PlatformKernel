using Liberty.Cache.Services;
using Liberty.Reservation.Application.Constants;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;
using Liberty.Reservation.Application.Domains.Repositories.Interfaces;
using Liberty.Reservation.Application.Models;
using Microsoft.EntityFrameworkCore;

namespace Liberty.Reservation.Application.Domains.Repositories;

public class BookingDataPersonTypeRepository(
    IDbContextFactory<DbContext> dbContextFactory,
    ICacheService cacheService
) : IBookingDataPersonTypeRepository
{
    public async Task<IEnumerable<BookingMetaPersonTypeModel>> GetAllBookingMetaPersonTypeModelsAsync(
        long facilityId,
        long siteId,
        long[] planIds,
        long[] roomGroupIds,
        bool facilityStateUseSpaTax,
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
                CacheKeys.BookingSearchPersonTypePrefixKey,
                facilityId,
                siteId,
                planId,
                roomGroupId
            )
            let cachePattern = string.Format(
                CacheKeys.BookingSearchPersonTypePrefixKey,
                facilityId,
                siteId,
                "*",
                "*"
            )
            select (planId, roomGroupId, cacheKey, cachePattern)).ToList();

        var cachedData = new List<BookingMetaPersonTypeModel>();
        if (useCache)
        {
            cachedData =
            [
                .. cacheService.GetByPatterns<BookingMetaPersonTypeModel>(
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

        var queryable = dbContext.Set<PlanRoomGroupSitePersonAgeType>()
            .AsNoTracking()
            .Where(
                planRoomGroupSitePersonAgeType
                    => planRoomGroupSitePersonAgeType.SiteId == siteId
            )
            .Where(
                planRoomGroupSitePersonAgeType =>
                    missingPlanIds.Contains(planRoomGroupSitePersonAgeType.PlanId)
            )
            .Where(
                planRoomGroupSitePersonAgeType =>
                    missingRoomGroupIds.Contains(planRoomGroupSitePersonAgeType.RoomGroupId)
            )
            .Where(planRoomGroupSitePersonAgeType => planRoomGroupSitePersonAgeType.IsEnabled)
            .Where(planRoomGroupSitePersonAgeType => planRoomGroupSitePersonAgeType.PersonAgeType!.IsEnabled)
            .Where(planRoomGroupSitePersonAgeType => planRoomGroupSitePersonAgeType.PersonAgeType!.IsVisible)
            .OrderBy(planRoomGroupSitePersonAgeType => planRoomGroupSitePersonAgeType.PlanId)
            .Select(
                planRoomGroupSitePersonAgeType => new BookingMetaPersonTypeModel(
                    planRoomGroupSitePersonAgeType.PlanId,
                    planRoomGroupSitePersonAgeType.RoomGroupId,
                    planRoomGroupSitePersonAgeType.SiteId,
                    planRoomGroupSitePersonAgeType.PersonAgeTypeId,
                    planRoomGroupSitePersonAgeType.PersonAgeType!.Name,
                    planRoomGroupSitePersonAgeType.PersonAgeType!.IsMain,
                    planRoomGroupSitePersonAgeType.PersonAgeType!.AgeMin,
                    planRoomGroupSitePersonAgeType.PersonAgeType!.AgeMax,
                    planRoomGroupSitePersonAgeType.PersonAgeType!.Meta!.FoodBed,
                    planRoomGroupSitePersonAgeType.IsRegardAdult,
                    planRoomGroupSitePersonAgeType.PriceSettingType,
                    planRoomGroupSitePersonAgeType.Value,
                    planRoomGroupSitePersonAgeType.IsEnabled,
                    facilityStateUseSpaTax,
                    planRoomGroupSitePersonAgeType.PersonAgeType!.DisplayOrder,
                    planRoomGroupSitePersonAgeType.PersonAgeType!.PersonAgeTypeSpaTaxDatas!
                        .Where(
                            personAgeTypeSpaTaxData
                                => personAgeTypeSpaTaxData.SpaTaxData!.IsVisible
                            // && personAgeTypeSpaTaxData.SpaTaxData!.IsEnabled
                        )
                        .Select(
                            personAgeTypeSpaTaxData => new SpaTaxDataOfBookingMetaPersonTypeModel(
                                personAgeTypeSpaTaxData.SpaTaxDataId,
                                personAgeTypeSpaTaxData.SpaTaxData!.PriceMax,
                                personAgeTypeSpaTaxData.SpaTaxData!.PriceMin,
                                personAgeTypeSpaTaxData.SpaTaxData!.Tax,
                                personAgeTypeSpaTaxData.SpaTaxData!.IsEnabled
                            )
                        )
                )
            )
            .AsSingleQuery();

        var personTypes = await queryable.ToListAsync(cancellationToken);

        var mergedData = personTypes.Union(cachedData).ToList();

        var bulkCacheData = new Dictionary<string, List<BookingMetaPersonTypeModel>>();

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
