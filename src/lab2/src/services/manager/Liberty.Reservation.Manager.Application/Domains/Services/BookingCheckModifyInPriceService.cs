using Liberty.Reservation.Application.Models.Requests;
using ReservationEntity = Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data.Reservation;

namespace Liberty.Reservation.Manager.Application.Domains.Services;

public class BookingCheckModifyInPriceService(
    IReservationService reservationService
) : IBookingCheckModifyInPriceService
{
    public async Task<bool> IsModifyInPriceAsync(
        ReservationEntity existingReservation,
        BookingAdjustRequest adjustRequest,
        CancellationToken cancellationToken = default
    )
    {
        if (existingReservation.RestNumber != adjustRequest.NumberOfNights)
        {
            return true;
        }

        if (existingReservation.RoomNumber != adjustRequest.NumberOfRooms)
        {
            return true;
        }

        var isModifyPeoples = await IsModifyPeoplesAsync(
            existingReservation,
            adjustRequest.NightPeoples?.ToList(),
            cancellationToken
        );
        if (isModifyPeoples)
        {
            return true;
        }

        var isModifyOptions = await IsModifyOptionsAsync(
            existingReservation,
            adjustRequest.NightOptions?.ToList(),
            cancellationToken
        );

        return isModifyOptions;
    }

    public async Task<bool> IsModifyPeoplesAsync(
        ReservationEntity existingReservation,
        List<NightPeopleOfReservationAdjustRequest>? roomPeoplesRequest,
        CancellationToken cancellationToken = default
    )
    {
        if (roomPeoplesRequest is null)
        {
            return false;
        }

        var existingPeoplesOfReservation = await reservationService
            .GetAllPersonAgeTypesOfReservationAsync(
                existingReservation.Id,
                cancellationToken
            ) as List<ReservationRoomGroupAppDatePersonAgeType>;

        if (existingPeoplesOfReservation is not { Count: > 0 })
        {
            return false;
        }

        var roomPeoples = roomPeoplesRequest
            .SelectMany(x => x.Rooms)
            .SelectMany(
                x => x.Peoples,
                (
                    room,
                    people
                ) => new
                {
                    room.RoomIndex,
                    people.PersonAgeTypeId,
                    people.NumberOfPeoples
                }
            )
            .ToList();

        if (existingPeoplesOfReservation.Count != roomPeoples.Count)
        {
            return true;
        }

        foreach (var roomPeople in roomPeoples)
        {
            var data = existingPeoplesOfReservation
                .SingleOrDefault(
                    x => x.RestIndex == roomPeople.RoomIndex
                         && x.PersonAgeTypeId == roomPeople.PersonAgeTypeId
                );

            if (data is null)
            {
                return true;
            }

            if (data.Number != roomPeople.NumberOfPeoples)
            {
                return true;
            }
        }

        return false;
    }

    public async Task<bool> IsModifyOptionsAsync(
        ReservationEntity existingReservation,
        List<NightOptionOfReservationAdjustRequest>? nightOptionsRequest,
        CancellationToken cancellationToken = default
    )
    {
        if (nightOptionsRequest is null)
        {
            return false;
        }

        var existingOptionsOfReservation = await reservationService
            .GetAllOptionItemsOfReservationAsync(
                existingReservation.Id,
                cancellationToken
            ) as List<ReservationRoomGroupAppDateOptionItem>;

        if (existingOptionsOfReservation is not { Count: > 0 })
        {
            return false;
        }

        var nightOptions = nightOptionsRequest
            .SelectMany(
                nightOption => nightOption.Rooms,
                (
                    nightOption,
                    room
                ) => new
                {
                    nightOption.AppDateId,
                    room.RoomIndex,
                    room.OptionItems
                }
            )
            .SelectMany(
                x => x.OptionItems,
                (
                    x,
                    option
                ) => (x.AppDateId, x.RoomIndex, option.OptionItemId, option.Number)
            )
            .ToList();

        if (existingOptionsOfReservation.Count != nightOptions.Count)
        {
            return true;
        }

        foreach (var nightOption in nightOptions)
        {
            var data = existingOptionsOfReservation.SingleOrDefault(
                x =>
                    x.AppDateId == nightOption.AppDateId
                    && x.RestIndex == nightOption.RoomIndex
                    && x.OptionItemId == nightOption.OptionItemId
            );

            if (data is null)
            {
                return true;
            }

            if (data.Number != nightOption.Number)
            {
                return true;
            }
        }

        return false;
    }
}
