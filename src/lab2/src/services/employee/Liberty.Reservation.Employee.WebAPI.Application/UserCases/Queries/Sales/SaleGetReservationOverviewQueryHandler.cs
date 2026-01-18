using Liberty.Reservation.Application.Cqrs.BaseQueries;
using Liberty.Reservation.Application.Models.Responses;
using Liberty.Reservation.Employee.Application.Domains.Repositories.Interfaces;
using Liberty.UnitOfWork.DbFunctions;
using Microsoft.EntityFrameworkCore;

namespace Liberty.Reservation.Employee.WebAPI.Application.UserCases.Queries.Sales;

public class SaleGetReservationOverviewQueryHandler(
    IMapper mapper,
    IReservationRepository reservationRepository
) : QuerySingleBaseHandler<SaleGetReservationOverviewQuery, SaleOverviewResponse>(mapper)
{
    protected override async Task<(IHeaderDictionary, SaleOverviewResponse)> HandleAsync(
        SaleGetReservationOverviewQuery request,
        CancellationToken cancellationToken
    )
    {
        var payload = request.Request;
        var reservations = await reservationRepository
            .GetQueryableWithAsNoTracking()
            .Where(x => x.ReservationState != ReservationStatus.Temporary)
            .Where(
                x =>
                x.CheckInDate
                    .GetDateTime(x.CheckInTime)
                    .AddHourOffset(x.Facility!.TimeZone) >= payload.StartAppDateId.GetDateTime(new TimeSpan(0, 0, 0))
            )
            .Where(
                x =>
                x.CheckInDate
                    .GetDateTime(x.CheckInTime)
                    .AddHourOffset(x.Facility!.TimeZone) <= payload.EndAppDateId.GetDateTime(new TimeSpan(23, 59, 59))
            )
            .Where(x => payload.FacilityIds.Contains(x.FacilityId))
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
