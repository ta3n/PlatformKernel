using Liberty.Reservation.Application.Exceptions;
using Liberty.Reservation.Application.Models.Requests;
using ReservationEntity = Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data.Reservation;

namespace Liberty.Reservation.Manager.Application.Domains.Services;

public class BookingCheckAvailableService(
    IReservationService reservationService,
    IPersonAgeTypeService personAgeTypeService,
    IOptionItemService optionItemService
) : IBookingCheckAvailableService
{
    public async Task<ReservationEntity> GetReservationAsync(
        long reservationId,
        CancellationToken cancellationToken = default
    )
    {
        var existingReservation = await reservationService.FindByIdAsync(
            reservationId,
            cancellationToken
        );
        if (existingReservation is null)
        {
            throw new ReservationNotfoundException();
        }

        return existingReservation;
    }

    public async Task<bool> CheckAdjustAvailableAsync(
        BookingAdjustRequest adjustRequest,
        CancellationToken cancellationToken = default
    )
    {
        if (!adjustRequest.IsAgree)
        {
            return false;
        }

        var checks = new List<Func<BookingAdjustRequest, CancellationToken, Task<bool>>>
        {
            CheckAvailableNumberOfNightsAsync,
            CheckAvailableNumberOfRoomsAsync,
            CheckAvailableReserverAsync,
            CheckAvailableMainUserAsync,
            CheckAvailableRoomPeoplesAsync,
            CheckAvailableNightOptionsAsync,
            CheckAvailableRoomRepresentativesAsync
        };

        foreach (var check in checks)
        {
            if (!await check(adjustRequest, cancellationToken))
            {
                return false;
            }
        }

        return true;
    }

    public Task<bool> CheckAvailableNumberOfNightsAsync(
        BookingAdjustRequest adjustRequest,
        CancellationToken cancellationToken = default
    )
    {
        return Task.FromResult(true);
    }

    public Task<bool> CheckAvailableNumberOfRoomsAsync(
        BookingAdjustRequest adjustRequest,
        CancellationToken cancellationToken = default
    )
    {
        return Task.FromResult(true);
    }

    public Task<bool> CheckAvailableReserverAsync(
        BookingAdjustRequest adjustRequest,
        CancellationToken cancellationToken = default
    )
    {
        return Task.FromResult(true);
    }

    public Task<bool> CheckAvailableMainUserAsync(
        BookingAdjustRequest adjustRequest,
        CancellationToken cancellationToken = default
    )
    {
        return Task.FromResult(true);
    }

    public async Task<bool> CheckAvailableRoomPeoplesAsync(
        BookingAdjustRequest adjustRequest,
        CancellationToken cancellationToken = default
    )
    {
        var personAgeTypeIds = adjustRequest.GetPersonAgeTypeIds();

        var existingCount = await personAgeTypeService.CountByIdsAsync(
            personAgeTypeIds,
            cancellationToken
        );

        var isAvailable = existingCount == personAgeTypeIds.Length;
        if (!isAvailable)
        {
            throw new PersonAgeTypeNotfoundException();
        }

        return isAvailable;
    }

    public async Task<bool> CheckAvailableNightOptionsAsync(
        BookingAdjustRequest adjustRequest,
        CancellationToken cancellationToken = default
    )
    {
        var optionItemIds = adjustRequest.GetOptionItemIds();

        var existingCount = await optionItemService.CountAvailableByIdsAsync(
            optionItemIds,
            AppDate.GetDateTime(adjustRequest.CheckInDateId),
            cancellationToken
        );

        var isAvailable = existingCount == optionItemIds.Length;
        if (!isAvailable)
        {
            throw new OptionItemNotfoundException();
        }

        return isAvailable;
    }

    public Task<bool> CheckAvailableRoomRepresentativesAsync(
        BookingAdjustRequest adjustRequest,
        CancellationToken cancellationToken = default
    )
    {
        return Task.FromResult(true);
    }
}
