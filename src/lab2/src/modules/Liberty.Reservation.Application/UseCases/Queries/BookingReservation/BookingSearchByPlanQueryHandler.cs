using AutoMapper;
using Liberty.Pagination;
using Liberty.Pagination.Utilities;
using Liberty.Reservation.Application.Cqrs.BaseQueries;
using Liberty.Reservation.Application.Domains.Services.Interfaces;
using Liberty.Reservation.Application.Models;
using Liberty.Reservation.Application.Models.Responses;
using Microsoft.AspNetCore.Http;

namespace Liberty.Reservation.Application.UseCases.Queries.BookingReservation;

public class BookingSearchByPlanQueryHandler(
    IMapper mapper,
    IBookingSearchService bookingSearchService,
    IBookingCalendarPriceService bookingCalendarPriceService
) : QueryPageBaseHandler<BookingSearchByPlanQuery, BookingSearchByPlanResponse>(mapper)
{
    protected override async Task<(IHeaderDictionary, IEnumerable<BookingSearchByPlanResponse>)> HandleAsync(
        BookingSearchByPlanQuery request,
        CancellationToken cancellationToken
    )
    {
        var payload = request.Payload;
        var facilityId = request.FacilityId;
        var siteId = request.SiteId;
        var isLoadingPriceAppDate = request.IsLoadingPriceAppDate;

        var page = await GetBookingDataPlansAsync(
            request,
            cancellationToken
        );
        var plans = page.Content as List<BookingPlanModel> ?? [];

        var bookingSearchResponse = bookingCalendarPriceService.GetAllRoomDatePricesInPlans(
            payload,
            [.. plans],
            siteId,
            facilityId,
            isLoadingPriceAppDate
        );

        var headers = page.GeneratePaginationHttpHeaders();

        return (headers, bookingSearchResponse);
    }

    private async Task<IPage<BookingPlanModel>> GetBookingDataPlansAsync(
        BookingSearchByPlanQuery request,
        CancellationToken cancellationToken
    )
    {
        var pageBookingPlanData = await bookingSearchService.GetAllBookingDataPlansAsync(
            request.FacilityId,
            request.SiteId,
            request.IsLoadingPriceAppDate,
            request.Payload,
            request.Pageable,
            cancellationToken
        );

        return pageBookingPlanData;
    }
}
