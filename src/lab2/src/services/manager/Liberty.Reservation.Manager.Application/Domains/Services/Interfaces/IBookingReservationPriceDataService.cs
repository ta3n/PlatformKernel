using Liberty.Reservation.Application.Models.Requests;
using Liberty.Reservation.Manager.Application.Models;

namespace Liberty.Reservation.Manager.Application.Domains.Services.Interfaces;

public interface IBookingReservationPriceDataService
{
    IEnumerable<BookingReservationPriceData> GetAllReservationPriceData(
        BookingCreateRequest bookingCreateRequest,
        BookingDataAvailableModel bookingDataAvailable,
        IEnumerable<BookingRoomAppDateModel> bookingRoomAppDates
    );
}
