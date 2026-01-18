using Liberty.Reservation.Application.Domains.Services.Interfaces;
using Liberty.Reservation.Application.Models;
using Liberty.Reservation.Application.Models.Requests;

namespace Liberty.Reservation.Site.WebAPI.Application.UserCases.Queries.Booking;

public class BookingGetAllPriceCalendarsQueryHandler(
    IMapper mapper,
    ICacheService cacheService,
    ISecurityContextAccessor securityContextAccessor,
    IBookingSearchService bookingSearchService,
    IBookingCalendarPriceService bookingCalendarPriceService
) : QueryListBaseHandler<BookingGetAllPriceCalendarsQuery, BookingCalendarPriceResponse>(mapper, cacheService)
{
    protected override string GetCacheKey(
        BookingGetAllPriceCalendarsQuery request
    )
    {
        var facilityId = securityContextAccessor.GetFacilityIdSelected();
        var siteId = securityContextAccessor.GetSiteIdSelected();

        return CacheHelper.GetCacheKeyByParameters(
            string.Format(
                CacheKeys.BookingSearchPrefixKey,
                facilityId,
                siteId
            ),
            CacheHelper.ComputeHash(
                [
                    nameof(BookingGetAllPriceCalendarsQueryHandler),
                    GetRequestJson(request)
                ]
            )
        );
    }

    protected override async Task<(IHeaderDictionary, IEnumerable<BookingCalendarPriceResponse>)> HandleAsync(
        BookingGetAllPriceCalendarsQuery request,
        CancellationToken cancellationToken
    )
    {
        var payload = request.Payload;
        var facilityId = securityContextAccessor.GetFacilityIdSelected();
        var siteId = securityContextAccessor.GetSiteIdSelected();

        var pagePlans = await bookingSearchService.GetAllBookingDataPlansAsync(
            facilityId,
            siteId,
            true,
            payload,
            PageableConstants.UnPaged,
            cancellationToken
        );
        var plans = pagePlans.Content as List<BookingPlanModel> ?? [];

        var bookingSearchResponse = bookingCalendarPriceService.GetAllRoomDatePricesInPlans(
                payload.BookingSearch,
                [.. plans],
                siteId,
                facilityId
            )
            .ToList();

        var data = new List<BookingCalendarPriceResponse>();

        foreach (var item in payload.PlanRoomGroups)
        {
            var planResponse = bookingSearchResponse.Find(
                x => x.Id == item.PlanId
            );
            if (planResponse is null)
            {
                continue;
            }

            var roomResponse = planResponse.Rooms.Find(
                x => x.Id == item.RoomGroupId
            );
            if (roomResponse is null)
            {
                continue;
            }

            var bookingCalendarPriceResponse = new BookingCalendarPriceResponse(
                item.PlanId,
                item.RoomGroupId,
                GetPriceCalendar(
                    payload,
                    roomResponse,
                    planResponse.CheckInEnd
                )
            );

            data.Add(bookingCalendarPriceResponse);
        }

        return (new HeaderDictionary(), data);
    }

    private static PriceCalendarOfRoomResponse GetPriceCalendar(
        BookingSearchPriceCalendarRequest request,
        RoomOfPlanResponse roomData,
        TimeSpan? checkInEnd
    )
    {
        var payload = request.BookingSearch;

        var dataResult = new PriceCalendarOfRoomResponse
        {
            AppDatePrices = [.. roomData.AppDatePrices],
            CheckInEnd = checkInEnd
        };

        var appDatePrices = dataResult.AppDatePrices
            .Where(x => x.AppDateId >= payload.CheckInDate)
            .Where(x => x.AppDateId < payload.CheckOutDate)
            .ToList();

        dataResult.MinTotalPrice = CalculateMinPrices(
            appDatePrices,
            payload
        );

        var checkInDate = AppDate.GetDateTime(payload.CheckInDate);
        var checkOutDate = AppDate.GetDateTime(payload.CheckOutDate);
        var totalStayDuration = (checkOutDate - checkInDate).Days;
        var isDateMatching = payload.RestNumber == appDatePrices.Count // Night must price
            && totalStayDuration == payload.RestNumber; // Night matching
        if (!isDateMatching)
        {
            return dataResult;
        }

        dataResult.Price = appDatePrices.Find(x => x.Status.IsAvailable && x.AppDateId == payload.CheckInDate)?.BasePrice ?? 0;
        dataResult.TotalSpaTax = appDatePrices.Find(x => x.Status.IsAvailable && x.AppDateId == payload.CheckInDate)?.TotalSpaTax
            ?? 0;
        dataResult.TotalPrice = dataResult.Price + dataResult.TotalSpaTax;
        dataResult.AppDatePrices = appDatePrices;

        return dataResult;
    }

    private static decimal? CalculateMinPrices(
        List<AppDatePriceOfPlanResponse> appDatePrices,
        BookingSearchPlanRequest payload
    )
    {
        var displayCheckInDate = payload.DisplayCheckInDate ?? payload.CheckInDate;
        var displayCheckOutDate = payload.DisplayCheckOutDate ?? payload.CheckOutDate;

        var minTotalPrice = appDatePrices
            .Where(
                x => x.AppDateId >= displayCheckInDate
                    && x.AppDateId <= displayCheckOutDate
            )
            .Select(x => x.TotalPrice)
            .DefaultIfEmpty(0)
            .Min();

        return minTotalPrice;
    }
}
