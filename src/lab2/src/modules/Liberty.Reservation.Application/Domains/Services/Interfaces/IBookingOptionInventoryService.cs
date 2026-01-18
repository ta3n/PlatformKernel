using Liberty.Reservation.Application.Models.Requests;
using Liberty.Reservation.Application.Models;

namespace Liberty.Reservation.Application.Domains.Services.Interfaces;

/// <summary>
/// Provides methods for validating booking option inventory and retrieving reservation option item data.
/// </summary>
public interface IBookingOptionInventoryService
{
    /// <summary>
    /// Validates the inventory for booking options for a given facility, site, and plan.
    /// </summary>
    /// <param name="facilityId">The unique identifier of the facility.</param>
    /// <param name="siteId">The unique identifier of the site.</param>
    /// <param name="planId">The unique identifier of the booking plan.</param>
    /// <param name="checkInDate">The check-in date represented as a long (Unix timestamp).</param>
    /// <param name="restNumber">The number of rest days for the booking.</param>
    /// <param name="options">A list of booking search option models to validate.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous validation operation.</returns>
    /// <example>
    /// <code>
    /// await bookingOptionInventoryService.ValidateOptionInventoryAsync(
    ///     facilityId: 1,
    ///     siteId: 2,
    ///     planId: 3,
    ///     checkInDate: 20240601,
    ///     restNumber: 2,
    ///     options: new List&lt;OptionOfBookingSearchModel&gt; { /* options */ },
    ///     cancellationToken: CancellationToken.None
    /// );
    /// </code>
    /// </example>
    Task ValidateOptionInventoryAsync(
        OptionInventoryModel optionInventoryModel,
        CancellationToken cancellationToken
    );

    /// <summary>
    /// Retrieves all reservation option item data for a booking request, available booking plan, and room application dates.
    /// </summary>
    /// <param name="bookingCreateRequest">The booking creation request containing booking details.</param>
    /// <param name="bookingDataAvailable">The available booking plan model.</param>
    /// <param name="bookingRoomAppDates">A collection of booking room application date models.</param>
    /// <returns>An enumerable of booking reservation option item data.</returns>
    /// <example>
    /// <code>
    /// var optionItems = bookingOptionInventoryService.GetAllReservationOptionItemData(
    ///     bookingCreateRequest,
    ///     bookingDataAvailable,
    ///     bookingRoomAppDates
    /// );
    /// </code>
    /// </example>
    IEnumerable<BookingReservationOptionItemData> GetAllReservationOptionItemData(
        BookingCreateRequest bookingCreateRequest,
        BookingPlanModel bookingDataAvailable,
        IEnumerable<BookingRoomAppDateModel> bookingRoomAppDates,
        long? existingReservationId
    );
}
