using Liberty.Cache.Services;
using Liberty.Reservation.Application.Constants;
using Liberty.Reservation.Application.Cqrs.BaseCommands;
using Liberty.Reservation.Application.Domains.Services.Interfaces;
using Liberty.Reservation.User.Application.Auth;
using Liberty.UnitOfWork.Abstractions;
using Microsoft.Extensions.Logging;

namespace Liberty.Reservation.User.WebAPI.Application.UserCases.Commands.BookingReservation;

public class BookingChangeLocationCommandHandler(
    ILogger<BookingChangeExecutionCommandHandler> logger,
    IUnitOfWork unitOfWork,
    IMapper mapper,
    ISecurityContextAccessor securityContextAccessor,
    ICacheService cacheService,
    IBookingReservationService bookingReservationService,
    IBookingCheckAvailableService bookingCheckAvailableService
) : UpdateCommandHandlerBase<BookingChangeLocationCommand, long>(unitOfWork, mapper)
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
            await bookingReservationService.ChangeLocationAsync(existingReservation, cancellationToken: cancellationToken);
            await cacheService.SetAsync(cacheKey, request.Payload.IsSiteLocation, cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Online payment location error: {Message}", ex.Message);
        }

        return existingReservation.Id;
    }
}
