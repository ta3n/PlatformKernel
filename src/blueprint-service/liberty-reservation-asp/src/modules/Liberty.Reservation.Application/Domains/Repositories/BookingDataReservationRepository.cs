using Liberty.Cache.Services;
using Liberty.Reservation.Application.Constants;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;
using Liberty.Reservation.Application.Domains.Repositories.Interfaces;
using Liberty.Reservation.Application.Models;
using Liberty.Reservation.Application.Models.Requests;
using Microsoft.EntityFrameworkCore;

namespace Liberty.Reservation.Application.Domains.Repositories;

public class BookingDataReservationRepository(
    IDbContextFactory<DbContext> dbContextFactory,
    ICacheService cacheService
) : IBookingDataReservationRepository
{
    public async Task<IEnumerable<BookingMetaReservationModel>> GetAllBookingMetaReservationModelsAsync(
        long facilityId,
        long siteId,
        long[] planIds,
        BookingSearchPlanRequest search,
        CancellationToken cancellationToken
    )
    {
        if (planIds is not { Length: > 0 })
        {
            return [];
        }

        var appDates = search.GetAppDates();

        var cacheKeys = (
            from planId in planIds
            from bookingDateId in appDates
            let cacheKey = string.Format(
                CacheKeys.BookingSearchDataReservationPrefixKey,
                facilityId,
                siteId,
                planId,
                bookingDateId
            )
            let cachePattern = string.Format(
                CacheKeys.BookingSearchDataReservationPrefixKey,
                facilityId,
                siteId,
                "*",
                bookingDateId
            )
            select (planId, bookingDateId, cacheKey, cachePattern)).ToList();

        var cachedData = new List<BookingMetaReservationModel>();
        if (search.UseCache)
        {
            cachedData =
            [
                .. cacheService.GetByPatterns<BookingMetaReservationModel>(
                        [.. cacheKeys.Select(x => x.cachePattern).Distinct()]
                    )
                    .Where(
                        x => cacheKeys.Exists(
                            key => key.planId == x.PlanId
                        )
                    )
            ];
        }

        var missingKeys = cacheKeys
            .Where(
                x => !cachedData.Exists(
                    c => c.PlanId == x.planId && x.bookingDateId == c.BookingDateId
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

        var missingAppDates = missingKeys
            .Select(x => x.bookingDateId)
            .Distinct()
            .ToArray();

        await using var dbContext = await dbContextFactory.CreateDbContextAsync(cancellationToken);

        var queryable = dbContext.Set<ReservationPlanRoomGroupAppDate>()
            .AsNoTracking()
            .Where(reservationPlanRoomGroupAppDate => missingPlanIds.Contains(reservationPlanRoomGroupAppDate.PlanId))
            // .Where(reservationPlanRoomGroupAppDate => reservationPlanRoomGroupAppDate.BookingDateId >= search.CheckInDate)
            .Where(
                reservationPlanRoomGroupAppDate => missingAppDates.Contains(reservationPlanRoomGroupAppDate.BookingDateId)
            )
            .Where(
                reservationPlanRoomGroupAppDate
                    => reservationPlanRoomGroupAppDate.Reservation!.ReservationState == ReservationStatus.Confirmed
                    || reservationPlanRoomGroupAppDate.Reservation.ReservationState == ReservationStatus.Reserved
                    || reservationPlanRoomGroupAppDate.Reservation.ReservationState == ReservationStatus.Modified
            )
            .OrderBy(reservationPlanRoomGroupAppDate => reservationPlanRoomGroupAppDate.BookingDateId)
            .Select(
                reservationPlanRoomGroupAppDate => new BookingMetaReservationModel(
                    reservationPlanRoomGroupAppDate.ReservationId,
                    reservationPlanRoomGroupAppDate.PlanId,
                    reservationPlanRoomGroupAppDate.RoomGroupId,
                    reservationPlanRoomGroupAppDate.BookingDateId,
                    reservationPlanRoomGroupAppDate.RestIndex,
                    reservationPlanRoomGroupAppDate.RoomGroupIndex,
                    reservationPlanRoomGroupAppDate.CheckInTime,
                    reservationPlanRoomGroupAppDate.CheckOutTime,
                    reservationPlanRoomGroupAppDate.Reservation!.IsReserved
                )
            )
            .AsSingleQuery();

        var reservations = await queryable.ToListAsync(cancellationToken);

        var mergedData = reservations.Union(cachedData).ToList();

        var bulkCacheData = new Dictionary<string, List<BookingMetaReservationModel>>();

        var dataLookup = mergedData
            .GroupBy(
                x => new
                {
                    x.PlanId,
                    x.BookingDateId
                }
            )
            .ToDictionary(g => g.Key, g => g.ToList());

        foreach (var (planId, bookingDateId, cacheKey, _) in cacheKeys)
        {
            var key = new
            {
                PlanId = planId,
                BookingDateId = bookingDateId
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
