using Liberty.Reservation.Application.Models;
using Liberty.Reservation.Application.Models.Responses;

namespace Liberty.Reservation.Application.Domains.Services.Interfaces;

/// <summary>
/// Provides functionality to retrieve pricing information for all room dates across specified booking plans.
/// </summary>
public interface IBookingCalendarPriceService
{
    /// <summary>
    /// Retrieves all room date prices across different plans based on the provided booking search criteria and plan data.
    /// </summary>
    /// <param name="bookingSearchModel">
    /// The booking search model containing search criteria such as dates, guest numbers, or other relevant filters.
    /// </param>
    /// <param name="listPlan">
    /// The list of booking plan data containing the details of various plans such as pricing rules, availability, and configurations.
    /// </param>
    /// <param name="siteId">
    /// The identifier of the site for which the room prices are being retrieved.
    /// </param>
    /// <param name="facilityId">
    /// The identifier of the facility for which the room prices are being retrieved.
    /// </param>
    /// <param name="isLoadingPriceAppDate">
    /// A boolean flag indicating whether to load price application dates. Defaults to <c>true</c>.
    /// </param>
    /// <returns>
    /// A collection of <see cref="BookingSearchByPlanResponse"/> objects, where each object contains the room date price information
    /// related to a specific plan.
    /// </returns>
    /// <example>
    /// <code>
    /// var prices = bookingCalendarPriceService.GetAllRoomDatePricesInPlans(
    ///     bookingSearchModel,
    ///     listPlan,
    ///     siteId,
    ///     facilityId
    /// );
    /// </code>
    /// </example>
    IEnumerable<BookingSearchByPlanResponse> GetAllRoomDatePricesInPlans(
        BookingSearchModel bookingSearchModel,
        List<BookingPlanModel> listPlan,
        long siteId,
        long facilityId,
        bool isLoadingPriceAppDate = true
    );
}
