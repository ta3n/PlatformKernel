using Liberty.Reservation.Application.Domains.Services.Interfaces;
using Liberty.Reservation.Application.Exceptions;
using Liberty.Reservation.Application.Models.Requests;

namespace Liberty.Reservation.Site.WebAPI.Application.UserCases.Queries.Booking;

public class BookingGetCalendarPricesQueryHandler(
    IMapper mapper,
    ICacheService cacheService,
    ISecurityContextAccessor securityContextAccessor,
    IBookingSearchService bookingSearchService,
    IBookingCalendarPriceService bookingCalendarPriceService
) : QuerySingleBaseHandler<BookingGetCalendarPricesQuery, PriceCalendarOfRoomResponse>(mapper, cacheService)
{
    protected override string GetCacheKey(
        BookingGetCalendarPricesQuery request
    )
    {
        var facilityId = securityContextAccessor.GetFacilityIdSelected();
        var siteId = securityContextAccessor.GetSiteIdSelected();

        return CacheHelper.GetCacheKeyByParameters(
            string.Format(
                CacheKeys.BookingDetailPrefixKey,
                facilityId,
                siteId,
                request.PlanId,
                request.RoomGroupId
            ),
            CacheHelper.ComputeHash(
                [
                    nameof(BookingGetCalendarPricesQueryHandler),
                    GetRequestJson(request)
                ]
            )
        );
    }

    protected override async Task<(IHeaderDictionary, PriceCalendarOfRoomResponse)> HandleAsync(
        BookingGetCalendarPricesQuery request,
        CancellationToken cancellationToken
    )
    {
        var payload = request.Payload;
        var siteId = securityContextAccessor.GetSiteIdSelected();
        var facilityId = securityContextAccessor.GetFacilityIdSelected();

        var plan = await bookingSearchService.GetBookingDataDetailByPlanAsync(
                new BookingPlanDetailRequest(
                    facilityId,
                    siteId,
                    request.PlanId,
                    request.RoomGroupId,
                    payload.CheckInDate,
                    payload.RestNumber,
                    payload.Secret
                ),
                payload,
                cancellationToken
            )
            ?? throw new PlanNotfoundException();
        if (plan.DayUse && payload.RestNumber != 1)
        {
            throw new BookingInvalidNightNumberOfUseDayException();
        }

        var dataResult = new PriceCalendarOfRoomResponse();

        var listPlanOfBookingResponse = bookingCalendarPriceService.GetAllRoomDatePricesInPlans(
            request.Payload,
            [plan],
            siteId,
            facilityId
        );

        var data = listPlanOfBookingResponse.FirstOrDefault();
        if (data is null)
        {
            return (new HeaderDictionary(), dataResult);
        }

        dataResult = new PriceCalendarOfRoomResponse
        {
            AppDatePrices = [.. data.Rooms.SelectMany(x => x.AppDatePrices)],
            CheckInEnd = data.CheckInEnd
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
            return (new HeaderDictionary(), dataResult);
        }

        dataResult.Price = appDatePrices.Find(x => x.Status.IsAvailable && x.AppDateId == payload.CheckInDate)?.BasePrice ?? 0;
        dataResult.TotalSpaTax = appDatePrices.Find(x => x.Status.IsAvailable && x.AppDateId == payload.CheckInDate)?.TotalSpaTax
            ?? 0;
        dataResult.TotalPrice = dataResult.Price + dataResult.TotalSpaTax;
        dataResult.AppDatePrices = appDatePrices;

        return (new HeaderDictionary(), dataResult);
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
