using Liberty.Reservation.Application.Cqrs.BaseQueries;
using Liberty.Reservation.Manager.Application.Auth;

namespace Liberty.Reservation.Manager.WebAPI.Application.UserCases.Queries.Sale;

public class SaleGetReservationOverviewQueryHandler(
    IMapper mapper,
    ISecurityContextAccessor securityContextAccessor,
    IReservationRepository reservationRepository
) : QuerySingleBaseHandler<SaleGetReservationOverviewQuery, SaleOverviewResponse>(mapper)
{
    protected override async Task<(IHeaderDictionary, SaleOverviewResponse)> HandleAsync(
        SaleGetReservationOverviewQuery request,
        CancellationToken cancellationToken
    )
    {
        var facilityId = securityContextAccessor.FacilityKey;

        var reservations = await reservationRepository
            .GetQueryableWithAsNoTracking()
            .Where(x => x.Facility!.Id == facilityId)
            .Where(x => x.ReservationState != ReservationStatus.Temporary)
            .Where(x => x.CheckInDate >= request.StartAppDateId)
            .Where(x => x.CheckInDate <= request.EndAppDateId)
            .Select(
                x => new
                {
                    x.Id,
                    x.ReservationDateTime,
                    x.IsCancelled,
                    x.IsModified,
                    x.IsReserved,
                    x.BookingData,
                    x.ReservationState,
                    x.PaymentType,
                    IsGmoPaymentCompleteAll = x.IsGmoPaymentCompleteAll()
                }
            )
            .OrderByDescending(x => x.ReservationDateTime)
            .ToListAsync(cancellationToken);

        var totalCountOfModifiedPayments = reservations
            .Count(x => x.ReservationState is ReservationStatus.Modified);

        var totalCountOfCancelledPayments = reservations
            .Count(x => x.IsCancelled);

        var validStates = new[] { ReservationStatus.Confirmed, ReservationStatus.Reserved };
        var reservationSucceed = reservations
            .Where(x => validStates.Contains(x.ReservationState))
            .ToList();

        var onSidePayments = reservationSucceed
            .Where(x => x.PaymentType == PaymentTypes.OnSidePayment)
            .ToList();

        var onlinePayments = reservationSucceed
            .Where(x => x.PaymentType == PaymentTypes.OnLinePayment)
            .ToList();

        var reservationReserved = reservations
            .Where(x => x.IsReserved)
            .ToList();

        var totalOnSidePaymentPrice = reservationReserved
            .Where(x => x.PaymentType == PaymentTypes.OnSidePayment)
            .Select(x => x.BookingData!.AllTotalPrice)
            .Sum();

        var totalOnlinePaymentPrice = reservationReserved
            .Where(x => x.PaymentType == PaymentTypes.OnLinePayment)
            .Select(a => a.BookingData!.AllTotalPrice)
            .Sum();

        var totalPaymentPrice = totalOnSidePaymentPrice + totalOnlinePaymentPrice;

        var totalCountOfOnSidePayments = onSidePayments.Count;
        var totalCountOfOnlinePayments = onlinePayments.Count;

        var response = new SaleOverviewResponse(
            totalCountOfOnSidePayments,
            totalCountOfOnlinePayments,
            totalCountOfModifiedPayments,
            totalCountOfCancelledPayments,
            totalOnSidePaymentPrice,
            totalOnlinePaymentPrice,
            totalPaymentPrice,
            0,
            0,
            totalOnlinePaymentPrice
        );

        return (new HeaderDictionary(), response);
    }
}
