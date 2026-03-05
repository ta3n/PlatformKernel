using Liberty.Reservation.Application.Models.Requests;
using Liberty.Reservation.Manager.Application.Models;

namespace Liberty.Reservation.Manager.Application.Domains.Services.Interfaces;

public interface IBookingRoomAppDateService
{
    Task<IEnumerable<BookingRoomAppDateModel>> GetAllRoomAppDates(
        BookingCreateRequest bookingCreateRequest,
        BookingDataAvailableModel bookingDataAvailable,
        CancellationToken cancellationToken = default
    );
}
