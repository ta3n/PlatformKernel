using Liberty.GmoPaymentGateway.Models.Requests;
using Liberty.GmoPaymentGateway.Services;
using Liberty.Reservation.Application.Constants;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Application.Cqrs.BaseQueries;
using Liberty.Reservation.Application.Domains.Services;
using Liberty.Reservation.Application.Exceptions;
using Liberty.Reservation.User.Application.Auth;
using Liberty.Reservation.User.Application.Domains.Repositories.Interfaces;
using Liberty.Reservation.User.Application.Exceptions;
using Liberty.Reservation.User.WebAPI.Application.Models.Responses;
using Liberty.Reservation.User.WebAPI.Application.Settings;
using Microsoft.Extensions.Options;

namespace Liberty.Reservation.User.WebAPI.Application.UserCases.Queries.BookingReservation;

public class ReservationGmoPaymentQueryHandler(
    IMapper mapper,
    ISecurityContextAccessor securityContextAccessor,
    IReservationRepository reservationRepository,
    IGmoPaymentGatewayService gmoPaymentGatewayService,
    IOptions<GMOPaymentSetting> gmoPaymentSetting,
    ISystemConfigRepository systemConfigRepository
) : QuerySingleBaseHandler<ReservationGmoPaymentQuery, GmoPaymentResponse>(mapper)
{
    protected override async Task<(IHeaderDictionary, GmoPaymentResponse)> HandleAsync(
        ReservationGmoPaymentQuery request,
        CancellationToken cancellationToken
    )
    {
        var userCode = securityContextAccessor.ApplicationUserKey;
        var paymentSetting = gmoPaymentSetting.Value;

        var globalCanOnlinePayment = await systemConfigRepository
                .GetQueryableWithAsNoTracking()
                .Select(x => x.CanOnlinePayment)
                .SingleOrDefaultAsync(cancellationToken)
            ?? throw new SystemConfigNotfoundException();

        if (!globalCanOnlinePayment)
        {
            throw new GlobalCanOnlinePaymentNotAllowException();
        }

        var queryable = reservationRepository
            .GetQueryableWithAsNoTracking();

        var queryableOrderOfReservation = queryable
            .Where(x => x.Id == request.Id)
            .Where(x => x.UserCode == userCode)
            .Where(x => x.Facility!.CanOnLinePayment)
            .Where(x => x.Facility!.IsOnLinePayment)
            .Where(x => x.Plan!.IsOnLinePayment)
            .Where(
                x =>
                    (
                        (x.ReservationState == ReservationStatus.Reserved || x.ReservationState == ReservationStatus.Modified)
                        && x.PaymentType == PaymentTypes.OnSidePayment
                    )
                    || (
                        x.ReservationState == ReservationStatus.Temporary && x.PaymentType == PaymentTypes.OnLinePayment
                    )
            )
            .Select(
                x =>
                    new OrderOfReservationResponse(
                        x.OrderReservations!.FirstOrDefault()!.Order!.ApiIssueCode,
                        x.OrderReservations!.FirstOrDefault()!.Order!.OrderDateTime,
                        x.BookingData!.AllTotalPrice,
                        x.BookingData!.TotalSpaTax,
                        x.BookingData!.LanguageCode,
                        x.CheckInDate,
                        x.BookingData!.Facility.TimeZone
                    )
            );

        var orderOfReservation = await queryableOrderOfReservation.SingleOrDefaultAsync(cancellationToken)
            ?? throw new ReservationNotfoundException();

        var checkInDate = AppDate.GetDateTime(orderOfReservation.CheckInDate);

        if (!BookingCheckAvailableService.IsWithinOnlinePaymentLimit(checkInDate, orderOfReservation.TimeZone))
        {
            throw new OnlinePaymentNotAllowedException();
        }

        var requestPaymentGetUrl = new PaymentGetUrlRequest(
            orderOfReservation.OrderId,
            Convert.ToInt32(orderOfReservation.Amount),
            paymentSetting.Tax,
            orderOfReservation.LanguageCode
        );

        var url = await gmoPaymentGatewayService.GetPaymentUrlAsync(requestPaymentGetUrl, cancellationToken);

        var response = new GmoPaymentResponse(
            orderOfReservation.OrderId,
            orderOfReservation.OrderTime!.Value.ToString(paymentSetting.OrderDateFormat),
            orderOfReservation.Amount,
            orderOfReservation.Tax,
            url
        );

        return (new HeaderDictionary(), response);
    }
}
