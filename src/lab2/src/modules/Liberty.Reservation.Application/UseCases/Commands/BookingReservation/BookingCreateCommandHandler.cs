using System;
using System.Globalization;
using AutoMapper;
using Liberty.ApplicationShared.Utils;
using Liberty.Reservation.Application.Constants;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;
using Liberty.Reservation.Application.Cqrs.BaseCommands;
using Liberty.Reservation.Application.Domains.Services.Interfaces;
using Liberty.Reservation.Application.Exceptions;
using Liberty.SysException.Exceptions;
using Liberty.UnitOfWork.Abstractions;
using Microsoft.Extensions.Logging;
using OrderEntity = Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data.Order;
using ReservationEntity = Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data.Reservation;

namespace Liberty.Reservation.Application.UseCases.Commands.BookingReservation;

public class BookingCreateCommandHandler(
    ILogger<BookingCreateCommandHandler> logger,
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IBookingCreateService bookingCreateService,
    IOrderBookingService orderBookingService,
    IBookingCheckAvailableService bookingCheckAvailableService
) : CreateCommandHandlerBase<BookingCreateCommand, ReservationEntity>(unitOfWork, mapper)
{
    private const string ApiIssueCodeFormat = "LPY{0:D10}DxxxxxxxAAAAAA";

    protected override async Task<ReservationEntity> HandleAsync(
        BookingCreateCommand request,
        CancellationToken cancellationToken
    )
    {
        var payload = request.Payload;

        logger.LogInformation(
            "{Action} - Booking start create new {Facility} {Site} {Plan} {RoomGroup} {BookingDate} ",
            nameof(BookingCreateCommandHandler),
            payload.FacilityId,
            payload.SiteId,
            payload.PlanId,
            payload.RoomGroupId,
            payload.BookingDate
        );

        var newReservation = await bookingCreateService.CreateBookingAsync(
            payload,
            request.ExternalInfo,
            null,
            cancellationToken
        );

        if (newReservation.PaymentType is PaymentTypes.OnLinePayment)
        {
            var isValidOnlinePaymentDate = CheckBookingPaymentOnlineDateAsync(newReservation);

            if (!isValidOnlinePaymentDate)
            {
                throw new OnlinePaymentNotAllowedException();
            }
        }

        var isBookingAvailable = await CheckBookingAvailabilityAsync(
            newReservation,
            cancellationToken
        );
        if (!isBookingAvailable)
        {
            throw new ReservationNoRemainRoomNumberRestException();
        }

        var selectedQuestions = request.ExternalInfo.SelectedQuestions ?? [];
        var planRequest = payload.PlanQuestions ?? [];
        var optionRequest = payload.OptionsQuestions ?? [];
        var questions = planRequest.Concat(optionRequest).ToList();
        if (questions.Count != 0 && selectedQuestions.Any())
        {
            bookingCheckAvailableService.IsQuestionsRequiredValidAll(
                questions,
                selectedQuestions
            );
        }

        try
        {
            await UnitOfWork.BeginTransactionAsync(cancellationToken: cancellationToken);

            var orderCode = EntityUtil.CreateCode();
            var newOrder = new OrderEntity
            {
                Code = orderCode,
                OrderDateTime = DateTime.UtcNow,
                ApiIssueCode = ConvertUtil.Format(ApiIssueCodeFormat, orderCode[..10]),
                IsEnabled = true
            };

            var newReservationOfOrder = await orderBookingService.CreateAsync(
                new OrderReservation
                {
                    Order = newOrder,
                    Reservation = newReservation,
                    IsEnabled = true
                },
                false,
                cancellationToken
            );

            await UnitOfWork.CommitAsync(cancellationToken);

            logger.LogInformation(
                "Booking successfully created new - {Action}",
                new
                {
                    Action = nameof(BookingCreateCommandHandler),
                    payload.FacilityId,
                    payload.SiteId,
                    payload.PlanId,
                    payload.RoomGroupId,
                    payload.BookingDate,
                    newReservationOfOrder.Reservation?.Code
                }
            );

            return newReservationOfOrder.Reservation!;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Creating a booking failed: {Message}", ex.Message);
            await UnitOfWork.RollbackAsync(cancellationToken);
            throw new AppLibertyException(ex.Message, ex);
        }
    }

    private async Task<bool> CheckBookingAvailabilityAsync(
        ReservationEntity reservation,
        CancellationToken cancellationToken = default
    )
    {
        var isReceptionAvailableTask = bookingCheckAvailableService.IsReceptionAvailableAsync(
            reservation.PlanId,
            reservation.CheckInDate,
            cancellationToken
        );

        var isNightNumberTask = bookingCheckAvailableService.IsNightNumberAsync(
            reservation.PlanId,
            reservation.RoomGroupId,
            reservation.SiteId,
            reservation.CheckInDate,
            reservation.RestNumber,
            cancellationToken
        );

        var isRoomNumberTask = bookingCheckAvailableService.IsRoomNumberAsync(
            reservation.PlanId,
            reservation.RoomGroupId,
            reservation.CheckInDate,
            reservation.RoomNumber,
            reservation.RestNumber,
            0,
            cancellationToken
        );

        await Task.WhenAll(
            isReceptionAvailableTask,
            isNightNumberTask,
            isRoomNumberTask
        );

        var isNightNumber = isNightNumberTask.Result;
        var isRoomNumber = isRoomNumberTask.Result;
        var isReceptionAvailable = isReceptionAvailableTask.Result;

        logger.LogInformation(
            "Booking check available - {Action} ",
            new
            {
                Action = nameof(BookingCreateCommandHandler),
                Facility = reservation.FacilityId,
                Site = reservation.SiteId,
                Plan = reservation.PlanId,
                RoomGroup = reservation.RoomGroupId,
                BookingDate = reservation.CheckInDate,
                reservation.RoomNumber,
                reservation.RestNumber,
                IsNightNumber = isNightNumber,
                IsRoomNumber = isRoomNumber,
                IsReceptionAvailable = isReceptionAvailable
            }
        );

        return isNightNumber && isRoomNumber && isReceptionAvailable;
    }

    private static bool CheckBookingPaymentOnlineDateAsync(
        ReservationEntity reservation
    )
    {
        var timeZone = reservation.BookingData!.Facility!.TimeZone;

        var offset = TimeSpan.TryParse(timeZone, CultureInfo.InvariantCulture, out var parsed)
            ? parsed
            : DefaultValues.DefaultTimeZoneOffset;

        var checkInDate = AppDate.GetDateTime(reservation.CheckInDate).Date;
        var now = DateTime.UtcNow.Add(offset).Date;

        return checkInDate.Date <= now.AddDays(DefaultValues.OnlinePaymentDayLimit);
    }
}
