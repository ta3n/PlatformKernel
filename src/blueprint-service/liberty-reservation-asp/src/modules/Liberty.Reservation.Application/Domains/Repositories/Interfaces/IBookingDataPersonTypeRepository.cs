using Liberty.Reservation.Application.Models;

namespace Liberty.Reservation.Application.Domains.Repositories.Interfaces;

/// <summary>
/// Interface for accessing booking data related to person types.
/// Provides methods to retrieve booking metadata for person types based on site, plans, room groups, and facility state.
/// </summary>
public interface IBookingDataPersonTypeRepository
{
    /// <summary>
    /// Retrieves all booking metadata for person types based on the specified facility, site, plans, room groups,
    /// and whether the facility state uses spa tax.
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
    /// <param name="roomGroupIds">
    /// An array of room group IDs to filter the results.
    /// </param>
    /// <param name="facilityStateUseSpaTax">
    /// A boolean indicating whether the facility state uses spa tax.
    /// </param>
    /// <param name="useCache">
    /// A boolean indicating whether to use cached data for the operation.
    /// </param>
    /// <param name="cancellationToken">
    /// A token to monitor for cancellation requests, allowing the operation to be canceled if needed.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains
    /// an enumerable collection of <see cref="BookingMetaPersonTypeModel"/>, where each model
    /// represents metadata for person types based on the specified criteria.
    /// </returns>
    /// <example>
    /// <code>
    /// var personTypeMetadata = await bookingDataPersonTypeRepository.GetAllBookingMetaPersonTypeModelsAsync(
    ///     facilityId: 1,
    ///     siteId: 2,
    ///     planIds: new long[] { 101, 102 },
    ///     roomGroupIds: new long[] { 201, 202 },
    ///     facilityStateUseSpaTax: true,
    ///     useCache: true,
    ///     cancellationToken: CancellationToken.None
    /// );
    /// </code>
    /// </example>
    Task<IEnumerable<BookingMetaPersonTypeModel>> GetAllBookingMetaPersonTypeModelsAsync(
        long facilityId,
        long siteId,
        long[] planIds,
        long[] roomGroupIds,
        bool facilityStateUseSpaTax,
        bool useCache,
        CancellationToken cancellationToken
    );
}
