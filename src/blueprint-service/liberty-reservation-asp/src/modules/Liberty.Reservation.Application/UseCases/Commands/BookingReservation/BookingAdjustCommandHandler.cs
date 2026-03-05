using AutoMapper;
using Liberty.Reservation.Application.Constants;
using Liberty.Reservation.Application.Cqrs.BaseCommands;
using Liberty.Reservation.Application.Domains.Services.Interfaces;
using Liberty.Reservation.Application.Exceptions;
using Liberty.UnitOfWork.Abstractions;
using MediatR;

namespace Liberty.Reservation.Application.UseCases.Commands.BookingReservation;

public class BookingAdjustCommandHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IMediator mediator,
    IBookingCheckAvailableService bookingCheckAvailableService
) : UpdateCommandHandlerBase<BookingAdjustCommand, Contexts.DataContexts.Entities.Data.Reservation>(unitOfWork, mapper)
{
    protected override async Task<Contexts.DataContexts.Entities.Data.Reservation> HandleAsync(
        BookingAdjustCommand request,
        CancellationToken cancellationToken
    )
    {
        var payload = request.Payload;
        var existingReservation = request.ExistingReservation;

        payload.CheckInDateId = existingReservation.CheckInDate;
        var bookingValid = await bookingCheckAvailableService.CheckAdjustAvailableAsync(
            existingReservation.FacilityId,
            existingReservation.PlanId,
            payload,
            cancellationToken
        );
        if (!bookingValid)
        {
            throw new ReservationInvalidException("Booking invalid");
        }

        var newReservation = request.IsModifyInPrice
            ? await mediator.Send(
                new BookingAdjustPriceCommand(
                    existingReservation,
                    request.ReservationState
                )
                {
                    Payload = payload,
                    IsNotCheckValidDateLimit = request.IsNotCheckValidDateLimit
                },
                cancellationToken
            )
            : await mediator.Send(
                new BookingAdjustHeaderDataCommand(
                    ReservationStatus.Modified
                ) { Payload = payload },
                cancellationToken
            );

        return newReservation;
    }
}
