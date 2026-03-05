using AutoMapper;
using Liberty.GmoPaymentGateway.Models.Requests;
using Liberty.GmoPaymentGateway.Services;
using Liberty.Reservation.Application.Cqrs.BaseQueries;
using Liberty.Reservation.Application.Domains.Services.Interfaces;
using Liberty.Reservation.Application.Models.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Liberty.Reservation.Application.UseCases.Queries.BookingReservation;

public class GmoSearchTradeQueryHandler(
    IMapper mapper,
    ILogger<GmoSearchTradeQueryHandler> logger,
    IGmoPaymentGatewayService gmoPaymentGatewayService,
    IBookingReservationService bookingReservationService
) : QuerySingleBaseHandler<GmoSearchTradeQuery, SearchTradeResponse>(mapper)
{
    protected override async Task<(IHeaderDictionary, SearchTradeResponse)> HandleAsync(
        GmoSearchTradeQuery request,
        CancellationToken cancellationToken
    )
    {
        var orderId = await bookingReservationService.FindOderIdOfOnlinePaymentAsync(
            request.ReservationId,
            cancellationToken
        );

        logger.LogInformation(
            "{Action} - Start online payment change {ReservationId} {OderId}",
            nameof(GmoSearchTradeQueryHandler),
            request.ReservationId,
            orderId
        );

        var requestSearchTrade = new SearchTradeRequest(orderId);
        var responseSearchTrade = await gmoPaymentGatewayService.SearchTradeAsync(
            requestSearchTrade,
            cancellationToken
        );

        var response = new SearchTradeResponse
        {
            Amount = responseSearchTrade.Amount,
            ProcessDate = responseSearchTrade.ProcessDate,
            Status = responseSearchTrade.Status
        };

        return (new HeaderDictionary(), response);
    }
}
