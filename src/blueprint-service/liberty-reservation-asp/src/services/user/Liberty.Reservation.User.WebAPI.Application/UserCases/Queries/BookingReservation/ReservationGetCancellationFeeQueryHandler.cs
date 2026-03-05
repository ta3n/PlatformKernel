using Liberty.Reservation.Application.Constants;
using Liberty.Reservation.Application.Cqrs.BaseQueries;
using Liberty.Reservation.Application.Domains.Services.Interfaces;
using Liberty.Reservation.Application.Models.Responses;
using Liberty.Reservation.User.Application.Auth;

namespace Liberty.Reservation.User.WebAPI.Application.UserCases.Queries.BookingReservation;

public class ReservationGetCancellationFeeQueryHandler(
    IMapper mapper,
    ISecurityContextAccessor securityContextAccessor,
    IBookingReservationService bookingReservationService,
    IBookingCheckAvailableService bookingCheckAvailableService
) : QuerySingleBaseHandler<ReservationGetCancellationFeeQuery, BookingCancellationFeeResponse>(mapper)
{
    protected override async Task<(IHeaderDictionary, BookingCancellationFeeResponse)> HandleAsync(
        ReservationGetCancellationFeeQuery request,
        CancellationToken cancellationToken
    )
    {
        var reservationId = request.Id;
        var userCode = securityContextAccessor.ApplicationUserKey;

        var existingReservation = await bookingCheckAvailableService.GetReservationByUserAsync(
            reservationId,
            userCode,
            cancellationToken
        );

        var cancelledDateTime = DateTime.UtcNow.AddHours(DefaultValues.TimeZoneOffset);
        var cancellationPrice = bookingReservationService.GetBookingCancellationFee(
            cancelledDateTime,
            existingReservation
        );

        return (
            new HeaderDictionary(),
            cancellationPrice
        );
    }
}
