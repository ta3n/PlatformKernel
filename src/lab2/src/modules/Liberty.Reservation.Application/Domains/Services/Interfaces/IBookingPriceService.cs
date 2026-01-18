using Liberty.Reservation.Application.Models;
using Liberty.Reservation.Application.Models.Requests;
using Liberty.Reservation.Application.Models.Responses;

namespace Liberty.Reservation.Application.Domains.Services.Interfaces;

/// <summary>
/// Defines an abstraction for calculating booking prices based on specific request parameters,
/// booking plans, and room group identifiers.
/// </summary>
public interface IBookingPriceService
{
    /// <summary>
    /// Calculates the booking price based on the provided request parameters, booking plan,
    /// and room group identifier. Optionally, an exception can be thrown if the calculation fails.
    /// </summary>
    /// <param name="bookingPriceRequest">
    /// The request object containing details such as dates, number of guests, and other parameters
    /// required for price calculation.
    /// </param>
    /// <param name="plan">
    /// The booking plan model that defines pricing rules and conditions.
    /// </param>
    /// <param name="roomGroupId">
    /// The identifier of the room group for which the price is being calculated.
    /// </param>
    /// <param name="execThrowException">
    /// A boolean flag indicating whether an exception should be thrown if the calculation fails.
    /// Defaults to <c>true</c>.
    /// </param>
    /// <returns>
    /// A <see cref="BookingPriceResponse"/> object containing the calculated price and any
    /// additional details related to the booking price.
    /// </returns>
    BookingPriceResponse? GetBookingPrice(
        BookingPriceRequest bookingPriceRequest,
        BookingPlanModel plan,
        long roomGroupId
    );
}
