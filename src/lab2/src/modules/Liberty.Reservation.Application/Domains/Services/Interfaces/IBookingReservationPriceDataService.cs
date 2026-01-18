using Liberty.Reservation.Application.Models;
using Liberty.Reservation.Application.Models.Requests;

namespace Liberty.Reservation.Application.Domains.Services.Interfaces;

public interface IBookingReservationPriceDataService
{
    /// <summary>
    /// Gets all reservation price data for the specified booking request, plan, and room app dates.
    /// </summary>
    /// <param name="bookingCreateRequest">The booking creation request.</param>
    /// <param name="bookingPlanAvailable">The available booking plan model.</param>
    /// <param name="bookingRoomAppDates">The collection of booking room app date models.</param>
    /// <returns>A collection of <see cref="BookingReservationPriceData"/>.</returns>
    /// <example>
    /// <code>
    /// var priceData = service.GetAllReservationPriceData(request, plan, roomAppDates);
    /// </code>
    /// </example>
    IEnumerable<BookingReservationPriceData> GetAllReservationPriceData(
        BookingCreateRequest bookingCreateRequest,
        BookingPlanModel bookingPlanAvailable,
        IEnumerable<BookingRoomAppDateModel> bookingRoomAppDates
    );
}
