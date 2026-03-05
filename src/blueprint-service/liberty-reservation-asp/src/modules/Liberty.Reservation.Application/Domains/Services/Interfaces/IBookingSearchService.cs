using Liberty.Pagination;
using Liberty.Reservation.Application.Models;
using Liberty.Reservation.Application.Models.Requests;

namespace Liberty.Reservation.Application.Domains.Services.Interfaces;

/// <summary>
/// Defines methods for searching and retrieving booking-related data.
/// </summary>
public interface IBookingSearchService
{
    /// <summary>
    /// Retrieves all booking data plans for a specified facility and site,
    /// applying search filters and pagination.
    /// </summary>
    /// <param name="facilityId">
    /// The ID of the facility to search for booking plans.
    /// </param>
    /// <param name="siteId">
    /// The ID of the site to search for booking plans.
    /// </param>
    /// <param name="isLoadingPriceAppDate">
    /// A boolean flag indicating whether to load price application dates.
    /// </param>
    /// <param name="search">
    /// The search request containing filters for the booking plans.
    /// </param>
    /// <param name="pageable">
    /// The pagination information for retrieving a specific page of results.
    /// </param>
    /// <param name="cancellationToken">
    /// A token to observe while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A task representing the asynchronous operation. The result contains a paginated collection of booking plan data.
    /// </returns>
    /// <example>
    /// <code>
    /// var bookingPlans = await bookingSearchService.GetAllBookingDataPlansAsync(
    ///     facilityId: 1,
    ///     siteId: 2,
    ///     isLoadingPriceAppDate: true,
    ///     search: new BookingSearchPlanRequest { /* filters */ },
    ///     pageable: new Pageable(1, 10),
    ///     cancellationToken: CancellationToken.None
    /// );
    /// </code>
    /// </example>
    Task<IPage<BookingPlanModel>> GetAllBookingDataPlansAsync(
        long facilityId,
        long siteId,
        bool isLoadingPriceAppDate,
        BookingSearchPlanRequest search,
        IPageable pageable,
        CancellationToken cancellationToken
    );

    /// <summary>
    /// Retrieves all booking data plans for a specified facility and site,
    /// applying search filters and pagination.
    /// </summary>
    /// <param name="facilityId">
    /// The ID of the facility to search for booking plans.
    /// </param>
    /// <param name="siteId">
    /// The ID of the site to search for booking plans.
    /// </param>
    /// <param name="isLoadingPriceAppDate">
    /// A boolean flag indicating whether to load price application dates.
    /// </param>
    /// <param name="search">
    /// The search request containing filters for the booking plans,
    /// specifically for price calendar data.
    /// </param>
    /// <param name="pageable">
    /// The pagination information for retrieving a specific page of results.
    /// </param>
    /// <param name="cancellationToken">
    /// A token to observe while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A task representing the asynchronous operation. The result contains a paginated collection of booking plan data.
    /// </returns>
    /// <example>
    /// <code>
    /// var bookingPlans = await bookingSearchService.GetAllBookingDataPlansAsync(
    ///     facilityId: 1,
    ///     siteId: 2,
    ///     isLoadingPriceAppDate: true,
    ///     search: new BookingSearchPriceCalendarRequest { /* filters */ },
    ///     pageable: new Pageable(1, 10),
    ///     cancellationToken: CancellationToken.None
    /// );
    /// </code>
    /// </example>
    Task<IPage<BookingPlanModel>> GetAllBookingDataPlansAsync(
        long facilityId,
        long siteId,
        bool isLoadingPriceAppDate,
        BookingSearchPriceCalendarRequest search,
        IPageable pageable,
        CancellationToken cancellationToken
    );

    /// <summary>
    /// Retrieves detailed booking plan data based on the specified plan and search criteria.
    /// </summary>
    /// <param name="request">
    /// The booking plan detail request containing specific identifiers such as facility ID,
    /// site ID, plan ID, and additional information necessary for fetching the plan details.
    /// </param>
    /// <param name="search">
    /// The search request containing criteria related to the booking plan, such as filters or configurations.
    /// </param>
    /// <param name="cancellationToken">
    /// A token to monitor for cancellation requests and terminate the operation if requested.
    /// </param>
    /// <returns>
    /// A <see cref="BookingPlanModel"/> object containing detailed information about the specified booking plan.
    /// </returns>
    Task<BookingPlanModel?> GetBookingDataDetailByPlanAsync(
        BookingPlanDetailRequest request,
        BookingSearchPlanRequest search,
        CancellationToken cancellationToken
    );
}
