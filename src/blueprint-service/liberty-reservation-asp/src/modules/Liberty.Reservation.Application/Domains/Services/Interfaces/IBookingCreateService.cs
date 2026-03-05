using Liberty.Reservation.Application.Models.Requests;
using ReservationEntity = Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data.Reservation;

namespace Liberty.Reservation.Application.Domains.Services.Interfaces;

/// <summary>
/// Represents a service responsible for creating booking reservations.
/// </summary>
public interface IBookingCreateService
{
    /// <summary>
    /// Creates a new booking reservation based on the provided details.
    /// </summary>
    /// <param name="bookingCreateRequest">The request containing details for creating the booking, such as facility, site, plan, and room group information.</param>
    /// <param name="bookingExternalInfoRequest">Additional external information related to the booking, including user code and selected questions.</param>
    /// <param name="existingReservation">An existing reservation entity, if applicable, to update or modify. Can be null for creating new reservations.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete. Defaults to its default value if not provided.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the created or updated reservation entity.</returns>
    Task<ReservationEntity> CreateBookingAsync(
        BookingCreateRequest bookingCreateRequest,
        BookingExternalInfoRequest bookingExternalInfoRequest,
        ReservationEntity? existingReservation,
        CancellationToken cancellationToken = default
    );
}
