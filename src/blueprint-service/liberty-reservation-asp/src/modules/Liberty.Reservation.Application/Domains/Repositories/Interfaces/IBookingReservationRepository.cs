using Liberty.Pagination;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;
using Liberty.Reservation.Application.Models;
using Liberty.UnitOfWork.Abstractions;

namespace Liberty.Reservation.Application.Domains.Repositories.Interfaces;

/// <summary>
/// Repository interface for booking reservation domain operations.
/// Provides methods to retrieve reservation-related data such as person age types, option items, and booking histories.
/// </summary>
/// <example>
/// <code>
/// var repository = serviceProvider.GetRequiredService&lt;IBookingReservationRepository&gt;();
/// var ageTypes = await repository.GetAllPersonAgeTypesOfReservationAsync(facilityId, reservationId);
/// </code>
/// </example>
public interface IBookingReservationRepository : IRepositoryBase<Contexts.DataContexts.Entities.Data.Reservation>
{
    /// <summary>
    /// Retrieves all person age types associated with a specific reservation and facility.
    /// </summary>
    /// <param name="facilityId">The facility identifier.</param>
    /// <param name="reservationId">The reservation identifier.</param>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains a collection of <see cref="ReservationRoomGroupAppDatePersonAgeType"/> or <c>null</c>.
    /// </returns>
    Task<IEnumerable<ReservationRoomGroupAppDatePersonAgeType>?> GetAllPersonAgeTypesOfReservationAsync(
        long facilityId,
        long reservationId,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Retrieves all option items associated with a specific reservation and facility.
    /// </summary>
    /// <param name="facilityId">The facility identifier.</param>
    /// <param name="reservationId">The reservation identifier.</param>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains a collection of <see cref="ReservationRoomGroupAppDateOptionItem"/>.
    /// </returns>
    Task<IEnumerable<ReservationRoomGroupAppDateOptionItem>> GetAllOptionItemsOfReservationAsync(
        long facilityId,
        long reservationId,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Retrieves all booking histories for the specified booking identifiers.
    /// </summary>
    /// <param name="bookingIds">A collection of booking identifiers.</param>
    /// <param name="pageable">Pagination information.</param>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains a paged collection of <see cref="BookingHistoryDataModel"/>.
    /// </returns>
    Task<IPage<BookingHistoryDataModel>> GetAllHistoriesByMainAsync(
        IEnumerable<long> bookingIds,
        IPageable pageable,
        CancellationToken cancellationToken = default
    );
}
