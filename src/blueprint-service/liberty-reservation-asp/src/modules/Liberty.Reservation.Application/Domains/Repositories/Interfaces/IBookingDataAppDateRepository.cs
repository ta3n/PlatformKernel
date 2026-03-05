using Liberty.Reservation.Application.Models;
using Liberty.Reservation.Application.Models.Requests;

namespace Liberty.Reservation.Application.Domains.Repositories.Interfaces;

/// <summary>
/// Interface for accessing booking data related to application dates.
/// Provides methods to retrieve booking metadata for rooms and plans.
/// </summary>
public interface IBookingDataAppDateRepository
{
    /// <summary>
    /// Retrieves all booking metadata for room application dates.
    /// </summary>
    /// <param name="facilityId">
    /// The ID of the facility to filter the results.
    /// </param>
    /// <param name="siteId">
    /// The ID of the site to filter the results.
    /// </param>
    /// <param name="roomGroupIds">
    /// An array of room group IDs to filter the results.
    /// </param>
    /// <param name="search">
    /// The search criteria for booking plans, including filters such as dates and guest numbers.
    /// </param>
    /// <param name="cancellationToken">
    /// A token to monitor for cancellation requests, allowing the operation to be canceled if needed.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains
    /// an enumerable collection of <see cref="BookingMetaRoomAppDateModel"/>, where each model
    /// represents metadata for room application dates based on the specified criteria.
    /// </returns>
    /// <example>
    /// <code>
    /// var roomAppDateMetadata = await bookingDataAppDateRepository.GetAllBookingMetaRoomAppDateModelsAsync(
    ///     facilityId: 1,
    ///     siteId: 2,
    ///     roomGroupIds: new long[] { 201, 202 },
    ///     search: new BookingSearchPlanRequest { /* filters */ },
    ///     cancellationToken: CancellationToken.None
    /// );
    /// </code>
    /// </example>
    Task<IEnumerable<BookingMetaRoomAppDateModel>> GetAllBookingMetaRoomAppDateModelsAsync(
        long facilityId,
        long siteId,
        long[] roomGroupIds,
        BookingSearchPlanRequest search,
        CancellationToken cancellationToken
    );

    /// <summary>
    /// Retrieves all booking metadata for plan application dates.
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
    /// <param name="search">
    /// The search criteria for booking plans, including filters such as dates and guest numbers.
    /// </param>
    /// <param name="cancellationToken">
    /// A token to monitor for cancellation requests, allowing the operation to be canceled if needed.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains
    /// an enumerable collection of <see cref="BookingMetaPlanAppDateModel"/>, where each model
    /// represents metadata for plan application dates based on the specified criteria.
    /// </returns>
    /// <example>
    /// <code>
    /// var planAppDateMetadata = await bookingDataAppDateRepository.GetAllBookingMetaPlanAppDateModelsAsync(
    ///     facilityId: 1,
    ///     siteId: 2,
    ///     planIds: new long[] { 101, 102 },
    ///     roomGroupIds: new long[] { 201, 202 },
    ///     search: new BookingSearchPlanRequest { /* filters */ },
    ///     cancellationToken: CancellationToken.None
    /// );
    /// </code>
    /// </example>
    Task<IEnumerable<BookingMetaPlanAppDateModel>> GetAllBookingMetaPlanAppDateModelsAsync(
        long facilityId,
        long siteId,
        long[] planIds,
        long[] roomGroupIds,
        BookingSearchPlanRequest search,
        CancellationToken cancellationToken
    );
}
