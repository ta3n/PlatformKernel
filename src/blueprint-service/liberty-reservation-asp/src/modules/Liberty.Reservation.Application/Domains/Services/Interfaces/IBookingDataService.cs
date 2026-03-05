using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Metas;
using Liberty.Reservation.Application.Models;

namespace Liberty.Reservation.Application.Domains.Services.Interfaces;

/// <summary>
/// Represents a service interface for retrieving booking-related data
/// required in the reservation domain.
/// </summary>
public interface IBookingDataService
{
    /// <summary>
    /// Retrieves booking data based on the provided reservation, reservation price data, and selected questions.
    /// </summary>
    /// <param name="reservation">
    /// The reservation entity containing details of the reservation for which booking data is to be retrieved.
    /// </param>
    /// <param name="availableBookingPlan">
    /// The booking plan model that provides available booking options for the reservation.
    /// </param>
    /// <param name="reservationPriceData">
    /// A list of reservation price data objects that provide detailed pricing information.
    /// This parameter is optional and can be null.
    /// </param>
    /// <param name="selectedQuestions">
    /// A list of questions that were selected for the reservation.
    /// This parameter is optional and can be null.
    /// </param>
    /// <param name="cancellationToken">
    /// A cancellation token to observe while waiting for the task to complete. The default is <see cref="CancellationToken.None"/>.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation and contains the booking data associated with the provided reservation.
    /// </returns>
    /// <example>
    /// <code>
    /// var bookingData = await bookingDataService.GetBookingDataAsync(
    ///     reservation,
    ///     availableBookingPlan,
    ///     reservationPriceData,
    ///     selectedQuestions,
    ///     cancellationToken);
    /// </code>
    /// </example>
    Task<BookingData> GetBookingDataAsync(
        Contexts.DataContexts.Entities.Data.Reservation reservation,
        BookingPlanModel availableBookingPlan,
        List<BookingReservationPriceData>? reservationPriceData,
        List<Question>? selectedQuestions,
        BookingCancellationPolicyModel? bookingCancellationPolicyModel,
        CancellationToken cancellationToken = default
    );
}
