using Liberty.Reservation.Application.Models;
using Liberty.Reservation.Application.Models.Requests;

namespace Liberty.Reservation.Application.Domains.Repositories.Interfaces;

/// <summary>
/// Interface for accessing booking data related to prices.
/// Provides methods to retrieve standard prices, booking metadata for prices, and discount data.
/// </summary>
public interface IBookingDataPriceRepository
{
    /// <summary>
    /// Retrieves all standard price models for the specified facility, site, plans, and room groups.
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
    /// <param name="useCache">
    /// A boolean indicating whether to use cached data for the operation.
    /// </param>
    /// <param name="cancellationToken">
    /// A token to monitor for cancellation requests.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains
    /// an enumerable collection of <see cref="BookingMetaStandardPriceModel"/>.
    /// </returns>
    /// <example>
    /// <code>
    /// var standardPrices = await bookingDataPriceRepository.GetAllStandardPriceModelsAsync(
    ///     facilityId: 1,
    ///     siteId: 2,
    ///     planIds: new long[] { 101, 102 },
    ///     roomGroupIds: new long[] { 201, 202 },
    ///     useCache: true,
    ///     cancellationToken: CancellationToken.None
    /// );
    /// </code>
    /// </example>
    Task<IEnumerable<BookingMetaStandardPriceModel>> GetAllStandardPriceModelsAsync(
        long facilityId,
        long siteId,
        long[] planIds,
        long[] roomGroupIds,
        bool useCache,
        CancellationToken cancellationToken
    );

    /// <summary>
    /// Retrieves all booking metadata for price data based on the specified criteria.
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
    /// an enumerable collection of <see cref="BookingMetaPriceDataModel"/>, where each model
    /// represents metadata for price data based on the specified criteria.
    /// </returns>
    /// <example>
    /// <code>
    /// var priceData = await bookingDataPriceRepository.GetAllBookingMetaPriceDataModelsAsync(
    ///     facilityId: 1,
    ///     siteId: 2,
    ///     planIds: new long[] { 101, 102 },
    ///     roomGroupIds: new long[] { 201, 202 },
    ///     search: new BookingSearchPlanRequest { /* filters */ },
    ///     standardPriceData: standardPrices,
    ///     cancellationToken: CancellationToken.None
    /// );
    /// </code>
    /// </example>
    Task<IEnumerable<BookingMetaPriceDataModel>> GetAllBookingMetaPriceDataModelsAsync(
        long facilityId,
        long siteId,
        long[] planIds,
        long[] roomGroupIds,
        BookingSearchPlanRequest search,
        CancellationToken cancellationToken
    );

    /// <summary>
    /// Retrieves all booking metadata for discount data based on the specified criteria.
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
    /// <param name="useCache">
    /// A boolean indicating whether to use cached data for the operation.
    /// </param>
    /// <param name="cancellationToken">
    /// A token to monitor for cancellation requests, allowing the operation to be canceled if needed.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains
    /// an enumerable collection of <see cref="BookingMetaDiscountDataModel"/>, where each model
    /// represents metadata for discount data based on the specified criteria.
    /// </returns>
    /// <example>
    /// <code>
    /// var discountData = await bookingDataPriceRepository.GetAllBookingMetaDiscountDataModelsAsync(
    ///     facilityId: 1,
    ///     siteId: 2,
    ///     planIds: new long[] { 101, 102 },
    ///     roomGroupIds: new long[] { 201, 202 },
    ///     useCache: true,
    ///     cancellationToken: CancellationToken.None
    /// );
    /// </code>
    /// </example>
    Task<IEnumerable<BookingMetaDiscountDataModel>> GetAllBookingMetaDiscountDataModelsAsync(
        long facilityId,
        long siteId,
        long[] planIds,
        long[] roomGroupIds,
        bool useCache,
        CancellationToken cancellationToken
    );

    /// <summary>
    /// Retrieves all range person models for the specified facility, site, plans, and room groups.
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
    /// <param name="useCache">
    /// A boolean indicating whether to use cached data for the operation.
    /// </param>
    /// <param name="cancellationToken">
    /// A token to monitor for cancellation requests.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains
    /// an enumerable collection of <see cref="BookingMetaRangePersonModel"/>.
    /// </returns>
    /// <example>
    /// <code>
    /// var priceData = await bookingDataPriceRepository.GetAllRangePersonModelsAsync(
    ///     facilityId: 1,
    ///     siteId: 2,
    ///     planIds: new long[] { 101, 102 },
    ///     roomGroupIds: new long[] { 201, 202 },
    ///     true,
    ///     cancellationToken: CancellationToken.None
    /// );
    /// </code>
    /// </example>
    Task<IEnumerable<BookingMetaRangePersonModel>> GetAllRangePersonModelsAsync(
        long facilityId,
        long siteId,
        long[] planIds,
        long[] roomGroupIds,
        bool useCache,
        CancellationToken cancellationToken
    );
}
