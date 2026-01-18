using Liberty.Cache.Services;
using Liberty.Reservation.Application.Constants;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;
using Liberty.Reservation.Application.Domains.Repositories.Interfaces;
using Liberty.Reservation.Application.Models;
using Liberty.Reservation.Application.Models.Requests;
using Microsoft.EntityFrameworkCore;

namespace Liberty.Reservation.Application.Domains.Repositories;

public class BookingDataOptionItemRepository(
    IDbContextFactory<DbContext> dbContextFactory,
    ICacheService cacheService
) : IBookingDataOptionItemRepository
{
    public async Task<IEnumerable<BookingMetaOptionItemModel>> GetAllBookingMetaOptionItemModelsAsync(
        long facilityId,
        long siteId,
        long[] planIds,
        BookingSearchPlanRequest search,
        long? existingReservationId,
        CancellationToken cancellationToken
    )
    {
        if (planIds is not { Length: > 0 })
        {
            return [];
        }

        var listOptionItemIds = search.GetOptionItemIds();
        if (listOptionItemIds is not { Length: > 0 })
        {
            return [];
        }

        var appDates = search.GetAppDates();

        var cacheKeys = (
            from planId in planIds
            from appDateId in appDates
            let cacheKey = string.Format(
                CacheKeys.BookingSearchDataOptionItemPrefixKey,
                facilityId,
                siteId,
                planId,
                appDateId
            )
            let cachePattern = string.Format(
                CacheKeys.BookingSearchDataOptionItemPrefixKey,
                facilityId,
                siteId,
                "*",
                appDateId
            )
            select (planId, cacheKey, cachePattern)).ToList();

        var cachedData = new List<BookingMetaOptionItemModel>();
        if (search.UseCache)
        {
            cachedData =
            [
                .. cacheService.GetByPatterns<BookingMetaOptionItemModel>(
                        [.. cacheKeys.Select(x => x.cachePattern).Distinct()]
                    )
                    .Where(
                        x => cacheKeys.Exists(
                            key => key.planId == x.PlanId
                        )
                    )
            ];
        }

        var missingKeys = (
                from planId in planIds
                from optionItemId in listOptionItemIds
                from appDate in appDates
                where !cachedData.Exists(
                    x =>
                        x.PlanId == planId
                        && x.OptionItemId == optionItemId
                        && x.AppDates.Any(
                            d => d.AppDateId == appDate
                        )
                )
                select new
                {
                    PlanId = planId,
                    OptionItemId = optionItemId,
                    AppDateId = appDate
                }
            ).Distinct()
            .ToArray();
        if (missingKeys is not { Length: > 0 })
        {
            return cachedData;
        }

        var missingPlanIds = missingKeys
            .Select(x => x.PlanId)
            .Distinct()
            .ToArray();

        var missingOptionItemIds = missingKeys
            .Select(x => x.OptionItemId)
            .Distinct()
            .ToArray();

        var missingAppDates = missingKeys
            .Select(x => x.AppDateId)
            .Distinct()
            .ToArray();

        await using var dbContext = await dbContextFactory.CreateDbContextAsync(cancellationToken);

        var queryable = dbContext.Set<PlanOptionItem>()
            .AsNoTracking()
            .Where(planOptionItem => missingPlanIds.Contains(planOptionItem.PlanId))
            .Where(planOptionItem => planOptionItem.OptionItem!.IsEnabled)
            .Where(planOptionItem => planOptionItem.Plan!.UseFixedOptionItem)
            .Where(planOptionItem => missingOptionItemIds.Contains(planOptionItem.OptionItemId))
            .Select(
                planOptionItem => new BookingMetaOptionItemModel(
                    planOptionItem.PlanId,
                    planOptionItem.OptionItemId,
                    planOptionItem.Number,
                    planOptionItem.OptionItemTarget,
                    planOptionItem.PlanOptionItemType,
                    planOptionItem.OptionItem!.OptionItemAppDates!
                        .Where(optionItemAppDate => missingAppDates.Contains(optionItemAppDate.AppDateId))
                        .Where(optionItemAppDate => !optionItemAppDate.IsNotSelled)
                        .Where(optionItemAppDate => optionItemAppDate.IsEnabled)
                        .Where(optionItemAppDate => optionItemAppDate.SellNumber > 0)
                        .Select(
                            optionItemAppDate => new BookingMetaPlanOptionItemAppDate(
                                optionItemAppDate.OptionItemId,
                                optionItemAppDate.OptionItem!.Name,
                                optionItemAppDate.OptionItem!.Price,
                                optionItemAppDate.AppDateId,
                                optionItemAppDate.IsNotSelled,
                                optionItemAppDate.SellNumber,
                                optionItemAppDate.OptionItem.MaxSupplyNumber,
                                optionItemAppDate.OptionItem!.ReservationRoomGroupAppDateOptionItems!
                                    .Where(x => x.BookingDateId == optionItemAppDate.AppDateId && x.ReservationId != existingReservationId)
                                    .Where(
                                        x =>
                                            x.Reservation!.ReservationState == ReservationStatus.Confirmed
                                            || x.Reservation!.ReservationState == ReservationStatus.Reserved
                                            || x.Reservation!.ReservationState == ReservationStatus.Modified
                                    )
                                    .Sum(x => x.Number),
                                optionItemAppDate.ReservationRoomGroupAppDateOptionItems!.Select(
                                    reservationRoomGroupAppDateOptionItem => new BookingMetaReservationOptionItemAppDate(
                                        reservationRoomGroupAppDateOptionItem.ReservationId,
                                        reservationRoomGroupAppDateOptionItem.RoomGroupId,
                                        reservationRoomGroupAppDateOptionItem.OptionItemId,
                                        reservationRoomGroupAppDateOptionItem.BookingDateId,
                                        reservationRoomGroupAppDateOptionItem.RestIndex,
                                        reservationRoomGroupAppDateOptionItem.RoomGroupIndex,
                                        reservationRoomGroupAppDateOptionItem.Price,
                                        reservationRoomGroupAppDateOptionItem.Number,
                                        reservationRoomGroupAppDateOptionItem.TotalPrice
                                    )
                                )
                            )
                        )
                )
            )
            .AsSingleQuery();

        var optionItems = await queryable.ToListAsync(cancellationToken);

        MergeOptionItems(cachedData, optionItems);
        var bulkCacheData = new Dictionary<string, List<BookingMetaOptionItemModel>>();

        var dataLookup = optionItems
            .GroupBy(x => new { x.PlanId })
            .ToDictionary(g => g.Key, g => g.ToList());

        foreach (var (planId, cacheKey, _) in cacheKeys)
        {
            var key = new { PlanId = planId };

            if (dataLookup.TryGetValue(key, out var value) && value.Count > 0)
            {
                bulkCacheData[cacheKey] = value;
            }
        }

        if (bulkCacheData is { Count: > 0 })
        {
            await cacheService.SetBulkAsync(bulkCacheData, cancellationToken);
        }

        return cachedData;
    }

    private static void MergeOptionItems(
        List<BookingMetaOptionItemModel> cachedData,
        List<BookingMetaOptionItemModel> optionItems
    )
    {
        foreach (var item in optionItems)
        {
            var existing = cachedData.Find(
                x =>
                    x.PlanId == item.PlanId && x.OptionItemId == item.OptionItemId
            );

            if (existing == null)
            {
                cachedData.Add(item);
            }
            else
            {
                var mergedAppDates = existing.AppDates.Union(item.AppDates).ToList();
                var mergedItem = existing with { AppDates = mergedAppDates };
                var idx = cachedData.FindIndex(
                    x =>
                        x.PlanId == item.PlanId && x.OptionItemId == item.OptionItemId
                );
                cachedData[idx] = mergedItem;
            }
        }
    }
}
