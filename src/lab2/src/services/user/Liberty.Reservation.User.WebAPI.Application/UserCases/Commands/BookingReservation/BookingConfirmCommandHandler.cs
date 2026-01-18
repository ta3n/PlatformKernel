using Liberty.Reservation.Application.Constants;
using Liberty.Reservation.Application.Cqrs.BaseCommands;
using Liberty.Reservation.Application.Domains.Services.Interfaces;
using Liberty.Reservation.Application.Exceptions;
using Liberty.Reservation.Application.Models;
using Liberty.Reservation.User.Application.Auth;
using Liberty.Reservation.User.WebAPI.Application.BackgroundServices;
using Liberty.Reservation.User.WebAPI.Application.UserCases.Events.Booking;
using Liberty.SysException.Exceptions;
using Liberty.UnitOfWork.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ReservationEntity = Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data.Reservation;

namespace Liberty.Reservation.User.WebAPI.Application.UserCases.Commands.BookingReservation;

public class BookingConfirmCommandHandler(
    ILogger<BookingConfirmCommandHandler> logger,
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IMediator mediator,
    IServiceProvider serviceProvider
) : UpdateCommandWithAuditEventHandlerBase<BookingConfirmCommand, long>(unitOfWork, mapper, mediator)
{
    private readonly ISecurityContextAccessor _securityContextAccessor
        = serviceProvider.GetRequiredService<ISecurityContextAccessor>();

    private readonly IBookingHoldManagementService _bookingHoldManagementService
        = serviceProvider.GetRequiredService<IBookingHoldManagementService>();

    private readonly IBookingCheckAvailableService _bookingCheckAvailableService
        = serviceProvider.GetRequiredService<IBookingCheckAvailableService>();

    private readonly IBookingReservationService _bookingReservationService
        = serviceProvider.GetRequiredService<IBookingReservationService>();

    private readonly IBookingOptionInventoryService _bookingOptionInventoryService
        = serviceProvider.GetRequiredService<IBookingOptionInventoryService>();

    private readonly BookingConfirmSendEmailBackgroundService _bookingConfirmSendEmailBackgroundService
        = serviceProvider.GetRequiredService<BookingConfirmSendEmailBackgroundService>();

    protected override async Task<long> HandleAsync(
        BookingConfirmCommand request,
        CancellationToken cancellationToken
    )
    {
        var payload = request.Payload;
        var userCode = _securityContextAccessor.ApplicationUserKey;

        var existingReservation = await _bookingCheckAvailableService.GetReservationBasicByUserAsync(
            payload.Id ?? 0,
            userCode,
            cancellationToken
        );

        var optionInventoryModel = new OptionInventoryModel(
            existingReservation.FacilityId,
            existingReservation.SiteId,
            existingReservation.PlanId,
            existingReservation.CheckInDate,
            existingReservation.RestNumber,
            existingReservation.OptionOfBookingSearchModel,
            null
        );

        await _bookingOptionInventoryService.ValidateOptionInventoryAsync(
            optionInventoryModel,
            cancellationToken
        );

        if (!existingReservation.IsUnConfirmed)
        {
            throw new GuestHasAlreadyConfirmedBookingException();
        }

        var reservationConfirm = new ReservationEntity
        {
            Id = existingReservation.Id,
            ReservationState = ReservationStatus.Confirmed,
            ConfirmedDateTime = DateTime.UtcNow,
            PaymentType = PaymentTypes.OnSidePayment
        };

        var isBookingAvailable = await CheckBookingAvailabilityAsync(
            existingReservation,
            cancellationToken
        );
        if (!isBookingAvailable)
        {
            throw new ReservationNoRemainRoomNumberRestException();
        }

        var bookingHoldCheckModel = new BookingHoldCheckModel(
            existingReservation.FacilityId,
            existingReservation.SiteId,
            existingReservation.PlanId,
            existingReservation.RoomGroupId,
            existingReservation.CheckInDate,
            existingReservation.RestNumber,
            existingReservation.RoomNumber,
            userCode,
            existingReservation.Code ?? string.Empty,
            BookingHoldValues.DefaultHoldTimeInSeconds
        );

        try
        {
            var isRoomHeld = await _bookingHoldManagementService.TryHoldRoomAsync(
                bookingHoldCheckModel,
                cancellationToken
            );
            if (!isRoomHeld)
            {
                throw new ReservationNoRemainRoomNumberRestException();
            }

            await UnitOfWork.BeginTransactionAsync(cancellationToken: cancellationToken);

            var bookingResponse = await _bookingReservationService.ConfirmedAsync(
                reservationConfirm,
                false,
                cancellationToken
            );

            await UnitOfWork.CommitAsync(cancellationToken);

            AuditEventData = new BookingConfirmedEvent
            {
                Id = bookingResponse.Id,
                AggregateCode = bookingResponse.Code ?? string.Empty,
                Request = request,
                UserCode = null,
                FacilityId = bookingResponse.FacilityId,
                SiteId = bookingResponse.SiteId
            };

            if (bookingResponse.IsReserved)
            {
                await _bookingHoldManagementService.ReleaseHoldAsync(
                    bookingHoldCheckModel,
                    cancellationToken
                );
            }

            _ = Task.Run(
                async () =>
                {
                    try
                    {
                        await _bookingConfirmSendEmailBackgroundService.EnqueueEmailJobAsync(
                            new BookingConfirmSendEmailJob(
                                existingReservation.Id,
                                existingReservation.UserCode,
                                payload.GuestCode,
                                existingReservation.LanguageCode ?? _securityContextAccessor.GetLanguageCode()
                            ),
                            CancellationToken.None
                        );
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, "Failed to enqueue email job for booking {BookingId}", existingReservation.Id);
                    }
                },
                CancellationToken.None
            );

            return bookingResponse.Id;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Confirm reservation failed: {Message}", ex.Message);
            await UnitOfWork.RollbackAsync(cancellationToken);

            await _bookingHoldManagementService.ReleaseHoldAsync(
                bookingHoldCheckModel,
                cancellationToken
            );

            throw new AppLibertyException(ex.Message, ex);
        }
    }

    private async Task<bool> CheckBookingAvailabilityAsync(
        ReservationBasicModel reservation,
        CancellationToken cancellationToken = default
    )
    {
        var isNightNumber = await _bookingCheckAvailableService.IsNightNumberAsync(
            reservation.PlanId,
            reservation.RoomGroupId,
            reservation.SiteId,
            reservation.CheckInDate,
            reservation.RestNumber,
            cancellationToken
        );

        var isRoomNumber = await _bookingCheckAvailableService.IsRoomNumberAsync(
            reservation.PlanId,
            reservation.RoomGroupId,
            reservation.CheckInDate,
            reservation.RoomNumber,
            reservation.RestNumber,
            0,
            cancellationToken
        );

        return isNightNumber && isRoomNumber;
    }
}
