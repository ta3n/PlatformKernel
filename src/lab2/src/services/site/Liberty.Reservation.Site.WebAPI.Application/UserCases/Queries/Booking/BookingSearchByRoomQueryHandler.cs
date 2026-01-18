using Liberty.Reservation.Application.Models.Requests;
using Liberty.Reservation.Site.Application.Models.Responses;
using Newtonsoft.Json;

namespace Liberty.Reservation.Site.WebAPI.Application.UserCases.Queries.Booking;

public class BookingSearchByRoomQueryHandler(
    IMapper mapper,
    ISecurityContextAccessor securityContextAccessor,
    ICacheService cacheService,
    IMediator mediator
) : QueryPageBaseHandler<BookingSearchByRoomQuery, BookingSearchByRoomResponse>(mapper, cacheService)
{
    protected override string GetCacheKey(
        BookingSearchByRoomQuery request
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
                    nameof(BookingSearchByRoomQueryHandler),
                    GetRequestJson(request)
                ]
            )
        );
    }

    protected override async Task<(IHeaderDictionary, IEnumerable<BookingSearchByRoomResponse>)> HandleAsync(
        BookingSearchByRoomQuery request,
        CancellationToken cancellationToken
    )
    {
        var payload = request.Payload;

        (IHeaderDictionary headers, IEnumerable<BookingSearchByRoomResponse> data) response = new();

        var listPlanOfBookingResponseCache = await GetBookingSearchByPlanAsync(
            request,
            cancellationToken
        );

        List<BookingSearchByPlanResponse> responseData;

        if (listPlanOfBookingResponseCache.data is null)
        {
            var (headers, data) = await mediator.Send(
                new BookingSearchByPlanQuery(payload, request.Pageable),
                cancellationToken
            );

            responseData = [.. data];
            response.headers = headers;
        }
        else
        {
            responseData = [.. listPlanOfBookingResponseCache.data];
            response.headers = listPlanOfBookingResponseCache.header;
        }

        var bookingData = ConvertToRoomOfBookingSearchResponse(responseData);
        bookingData = [.. bookingData.OrderByDescending(x => x.DisplayOrder)];
        CalculateMinPrices(bookingData, payload);

        response.data = bookingData;

        return response;
    }

    private string GetCacheKeyBookingSearchByPlan(
        BookingSearchByRoomQuery request
    )
    {
        var requestBookingSearchByPlan = new BookingSearchByPlanQuery(
            request.Payload,
            request.Pageable
        );

        var facilityId = securityContextAccessor.GetFacilityIdSelected();
        var siteId = securityContextAccessor.GetSiteIdSelected();
        var languageCode = securityContextAccessor.GetLanguageCode();

        var cacheKey = CacheHelper.GetCacheKeyByParameters(
            string.Format(
                CacheKeys.BookingSearchPrefixKey,
                facilityId,
                siteId
            ),
            CacheHelper.ComputeHash(
                [
                    nameof(BookingSearchByPlanQuery),
                    JsonConvert.SerializeObject(
                        requestBookingSearchByPlan,
                        JsonSettings.Optimized
                    )
                ]
            )
        );

        cacheKey = $"{cacheKey}:{languageCode}";

        return cacheKey;
    }

    private async Task<(IHeaderDictionary header, IEnumerable<BookingSearchByPlanResponse>? data)> GetBookingSearchByPlanAsync(
        BookingSearchByRoomQuery request,
        CancellationToken cancellationToken = default
    )
    {
        var cacheKeyBookingSearchByPlan = GetCacheKeyBookingSearchByPlan(request);
        var dataBookingSearchJson = await CacheService!.GetStringAsync(
            cacheKeyBookingSearchByPlan,
            cancellationToken
        );
        var dataBookingSearchTuple = ConvertJsonToTuple<IEnumerable<BookingSearchByPlanResponse>>(
            dataBookingSearchJson
        );

        var header = dataBookingSearchTuple?.Item1 ?? new HeaderDictionary();
        return (header, dataBookingSearchTuple?.Item2 ?? null);
    }

    private static List<BookingSearchByRoomResponse> ConvertToRoomOfBookingSearchResponse(
        IEnumerable<BookingSearchByPlanResponse> listPlanOfBookingResponse
    )
    {
        return
        [
            .. listPlanOfBookingResponse
                .SelectMany(
                    plan => plan.Rooms.Select(
                        room => new
                        {
                            Plan = plan,
                            Room = room
                        }
                    )
                )
                .GroupBy(x => x.Room.Id)
                .Select(
                    group => new BookingSearchByRoomResponse(
                        group.First().Room.Id,
                        group.First().Room.Name,
                        group.First().Room.Overview,
                        group.First().Room.DisplayOrder,
                        group.First().Room.IsEnabledSmoking
                    )
                    {
                        Files = group.First()
                            .Room.Files?
                            .Select(
                                file => new FileResponse(
                                    file.Code,
                                    file.ContentType
                                )
                            )
                            .ToList(),
                        Plans =
                        [
                            .. group
                                .OrderByDescending(x => x.Plan.DisplayOrder)
                                .Select(
                                    x => new PlanOfRoomSearchResponse(
                                        x.Plan.Id,
                                        x.Plan.Name,
                                        x.Plan.PlanType,
                                        x.Plan.DayUse
                                    )
                                    {
                                        BasePrice = x.Plan.BasePrice,
                                        CheckInStart = x.Plan.CheckInStart,
                                        CheckInEnd = x.Plan.CheckInEnd,
                                        CheckOut = x.Plan.CheckOut,
                                        Files = x.Plan.Files?
                                            .Select(
                                                file => new FileResponse(
                                                    file.Code,
                                                    file.ContentType
                                                )
                                            )
                                            .ToList(),
                                        AppDatePrices =
                                        [
                                            .. x.Room.AppDatePrices
                                                .Select(
                                                    price => new AppDatePriceOfPlanResponse(
                                                        price.AppDateId,
                                                        price.RemainNumber
                                                    )
                                                    {
                                                        BasePrice = price.BasePrice,
                                                        Price = price.Price,
                                                        TotalSpaTax = price.TotalSpaTax,
                                                        Status = price.Status
                                                    }
                                                )
                                        ]
                                    }
                                )
                        ]
                    }
                )
                .OrderByDescending(x => x.DisplayOrder)
        ];
    }

    private static void CalculateMinPrices(
        List<BookingSearchByRoomResponse> rooms,
        BookingSearchPlanRequest payload
    )
    {
        rooms.ForEach(
            item =>
            {
                var displayCheckInDate = payload.DisplayCheckInDate ?? payload.CheckInDate;
                var displayCheckOutDate = payload.DisplayCheckOutDate ?? payload.CheckOutDate;

                var minTotalPrice = item.Plans?
                    .SelectMany(x => x.AppDatePrices)
                    .Where(
                        x => x.AppDateId >= displayCheckInDate
                            && x.AppDateId <= displayCheckOutDate
                    )
                    .Select(x => x.TotalPrice)
                    .DefaultIfEmpty(0)
                    .Min();

                item.MinTotalPrice = minTotalPrice;
            }
        );
    }
}
