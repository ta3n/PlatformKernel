using Liberty.Reservation.Application.Constants;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;
using Liberty.Reservation.Application.Domains.Repositories.Interfaces;
using Liberty.Reservation.Application.Domains.Services.Interfaces;
using Liberty.Reservation.Application.Models.Requests;
using ReservationEntity = Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data.Reservation;

namespace Liberty.Reservation.Application.Domains.Services;

public class BookingCheckModifyInPriceService(
    IBookingReservationRepository bookingReservationRepository
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

    private async Task<bool> IsModifyPeoplesAsync(
        ReservationEntity existingReservation,
        List<NightPeopleOfReservationAdjustRequest>? roomPeoplesRequest,
        CancellationToken cancellationToken = default
    )
    {
        if (roomPeoplesRequest is null)
        {
            return false;
        }

        if (
            await bookingReservationRepository
                .GetAllPersonAgeTypesOfReservationAsync(
                    existingReservation.FacilityId,
                    existingReservation.Id,
                    cancellationToken
                ) is not List<ReservationRoomGroupAppDatePersonAgeType> { Count: > 0 } existingPeoplesOfReservation
        )
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
                    people.NumberOfPeoples,
                    people.Gender
                }
            )
            .ToList();

        if (existingPeoplesOfReservation.Sum(x => x.Number) != roomPeoples.Count)
        {
            return true;
        }

        foreach (var roomPeople in roomPeoples)
        {
            var data = existingPeoplesOfReservation
                .Find(
                    x => x.RestIndex == roomPeople.RoomIndex
                        && x.PersonAgeTypeId == roomPeople.PersonAgeTypeId
                );

            if (data is null)
            {
                return true;
            }

            switch (roomPeople.Gender)
            {
                case Genders.Female when data.FemaleNumber != roomPeople.NumberOfPeoples:
                case Genders.Male when data.MaleNumber != roomPeople.NumberOfPeoples:
                case Genders.None when data.GenderNoneNumber != roomPeople.NumberOfPeoples:
                    return true;
                default:
                    continue;
            }
        }

        return false;
    }

    private async Task<bool> IsModifyOptionsAsync(
        ReservationEntity existingReservation,
        List<NightOptionOfReservationAdjustRequest>? nightOptionsRequest,
        CancellationToken cancellationToken = default
    )
    {
        if (nightOptionsRequest is null)
        {
            return false;
        }

        var existingOptionsOfReservation = await bookingReservationRepository
            .GetAllOptionItemsOfReservationAsync(
                existingReservation.FacilityId,
                existingReservation.Id,
                cancellationToken
            ) as List<ReservationRoomGroupAppDateOptionItem>;

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

        if (existingOptionsOfReservation?.Count != nightOptions.Count)
        {
            return true;
        }

        foreach (var (appDateId, roomIndex, optionItemId, number) in nightOptions)
        {
            var data = existingOptionsOfReservation.SingleOrDefault(
                x =>
                    x.BookingDateId == appDateId
                    && x.RoomGroupIndex == roomIndex
                    && x.OptionItemId == optionItemId
            );

            if (data is null)
            {
                return true;
            }

            if (data.Number != number)
            {
                return true;
            }
        }

        return false;
    }
}
