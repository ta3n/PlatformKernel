using Liberty.Reservation.Manager.Application.Models;

namespace Liberty.Reservation.Manager.Application.Domains.Services;

public class BookingPriceDataService : IBookingPriceDataService
{
    public BookingPriceTotalModel GetTotalPrice(
        IEnumerable<BookingReservationPriceData> reservationPriceData,
        IEnumerable<BookingReservationOptionItemData> reservationOptionItemData
    )
    {
        var bookingReservationPriceData = reservationPriceData as BookingReservationPriceData[] ?? [];

        var totalRoomGroupPrice = bookingReservationPriceData.Sum(
            x => x.TotalPrice
        );

        var totalSpaTax = bookingReservationPriceData.Sum(
            x => x.TotalSpaTax
        );

        var totalOptionItemPrice = reservationOptionItemData.Sum(
            x => x.TotalPrice
        );

        var data = new BookingPriceTotalModel
        {
            TotalRoomGroupPrice = totalRoomGroupPrice ?? 0,
            TotalSpaTax = totalSpaTax ?? 0,
            TotalOptionItemPrice = totalOptionItemPrice ?? 0,
            UsedPoint = 0,
            IncomePoint = 0,
            RemainPoint = 0
        };

        return data;
    }
}
