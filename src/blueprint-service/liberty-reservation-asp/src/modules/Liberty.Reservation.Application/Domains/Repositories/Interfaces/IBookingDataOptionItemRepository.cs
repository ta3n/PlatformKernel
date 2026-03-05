using Liberty.Reservation.Application.Models;
using Liberty.Reservation.Application.Models.Requests;

namespace Liberty.Reservation.Application.Domains.Repositories.Interfaces;

/// <summary>
/// Interface for accessing booking data related to option items.
/// Provides methods to retrieve booking metadata for option items based on plans and search criteria.
/// </summary>
public interface IBookingDataOptionItemRepository
{
    /// <summary>
    /// Retrieves all booking metadata for option items based on the specified facility, site, plans, and search criteria.
    /// </summary>
    /// <param name="facilityId">
    /// The ID of the facility to filter the results.
    /// </param>
    /// <param name="siteId">
    /// The ID of the site to filter the results.
    /// </param>
    /// <param name="planIds">
    /// An array of plan IDs to filter the results.
    /// </param>
    /// <param name="search">
    /// The search criteria for booking plans, including filters such as dates and guest numbers.
    /// </param>
    /// <param name="cancellationToken">
    /// A token to monitor for cancellation requests, allowing the operation to be canceled if needed.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains
    /// an enumerable collection of <see cref="BookingMetaOptionItemModel"/>, where each model
    /// represents metadata for option items based on the specified criteria.
    /// </returns>
    /// <example>
    /// <code>
    /// var optionItemMetadata = await bookingDataOptionItemRepository.GetAllBookingMetaOptionItemModelsAsync(
    ///     facilityId: 1,
    ///     siteId: 2,
    ///     planIds: new long[] { 101, 102 },
    ///     search: new BookingSearchPlanRequest { /* filters */ },
    ///     cancellationToken: CancellationToken.None
    /// );
    /// </code>
    /// </example>
    Task<IEnumerable<BookingMetaOptionItemModel>> GetAllBookingMetaOptionItemModelsAsync(
        long facilityId,
        long siteId,
        long[] planIds,
        BookingSearchPlanRequest search,
        long? existingReservationId,
        CancellationToken cancellationToken
    );
}
