using Liberty.Reservation.Manager.Application.Models;

namespace Liberty.Reservation.Manager.Application.Domains.Services.Interfaces;

public interface IBookingPriceDataService
{
    BookingPriceTotalModel GetTotalPrice(
        IEnumerable<BookingReservationPriceData> reservationPriceData,
        IEnumerable<BookingReservationOptionItemData> reservationOptionItemData
    );
}
