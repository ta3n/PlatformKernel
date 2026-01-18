using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Application.Exceptions;
using Liberty.Reservation.Application.Models;
using Liberty.Reservation.Application.Models.Requests;
using Liberty.Reservation.Application.Models.Responses;

namespace Liberty.Reservation.Application.Domains.Services;

public class BookingPriceOfRoomService : BookingPriceOfPlanService
{
    protected override void SetPlanRoomPrice(
        BookingPriceRequest bookingPriceRequest,
        BookingPlanModel planModel,
        RoomGroupOfBookingPlanModel roomGroupModel,
        List<RoomOfBookingResponse> planRoomGroupSiteAppDatePrices
    )
    {
        var execThrowException = bookingPriceRequest.ExecThrowException;

        var reservationDate = bookingPriceRequest.CheckInDate;
        var reservationPersonData = bookingPriceRequest.GuestsPerRoom;
        var listSearchOptionItem = bookingPriceRequest.OptionItems ?? [];

        var personAgeTypeIds = roomGroupModel.PersonTypes
            .Where(x => x.IsRegardAdult)
            .Select(x => x.PersonAgeTypeId)
            .ToList();
        var bookingDate = DateTime.UtcNow;

        foreach (var roomGroupAppDate in planRoomGroupSiteAppDatePrices)
        {
            var appDateId = roomGroupAppDate.AppDateId;
            var numberGuest = NumberGuestsAdultOfRoom(
                appDateId,
                reservationDate,
                roomGroupAppDate.RoomIndex,
                reservationPersonData,
                personAgeTypeIds
            );
            var prevDay = (AppDate.GetDateTime(appDateId).Date - bookingDate.Date).Days;

            var planRoomGroupSiteAppDate = roomGroupModel.PlanAppDates
                .SingleOrDefault(x => x.AppDateId == appDateId);

            var roomPrice = GetPrice(
                appDateId,
                numberGuest,
                [..roomGroupModel.PriceData]
            );

            var discountedPrice = CalculateDiscountedPrice(
                numberGuest,
                prevDay,
                roomPrice,
                planRoomGroupSiteAppDate,
                [..roomGroupModel.DiscountData]
            );

            HandleCalculatePeoplePrice(
                reservationPersonData,
                [..roomGroupModel.PersonTypes],
                roomGroupAppDate,
                reservationDate,
                discountedPrice,
                roomPrice
            );

            HandleCalculateOptionPrice(
                listSearchOptionItem,
                [..planModel.OptionItems],
                roomGroupAppDate,
                appDateId
            );

            roomGroupAppDate.Price = roomGroupAppDate.Peoples.Sum(x => x.TotalPrice ?? 0);
            roomGroupAppDate.TotalSpaTax = roomGroupAppDate.Peoples.Sum(x => x.TotalSpaTax) ?? 0;
            roomGroupAppDate.TotalOptionPrice = roomGroupAppDate.Options.Sum(x => x.TotalOptionPrice);

            if (HasAnyPriceIsNegative(roomGroupAppDate.Peoples))
            {
                throw new ReservationPriceException("all people's total prices less than or equal to 0");
            }

            if (roomGroupAppDate.TotalPrice > 0)
            {
                continue;
            }

            if (execThrowException)
            {
                throw new ReservationPriceException("totalPrice");
            }
        }
    }

    protected override void HandleCalculatePeoplePrice(
        List<PersonOfBookingPriceRequest> reservationPersonData,
        List<BookingMetaPersonTypeModel> personAgeTypes,
        RoomOfBookingResponse roomGroupAppDate,
        long reservationDate,
        int? discountedPrice,
        int? roomPrice
    )
    {
        var persons = GetFilteredPersons(
            reservationPersonData,
            roomGroupAppDate,
            reservationDate
        );
        var existingReqPersonIds = persons.Select(x => x.PersonAgeTypeId).ToList();
        var mainPersonTypeId = GetMainPersonTypeId(
            personAgeTypes
        );

        foreach (var person in persons)
        {
            var planPersonAgeType = GetPlanPersonAgeType(
                person.PersonAgeTypeId,
                personAgeTypes
            );
            var isPersonTypeMain = planPersonAgeType.PersonAgeTypeIsMain ?? false;

            var calculatedRoomPrice = CalculateRoomPrice(
                planPersonAgeType,
                roomPrice,
                discountedPrice
            );

            if (isPersonTypeMain)
            {
                ValidateRoomPrice(
                    planPersonAgeType,
                    calculatedRoomPrice,
                    person.PersonAgeTypeId
                );
            }

            const int defaultNumberOfPersons = 1;
            var otherPersons = 0;
            var numberOfPersons = person.Persons;
            var nonePersons = numberOfPersons - person.MalePersons - person.FemalePersons;

            var selectNumberOfPersons = !isPersonTypeMain
                ? numberOfPersons
                : defaultNumberOfPersons;

            var totalRoomPrice = calculatedRoomPrice * selectNumberOfPersons;
            var spaTax = planPersonAgeType.GetSpaTax(calculatedRoomPrice) ?? 0;
            var totalSpaTax = spaTax * numberOfPersons;

            if (isPersonTypeMain && planPersonAgeType.IsRegardAdult)
            {
                otherPersons = CalculateOtherPersons(
                    persons,
                    personAgeTypes,
                    existingReqPersonIds,
                    mainPersonTypeId
                );
            }

            roomGroupAppDate.Peoples.Add(
                new PeopleOfBookingResponse(
                    calculatedRoomPrice,
                    spaTax,
                    totalSpaTax,
                    totalRoomPrice,
                    person.MalePersons,
                    person.FemalePersons,
                    nonePersons,
                    numberOfPersons,
                    otherPersons,
                    person.PersonAgeTypeId,
                    planPersonAgeType.PersonAgeTypeName?.GetValueByHeader()
                )
            );
        }
    }

    private static List<PersonOfBookingPriceRequest> GetFilteredPersons(
        List<PersonOfBookingPriceRequest> reservationPersonData,
        RoomOfBookingResponse roomGroupAppDate,
        long reservationDate
    )
    {
        var persons = reservationPersonData
            .Where(x => x.AppDateId == roomGroupAppDate.AppDateId)
            .Where(x => x.RoomGroupIndex == roomGroupAppDate.RoomIndex)
            .Where(x => x.Persons > 0)
            .ToList();

        if (persons is { Count: 0 })
        {
            persons =
            [
                .. reservationPersonData
                    .Where(x => x.AppDateId == reservationDate)
                    .Where(x => x.RoomGroupIndex == roomGroupAppDate.RoomIndex)
                    .Where(x => x.Persons > 0)
            ];
        }

        return persons.Count != 0
            ? persons
            : throw new ReservationPriceException($"personAgeType appDateId {roomGroupAppDate.AppDateId}");
    }

    private static long GetMainPersonTypeId(
        List<BookingMetaPersonTypeModel> personAgeTypes
    )
    {
        return personAgeTypes.Find(x => x.PersonAgeTypeIsMain ?? false)?.PersonAgeTypeId ?? 0;
    }

    private static BookingMetaPersonTypeModel GetPlanPersonAgeType(
        long personAgeTypeId,
        List<BookingMetaPersonTypeModel> personAgeTypes
    )
    {
        return personAgeTypes.Find(x => x.PersonAgeTypeId == personAgeTypeId)
            ?? throw new ReservationPriceException($"personAgeTypeId {personAgeTypeId}");
    }

    private static int? CalculateRoomPrice(
        BookingMetaPersonTypeModel personAgeType,
        int? roomPrice,
        int? discountedPrice
    )
    {
        return personAgeType.PersonAgeTypeIsMain ?? false
            ? discountedPrice
            : GetPriceOfPersonAgeType(
                roomPrice,
                personAgeType.PriceSettingType,
                personAgeType.Value
            );
    }

    private static void ValidateRoomPrice(
        BookingMetaPersonTypeModel personAgeType,
        int? roomPrice,
        long personAgeTypeId
    )
    {
        if (personAgeType.IsRegardAdult && roomPrice is null or <= 0)
        {
            throw new ReservationPriceException($"personAgeTypeId {personAgeTypeId} price must be greater than 0");
        }
    }

    private static int CalculateOtherPersons(
        List<PersonOfBookingPriceRequest> persons,
        List<BookingMetaPersonTypeModel> personAgeTypes,
        List<long> existingReqPersonIds,
        long mainPersonTypeId
    )
    {
        var regardedAdultPersonIds = personAgeTypes
            .Where(x => existingReqPersonIds.Contains(x.PersonAgeTypeId) && x.IsRegardAdult)
            .Select(x => x.PersonAgeTypeId)
            .Where(id => id != mainPersonTypeId)
            .ToList();

        return persons
                .Where(x => regardedAdultPersonIds.Contains(x.PersonAgeTypeId))
                .Sum(x => x.Persons)
            ?? 0;
    }
}
