using Liberty.Cache.Services;
using Liberty.Reservation.Application.Constants;
using Liberty.Reservation.Application.Cqrs.BaseCommands;
using Liberty.Reservation.Application.Domains.Services.Interfaces;
using Liberty.Reservation.User.Application.Auth;
using Liberty.Reservation.User.WebAPI.Application.UserCases.Events.Booking;
using Liberty.UnitOfWork.Abstractions;
using Microsoft.Extensions.Logging;

namespace Liberty.Reservation.User.WebAPI.Application.UserCases.Commands.BookingReservation;

public class BookingChangeLocationCommandHandler(
    ILogger<BookingChangeExecutionCommandHandler> logger,
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IMediator mediator,
    ISecurityContextAccessor securityContextAccessor,
    ICacheService cacheService,
    IBookingReservationService bookingReservationService,
    IBookingCheckAvailableService bookingCheckAvailableService
) : UpdateCommandWithAuditEventHandlerBase<BookingChangeLocationCommand, long>(unitOfWork, mapper, mediator)
{
    protected override async Task<long> HandleAsync(
        BookingChangeLocationCommand request,
        CancellationToken cancellationToken
    )
    {
        var userCode = securityContextAccessor.ApplicationUserKey;

        var cacheKey = string.Format(CacheKeys.UserLocationPrefixKey, userCode);
        var existingReservation = await bookingCheckAvailableService.GetReservationByCodeAsync(
            request.Payload.Code,
            userCode,
            cancellationToken
        );

        try
        {
            existingReservation.BookingData!.IsSiteLocation = request.Payload.IsSiteLocation;
            var reservation = await bookingReservationService.ChangeLocationAsync(
                existingReservation,
                cancellationToken: cancellationToken
            );
            await cacheService.SetAsync(cacheKey, request.Payload.IsSiteLocation, cancellationToken);

            AuditEventData = new BookingChangedLocalEvent
            {
                Id = reservation.Id,
                AggregateCode = reservation.Code ?? string.Empty,
                Request = request,
                UserCode = securityContextAccessor.ApplicationUserKey,
                FacilityId = reservation.FacilityId,
                SiteId = reservation.SiteId,
                OldId = null
            };
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Online payment location error: {Message}", ex.Message);
        }

        return existingReservation.Id;
    }
}
