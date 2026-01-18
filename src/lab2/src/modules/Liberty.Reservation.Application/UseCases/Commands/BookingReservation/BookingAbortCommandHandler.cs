using AutoMapper;
using Liberty.Reservation.Application.Cqrs.BaseCommands;
using Liberty.Reservation.Application.Domains.Services.Interfaces;
using Liberty.Reservation.Application.Models.Responses;
using Liberty.SysException.Exceptions;
using Liberty.UnitOfWork.Abstractions;
using Microsoft.Extensions.Logging;
using ReservationEntity = Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data.Reservation;

namespace Liberty.Reservation.Application.UseCases.Commands.BookingReservation;

public class BookingAbortCommandHandler(
    ILogger<BookingAbortCommandHandler> logger,
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IBookingReservationService reservationService
) : UpdateCommandHandlerBase<BookingAbortCommand, BookingAbortResponse>(unitOfWork, mapper)
{
    protected override async Task<BookingAbortResponse> HandleAsync(
        BookingAbortCommand request,
        CancellationToken cancellationToken
    )
    {
        var payload = request.Payload;
        var cancelledDateTime = DateTime.UtcNow;

        try
        {
            var reservation = new ReservationEntity
            {
                Id = payload.Id ?? 0,
                CancelledDateTime = cancelledDateTime,
                ReservationState = request.ReservationState,
                CancellationFeeType = request.ExistingReservation.CancellationFeeType
            };

            var editReservation = await reservationService.CancelAsync(
                reservation,
                request.CancellationPrice,
                request.RateFee,
                false,
                cancellationToken
            );

            return new(
                editReservation.Id,
                request.CancellationPrice,
                cancelledDateTime,
                request.RateFee
            );
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Cancellation reservation failed: {Message}", ex.Message);
            await UnitOfWork.RollbackAsync(cancellationToken);
            throw new AppLibertyException(ex.Message, ex);
        }
    }
}
