using AutoMapper;
using Liberty.ApplicationShared.Utils;
using Liberty.Reservation.Application.Constants;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;
using Liberty.Reservation.Application.Cqrs.BaseCommands;
using Liberty.Reservation.Application.Domains.Services.Interfaces;
using Liberty.Reservation.Application.Models.Requests;
using Liberty.SysException.Exceptions;
using Liberty.UnitOfWork.Abstractions;
using Microsoft.Extensions.Logging;
using ReservationEntity = Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data.Reservation;

namespace Liberty.Reservation.Application.UseCases.Commands.BookingReservation;

public class BookingAdjustHeaderDataCommandHandler(
    ILogger<BookingAdjustHeaderDataCommandHandler> logger,
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IBookingReservationService reservationService
) : UpdateCommandHandlerBase<BookingAdjustHeaderDataCommand, ReservationEntity>(unitOfWork, mapper)
{
    protected override async Task<ReservationEntity> HandleAsync(
        BookingAdjustHeaderDataCommand request,
        CancellationToken cancellationToken
    )
    {
        var payload = request.Payload;

        var editHeaderDataOfReservation = Mapper.Map<ReservationEntity>(payload);
        editHeaderDataOfReservation.Memo = payload.FreeInput;
        editHeaderDataOfReservation.IsSameMainUser = payload.MainUser is null;
        editHeaderDataOfReservation.UseRoomUser = payload.RoomRepresentatives?.Any() ?? false;
        editHeaderDataOfReservation.ReservationState = request.ReservationState;
        editHeaderDataOfReservation.CheckInTime = ConvertUtil.ToNullableTimeSpan(request.Payload.CheckInTime);
        if (payload.RoomRepresentatives is not null)
        {
            editHeaderDataOfReservation.ReservationPlanRoomGroupAppDates =
            [
                .. payload.RoomRepresentatives
                    .Select(
                        x => new ReservationPlanRoomGroupAppDate
                        {
                            RoomGroupIndex = x.RoomIndex,
                            CustomerInfo = new CustomerInfo
                            {
                                Name = x.FullName,
                                Kana = x.Kana
                            }
                        }
                    )
            ];
        }

        editHeaderDataOfReservation = GetQuestions(
            payload,
            editHeaderDataOfReservation
        );

        try
        {
            var editedHeaderDataOfReservation = await reservationService.AdjustHeaderDataOfReservationAsync(
                editHeaderDataOfReservation,
                false,
                cancellationToken
            );

            return editedHeaderDataOfReservation;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Update header data of reservation failed: {Message}", ex.Message);
            await UnitOfWork.RollbackAsync(cancellationToken);
            throw new AppLibertyException(ex.Message, ex);
        }
    }

    private static ReservationEntity GetQuestions(
        BookingAdjustRequest bookingAdjustRequest,
        ReservationEntity editReservation
    )
    {
        editReservation.ReservationQuestions = [];

        foreach (var bookingQuestion in bookingAdjustRequest.PlanQuestions ?? [])
        {
            var planQuestion = new ReservationQuestion
            {
                QuestionId = bookingQuestion.QuestionId,
                AnswerData = bookingQuestion.AnswerData,
                ReservationQuestionType = ReservationQuestionTypes.Plan
            };

            editReservation.ReservationQuestions.Add(planQuestion);
        }

        foreach (var bookingQuestion in bookingAdjustRequest.OptionsQuestions ?? [])
        {
            var optionsQuestion = new ReservationQuestion
            {
                QuestionId = bookingQuestion.QuestionId,
                AnswerData = bookingQuestion.AnswerData,
                ReservationQuestionType = ReservationQuestionTypes.OptionItem
            };

            editReservation.ReservationQuestions.Add(optionsQuestion);
        }

        return editReservation;
    }
}
