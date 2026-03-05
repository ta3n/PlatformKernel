using System.Globalization;
using Liberty.ApplicationShared.Utils;
using Liberty.GmoPaymentGateway.Models.Requests;
using Liberty.GmoPaymentGateway.Services;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Application.Domains.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace Liberty.Reservation.Application.Domains.Services;

public class BookingOnlinePaymentService(
    ILogger<BookingOnlinePaymentService> logger,
    IBookingReservationService bookingReservationService,
    IGmoChangeTranReportService gmoChangeTranReportService,
    IGmoPaymentGatewayService gmoPaymentGatewayService
) : IBookingOnlinePaymentService
{
    public async Task<(string? transactionId, string existingOrderId)> OnlinePaymentChangeAmountAsync(
        long reservationId,
        decimal totalPrice,
        bool isRollback = false,
        CancellationToken cancellationToken = default
    )
    {
        var apiIssueCode = await bookingReservationService.FindOderIdOfOnlinePaymentAsync(
            reservationId,
            cancellationToken
        );

        logger.LogInformation(
            "{Action} - Start online payment change {ReservationId} {OderId} {TotalAmount}",
            nameof(OnlinePaymentChangeAmountAsync),
            reservationId,
            apiIssueCode,
            totalPrice
        );

        var requestSearchTrade = new SearchTradeRequest(apiIssueCode);
        var responseSearchTrade = await gmoPaymentGatewayService.SearchTradeAsync(
            requestSearchTrade,
            cancellationToken
        );

        var requestChangeOrder = new ChangeOrderRequest(
            responseSearchTrade.OrderId!,
            responseSearchTrade.AccessId!,
            responseSearchTrade.AccessPass!,
            totalPrice.ToString(CultureInfo.CurrentCulture),
            responseSearchTrade.Tax!,
            responseSearchTrade.Method!,
            responseSearchTrade.PayTimes!
        ) { IsRollback = isRollback };
        var responseChangeOrder = await gmoPaymentGatewayService.ChangeOrderAsync(
            requestChangeOrder,
            cancellationToken
        );

        var report = new GmoChangeTranReport
        {
            Code = EntityUtil.CreateCode(),
            AccessId = responseSearchTrade.AccessId,
            ReservationId = reservationId,
            OrderId = apiIssueCode,
            Request = requestChangeOrder,
            Response = responseChangeOrder
        };

        await gmoChangeTranReportService.CreateAsync(
            report,
            false,
            cancellationToken
        );

        logger.LogInformation(
            "{Action} - Successfully online payment change amount {ReservationId} {OderId} {TranId}",
            nameof(OnlinePaymentChangeAmountAsync),
            reservationId,
            apiIssueCode,
            responseChangeOrder.TranId
        );

        return (responseChangeOrder.TranId, apiIssueCode);
    }
}
