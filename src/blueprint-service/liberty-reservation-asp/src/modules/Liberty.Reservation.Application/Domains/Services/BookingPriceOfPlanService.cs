using Liberty.Reservation.Application.Constants;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Application.Domains.Services.Interfaces;
using Liberty.Reservation.Application.Exceptions;
using Liberty.Reservation.Application.Models;
using Liberty.Reservation.Application.Models.Requests;
using Liberty.Reservation.Application.Models.Responses;

namespace Liberty.Reservation.Application.Domains.Services;

public class BookingPriceOfPlanService : IBookingPriceService
{
    public BookingPriceResponse? GetBookingPrice(
        BookingPriceRequest bookingPriceRequest,
        BookingPlanModel plan,
        long roomGroupId
    )
    {
        var execThrowException = bookingPriceRequest.ExecThrowException;

        var roomGroupData = GetRoomGroupDataAvailable(
            bookingPriceRequest,
            plan,
            roomGroupId
        );
        if (roomGroupData is null)
        {
            return null;
        }

        var appDatePriceOfBookingResponses = new List<AppDatePriceOfBookingResponse>();

        //app date prices grouped by room
        var appDatePricesByRoom = GetRoomGroupSiteAppDatePrice(
            bookingPriceRequest,
            plan,
            roomGroupData
        );
        if (appDatePricesByRoom is null)
        {
            return null;
        }

        // Group by AppDateId to calculate totals
        var groupedAppDatePrices = appDatePricesByRoom
            .GroupBy(room => room.AppDateId)
            .Select(
                group => new AppDatePriceOfBookingResponse
                {
                    AppDateId = group.Key,
                    Price = group.Sum(room => room.Price),
                    TotalSpaTax = group.Sum(room => room.TotalSpaTax),
                    TotalOptionPrice = group.Sum(room => room.TotalOptionPrice),
                    Rooms = [.. group]
                }
            );

        appDatePriceOfBookingResponses.AddRange(groupedAppDatePrices);

        if (appDatePriceOfBookingResponses.Count != bookingPriceRequest.RestNumber)
        {
            return execThrowException ? throw new ReservationPriceException("restNumber") : null;
        }

        var bookingPriceResponse = new BookingPriceResponse
        {
            AppDatePrices = appDatePriceOfBookingResponses,
            UseSpaTax = plan.FacilityUseSpaTax
        };

        return bookingPriceResponse;
    }

    private static RoomGroupOfBookingPlanModel? GetRoomGroupDataAvailable(
        BookingPriceRequest bookingPriceRequest,
        BookingPlanModel planModel,
        long roomGroupId
    )
    {
        var execThrowException = bookingPriceRequest.ExecThrowException;
        var roomGroupData = planModel.RoomGroups.First(x => x.Id == roomGroupId);

        var availablePersonTypes = roomGroupData.PersonTypes.ToList();
        if (availablePersonTypes is { Count: 0 })
        {
            return execThrowException ? throw new ReservationPriceException("personAgeType of plan unavailable") : null;
        }

        var availablePriceData = roomGroupData.PriceData
            .Where(x => x.AppDateId >= bookingPriceRequest.CheckInDate)
            .ToList();
        if (availablePriceData is { Count: 0 })
        {
            return execThrowException ? throw new ReservationPriceException("price calendar of plan unavailable") : null;
        }

        var availableRoomAppDates = roomGroupData.AppDates
            .Where(x => x.AppDateId >= bookingPriceRequest.CheckInDate)
            .Where(x => x.RemainNumber > 0)
            .ToList();
        var isUnAvailable = availableRoomAppDates is { Count: 0 };
        if (!isUnAvailable)
        {
            return roomGroupData;
        }

        return execThrowException ? throw new ReservationPriceException("unavailable room inventory") : null;
    }

    private List<RoomOfBookingResponse>? GetRoomGroupSiteAppDatePrice(
        BookingPriceRequest bookingPriceRequest,
        BookingPlanModel planModel,
        RoomGroupOfBookingPlanModel roomGroupModel
    )
    {
        var inventoryOfAppDatePrices = SetRoomInventory(
            bookingPriceRequest,
            planModel,
            roomGroupModel
        );
        if (inventoryOfAppDatePrices is null)
        {
            return null;
        }

        SetPlanRoomPrice(
            bookingPriceRequest,
            planModel,
            roomGroupModel,
            inventoryOfAppDatePrices
        );

        return inventoryOfAppDatePrices;
    }

    protected virtual void SetPlanRoomPrice(
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

    protected static bool HasAnyPriceIsNegative(
        List<PeopleOfBookingResponse> peoples
    )
    {
        return !peoples.TrueForAll(p => (p.TotalPrice ?? 0) >= 0);
    }

    protected static int? CalculateDiscountedPrice(
        int numberGuest,
        int prevDay,
        int? price,
        BookingMetaPlanAppDateModel? planAppDate,
        List<BookingMetaDiscountDataModel> discountData
    )
    {
        if (planAppDate is null)
        {
            return price;
        }

        var discountedPrice = price;
        if (!planAppDate.UseAutoDiscount || discountData.Count <= 0)
        {
            return discountedPrice;
        }

        var existingDiscountData = discountData
            .Where(x => x.InRange(numberGuest))
            .OrderBy(x => x.PersonMax)
            .FirstOrDefault(x => x.InPrevDay(prevDay));

        if (existingDiscountData == null)
        {
            return discountedPrice;
        }

        var discount = existingDiscountData.GetDiscount(price);
        if (discount != null)
        {
            discountedPrice -= discount.Value;
        }

        return discountedPrice < 0 ? 0 : discountedPrice;
    }

    protected virtual void HandleCalculatePeoplePrice(
        List<PersonOfBookingPriceRequest> reservationPersonData,
        List<BookingMetaPersonTypeModel> personAgeTypes,
        RoomOfBookingResponse roomGroupAppDate,
        long reservationDate,
        int? discountedPrice,
        int? roomPrice
    )
    {
        var persons = reservationPersonData
            .Where(x => x.AppDateId == roomGroupAppDate.AppDateId)
            .Where(x => x.RoomGroupIndex == roomGroupAppDate.RoomIndex)
            .Where(x => x.Persons > 0)
            .ToList();

        if (persons is not { Count: > 0 })
        {
            persons =
            [
                .. reservationPersonData
                    .Where(x => x.AppDateId == reservationDate)
                    .Where(x => x.RoomGroupIndex == roomGroupAppDate.RoomIndex)
                    .Where(x => x.Persons > 0)
            ];
        }

        if (persons is not { Count: > 0 })
        {
            throw new ReservationPriceException($"personAgeType appDateId {roomGroupAppDate.AppDateId}");
        }

        foreach (var person in persons)
        {
            var personAgeTypeId = person.PersonAgeTypeId;
            var planPersonAgeType = personAgeTypes.Find(x => x.PersonAgeTypeId == personAgeTypeId)
                ?? throw new ReservationPriceException($"personAgeTypeId {personAgeTypeId}");
            var isPersonTypeMain = planPersonAgeType.PersonAgeTypeIsMain ?? false;

            var personRoomPrice = isPersonTypeMain
                ? discountedPrice
                : GetPriceOfPersonAgeType(
                    roomPrice,
                    planPersonAgeType.PriceSettingType,
                    planPersonAgeType.Value
                );

            if (isPersonTypeMain && roomPrice is null or <= 0)
            {
                throw new ReservationPriceException($"personAgeTypeId {personAgeTypeId} price greater than 0");
            }

            var numberOfPersons = person.Persons;
            var totalRoomPrice = personRoomPrice * numberOfPersons;
            var spaTax = planPersonAgeType.GetSpaTax(personRoomPrice) ?? 0;
            var totalSpaTax = spaTax * numberOfPersons;
            var nonePersons = numberOfPersons - person.MalePersons - person.FemalePersons;

            roomGroupAppDate.Peoples.Add(
                new PeopleOfBookingResponse(
                    personRoomPrice,
                    spaTax,
                    totalSpaTax,
                    totalRoomPrice,
                    person.MalePersons,
                    person.FemalePersons,
                    nonePersons,
                    numberOfPersons,
                    0,
                    personAgeTypeId,
                    planPersonAgeType.PersonAgeTypeName?.GetValueByHeader(DefaultValues.LanguageCode)
                )
            );
        }
    }

    protected static void HandleCalculateOptionPrice(
        List<OptionOfBookingPriceRequest> listSearchOptionItem,
        List<BookingMetaOptionItemModel> appDatesOfOptionItems,
        RoomOfBookingResponse roomGroupAppDate,
        long appDateId
    )
    {
        if (listSearchOptionItem is { Count: 0 })
        {
            return;
        }

        var optionOfBookingSearch = listSearchOptionItem
            .Where(x => x.AppDateId == roomGroupAppDate.AppDateId)
            .Where(x => x.RoomGroupIndex == roomGroupAppDate.RoomIndex)
            .Where(x => x.Number > 0)
            .ToList();

        if (optionOfBookingSearch is not { Count: > 0 })
        {
            return;
        }

        var optionItems = appDatesOfOptionItems
            .SelectMany(x => x.AppDates)
            .Where(x => x.AppDateId == appDateId)
            .ToList();

        foreach (var option in optionOfBookingSearch)
        {
            var optionItem = optionItems.Find(
                    x => x.OptionItemId == option.OptionItemId
                        && x.SellNumber >= option.Number
                )
                ?? throw new ReservationPriceException($"optionItemId {option.OptionItemId}");

            var optionOfBookingResponse = new OptionOfBookingResponse(
                optionItem.OptionItemId,
                optionItem.OptionItemName?.GetValueByHeader(),
                optionItem.OptionItemPrice,
                option.Number
            );

            roomGroupAppDate.Options.Add(optionOfBookingResponse);
        }
    }

    /// <summary>
    /// Get Price Of A Room
    /// </summary>
    protected static int? GetPrice(
        long appDateId,
        int? adultPersonAgePersons,
        List<BookingMetaPriceDataModel> priceData
    )
    {
        return priceData
            .Where(b => b.AppDateId == appDateId)
            .OrderBy(x => x.PersonMax)
            .FirstOrDefault(b => b.InRange(adultPersonAgePersons))
            ?.Price;
    }

    /// <summary>
    /// Number Of Guests Per Room
    /// </summary>
    protected virtual int NumberGuestsAdultOfRoom(
        long appDateId,
        long reservationDate,
        long roomIndex,
        List<PersonOfBookingPriceRequest> reservationPersonRequest,
        List<long> personAgeTypeIds
    )
    {
        var guestsAdultOfRoom = reservationPersonRequest
            .Where(x => x.AppDateId == appDateId)
            .Where(x => x.RoomGroupIndex == roomIndex)
            .Where(x => personAgeTypeIds.Contains(x.PersonAgeTypeId))
            .Where(x => x.Persons is > 0)
            .Distinct()
            .ToList();

        if (guestsAdultOfRoom is { Count: 0 })
        {
            guestsAdultOfRoom =
            [
                .. reservationPersonRequest
                    .Where(x => x.AppDateId == reservationDate)
                    .Where(x => x.RoomGroupIndex == roomIndex)
                    .Where(a => personAgeTypeIds.Contains(a.PersonAgeTypeId))
                    .Where(x => x.Persons is > 0)
                    .Distinct()
            ];
        }

        var persons = guestsAdultOfRoom.Sum(x => x.Persons) ?? 0;

        return persons;
    }

    /// <summary>
    /// Number Of Guests All Room
    /// </summary>
    private static int NumberOfGuestsAllRoom(
        long appDateId,
        long reservationDate,
        List<PersonOfBookingPriceRequest> reservationPersonRequest
    )
    {
        var numberPerson = reservationPersonRequest
            .Where(x => x.Persons is > 0)
            .Where(x => x.AppDateId == appDateId)
            .Sum(x => x.Persons!.Value);

        if (numberPerson is 0)
        {
            return reservationPersonRequest
                .Where(x => x.Persons is > 0)
                .Where(x => x.AppDateId == reservationDate)
                .Sum(x => x.Persons!.Value);
        }

        return numberPerson;
    }

    /// <summary>
    /// Max Number Of Guests Per Room
    /// </summary>
    private static int MaxNumberOfGuestsPerRoom(
        long appDateId,
        long reservationDate,
        long roomIndex,
        List<PersonOfBookingPriceRequest> reservationPersonRequest,
        List<long> personAgeTypeIdMaxGuest
    )
    {
        var maxPersons = reservationPersonRequest
            .Where(x => x.Persons is > 0)
            .Where(x => x.AppDateId == appDateId)
            .Where(x => x.RoomGroupIndex == roomIndex)
            .Where(x => personAgeTypeIdMaxGuest.Contains(x.PersonAgeTypeId))
            .Sum(x => x.Persons!.Value);

        if (maxPersons is 0)
        {
            return reservationPersonRequest
                .Where(x => x.Persons is > 0)
                .Where(x => x.RoomGroupIndex == roomIndex)
                .Where(x => x.AppDateId == reservationDate)
                .Where(x => personAgeTypeIdMaxGuest.Contains(x.PersonAgeTypeId))
                .Sum(x => x.Persons!.Value);
        }

        return maxPersons;
    }

    private List<RoomOfBookingResponse>? SetRoomInventory(
        BookingPriceRequest bookingPriceRequest,
        BookingPlanModel planModel,
        RoomGroupOfBookingPlanModel roomGroupModel
    )
    {
        var execThrowException = bookingPriceRequest.ExecThrowException;

        var planRoomGroupSiteAppDatePrices = new List<RoomOfBookingResponse>();
        long appDateId = 0;
        var roomIndex = 0;

        foreach (
            var roomGroupAppDate in roomGroupModel.AppDates
                .Where(
                    x => x.AppDateId >= bookingPriceRequest.CheckInDate
                        && x.AppDateId <= bookingPriceRequest.GetDateEndNight()
                )
                .OrderBy(x => x.AppDateId)
        )
        {
            if (appDateId != roomGroupAppDate.AppDateId)
            {
                roomIndex = 0;
            }

            appDateId = roomGroupAppDate.AppDateId;

            while (roomIndex < bookingPriceRequest.RoomNumber)
            {
                var data = new RoomOfBookingResponse(
                    appDateId,
                    roomIndex
                );

                var (isCheckDayBooking, errorMessage) = IsCheckDayBooking(
                    bookingPriceRequest,
                    roomGroupAppDate,
                    data,
                    planModel,
                    roomGroupModel
                );
                if (execThrowException && !string.IsNullOrEmpty(errorMessage))
                {
                    throw new ReservationPriceException(errorMessage);
                }

                switch (isCheckDayBooking)
                {
                    case false when !string.IsNullOrEmpty(errorMessage):
                        return null;
                    case true:
                        planRoomGroupSiteAppDatePrices.Add(data);
                        break;
                }

                roomIndex++;
            }
        }

        return planRoomGroupSiteAppDatePrices;
    }

    private (bool isValid, string? errorMessage) IsCheckDayBooking(
        BookingPriceRequest bookingPriceRequest,
        BookingMetaRoomAppDateModel roomGroupAppDate,
        RoomOfBookingResponse roomOfBookingResponse,
        BookingPlanModel planModel,
        RoomGroupOfBookingPlanModel roomGroupModel
    )
    {
        var appDateId = roomOfBookingResponse.AppDateId;
        var roomIndex = roomOfBookingResponse.RoomIndex;

        var reservationPersonRequest = bookingPriceRequest.GuestsPerRoom;

        var reservationPairs = planModel.Reservations
            .Where(x => x.PlanId == planModel.Id)
            .Count(x => x.BookingDateId == appDateId);

        var reservedNumber = planModel.Reservations
            .Where(x => x.RoomGroupId == roomGroupAppDate.RoomGroupId)
            .Count(x => x.BookingDateId == appDateId);

        if (bookingPriceRequest.ReservationId > 0)
        {
            reservedNumber -= planModel.Reservations
                .Where(x => x.RoomGroupId == roomGroupAppDate.RoomGroupId)
                .Where(x => x.ReservationId == bookingPriceRequest.ReservationId)
                .Count(x => x.BookingDateId == appDateId);
        }

        var remainNumber = GetRemainNumber(
            planModel,
            roomGroupAppDate.SellNumber ?? 0,
            reservedNumber
        );

        var roomNumber = bookingPriceRequest.RoomNumber;
        var reservationDate = bookingPriceRequest.CheckInDate;

        var personAgeTypeIds = roomGroupModel.PersonTypes
            .Where(x => x.IsRegardAdult)
            .Select(x => x.PersonAgeTypeId)
            .ToList();
        var personAgeTypeIdMaxGuest = roomGroupModel.PersonTypes
            .Where(x => (x.PersonAgeTypeIsMain ?? false) || (x.PersonAgeTypeFoodBed ?? 0) >= FoodBeds.Bed)
            .Select(x => x.PersonAgeTypeId)
            .ToList();

        var numberGuestsAdult = NumberGuestsAdultOfRoom(
            appDateId,
            reservationDate,
            roomIndex,
            reservationPersonRequest,
            personAgeTypeIds
        );
        var numberGuestAll = NumberOfGuestsAllRoom(
            appDateId,
            reservationDate,
            reservationPersonRequest
        );
        var numberOfMaxGuest = MaxNumberOfGuestsPerRoom(
            appDateId,
            reservationDate,
            roomIndex,
            reservationPersonRequest,
            personAgeTypeIdMaxGuest
        );

        if (!roomGroupModel.PriceData.Any())
        {
            return (false, $"{appDateId} price calendar not exist.");
        }

        var existingPrice = roomGroupModel.PriceData.ToList()
            .Exists(
                x => x.InRange(numberGuestsAdult)
                    && x.Price!.HasValue
                    && x.Price!.Value > 0
                    && x.AppDateId == appDateId
            );

        if (!existingPrice)
        {
            return (false, $"{appDateId} price not exist.");
        }

        if (!remainNumber.HasValue || remainNumber <= 0 || remainNumber < roomNumber)
        {
            return (false, $"{appDateId} unavailable room inventory.");
        }

        if (numberOfMaxGuest > roomGroupAppDate.RoomCapacityMax)
        {
            return (false, $"{appDateId} number of max guests in room.");
        }

        if (roomGroupAppDate.IsNotSelled)
        {
            return (false, $"{appDateId} room is not sell.");
        }

        if (!IsDaySaleLimitReservedNumber(roomGroupAppDate.ReservedNumber, planModel))
        {
            return (false, $"{appDateId} sale limit reserve number.");
        }

        if (!IsDaySaleLimitReservedPairs(reservationPairs, planModel))
        {
            return (false, $"{appDateId} sale limit reserved pairs.");
        }

        if (!IsDaySaleLimitAllPersons(numberGuestAll, planModel))
        {
            return (false, $"{appDateId} sale limit total persons.");
        }

        if (!planModel.CanDisplayDate(appDateId))
        {
            return (false, $"{appDateId} cannot display date.");
        }

        if (!planModel.CanAcceptDate(appDateId))
        {
            return (false, $"{appDateId} cannot accept date.");
        }

        if (!planModel.CanBookingReceptionDate(appDateId))
        {
            return (false, $"{appDateId} cannot booking reception date.");
        }

        return (true, null);
    }

    private static bool IsDaySaleLimitReservedNumber(
        int reservedNumber,
        BookingPlanModel plan
    )
    {
        if (!plan.UseDaySaleLimit || plan.PlanDaySaleLimitType != PlanDaySaleLimitTypes.RoomGroup || plan.RoomNumberDaySaleLimit == null
        )
        {
            return true;
        }

        return plan.RoomNumberDaySaleLimit.Value > reservedNumber;
    }

    private static bool IsDaySaleLimitReservedPairs(
        int reservationPairs,
        BookingPlanModel plan
    )
    {
        if (!plan.UseDaySaleLimit || plan.PlanDaySaleLimitType != PlanDaySaleLimitTypes.Pair || plan.GroupNumberDaySaleLimit == null)
        {
            return true;
        }

        return plan.GroupNumberDaySaleLimit.Value > reservationPairs;
    }

    private static bool IsDaySaleLimitAllPersons(
        int allPersons,
        BookingPlanModel plan
    )
    {
        return !plan.UseAcceptPersonNumber
            || (plan is { AcceptPersonNumberMax: not null, AcceptPersonNumberMin: not null }
                && plan.AcceptPersonNumberMax.Value >= allPersons
                && plan.AcceptPersonNumberMin.Value <= allPersons);
    }

    protected static int? GetPriceOfPersonAgeType(
        int? price,
        PriceSettingTypes priceSettingType,
        float? value
    )
    {
        var val = value ?? 0f;
        var p = price ?? 0;

        switch (priceSettingType)
        {
            default:
            case PriceSettingTypes.None:
                return price;

            case PriceSettingTypes.Percent:
                var val2 = p * val / 100;

                return (int)Math.Round(val2);

            case PriceSettingTypes.Price:
                return (int)val;

            case PriceSettingTypes.Discount:
                return p - (int)val;
        }
    }

    private static int? GetRemainNumber(
        BookingPlanModel plan,
        int sellNumber,
        int reservedNumber
    )
    {
        var remainNumber = sellNumber - reservedNumber;

        if (plan is { UseDaySaleLimit: true, PlanDaySaleLimitType: PlanDaySaleLimitTypes.RoomGroup })
        {
            remainNumber = plan.RoomNumberDaySaleLimit ?? 0 - reservedNumber;
        }

        return remainNumber;
    }
}
