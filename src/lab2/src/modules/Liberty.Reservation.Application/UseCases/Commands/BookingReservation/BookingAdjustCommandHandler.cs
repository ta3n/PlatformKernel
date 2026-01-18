using AutoMapper;
using Liberty.Reservation.Application.Constants;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Metas;
using Liberty.Reservation.Application.Cqrs.BaseCommands;
using Liberty.Reservation.Application.Domains.Services.Interfaces;
using Liberty.Reservation.Application.Exceptions;
using Liberty.Reservation.Application.Models.Requests;
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

        var questionExists = existingReservation.BookingData?.GetAllQuestions().DistinctBy(x => x.Id).ToList() ?? [];
        var planQuestions = payload.PlanQuestions?.ToList() ?? [];
        var optionQuestions = payload.OptionsQuestions?.ToList() ?? [];

        // Build a dictionary for fast lookup of IsRequired by QuestionId
        if (
            questionExists.Count != 0 || planQuestions.Any() || optionQuestions.Any()
        )
        {
            BindIsRequiredToQuestions(planQuestions, optionQuestions, questionExists);

            var questionChecks = planQuestions.Concat(optionQuestions).ToList();
            if (questionExists.Count != 0 && questionChecks.Count != 0)
            {
                bookingCheckAvailableService.IsQuestionsRequiredValidAll(
                    questionChecks,
                    questionExists
                );
            }
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

    private static void BindIsRequiredToQuestions(
        IEnumerable<QuestionOfBookingCreateRequest> planQuestions,
        IEnumerable<QuestionOfBookingCreateRequest> optionQuestions,
        IEnumerable<QuestionData> existingQuestions
    )
    {
        var requiredMap = existingQuestions.ToDictionary(q => q.Id ?? 0, q => q.IsRequired);

        var allQuestions = planQuestions.Concat(optionQuestions);

        foreach (var item in allQuestions)
        {
            if (requiredMap.TryGetValue(item.QuestionId, out var isRequired))
            {
                item.IsRequired = isRequired ?? true;
            }
        }
    }
}
