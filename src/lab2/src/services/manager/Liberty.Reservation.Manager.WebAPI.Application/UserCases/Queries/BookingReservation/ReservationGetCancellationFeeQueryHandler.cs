using Liberty.Reservation.Application.Cqrs.BaseQueries;
using Liberty.Reservation.Application.Domains.Services.Interfaces;
using Liberty.Reservation.Application.Models.Responses;
using Liberty.Reservation.Manager.Application.Auth;

namespace Liberty.Reservation.Manager.WebAPI.Application.UserCases.Queries.BookingReservation;

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
        var facilityId = securityContextAccessor.FacilityKey;
        var timeZoneOffset = bookingReservationService.GetFacilityTimeZoneById(facilityId);
        var existingReservation = await bookingCheckAvailableService.GetReservationByFacilityAsync(
            reservationId,
            facilityId,
            cancellationToken
        );

        var cancelledDateTime = DateTime.UtcNow.Add(timeZoneOffset);
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
