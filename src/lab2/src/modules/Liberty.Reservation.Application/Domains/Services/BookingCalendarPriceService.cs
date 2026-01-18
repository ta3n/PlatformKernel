using Liberty.Reservation.Application.Constants;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Application.Domains.Services.Interfaces;
using Liberty.Reservation.Application.Models;
using Liberty.Reservation.Application.Models.Requests;
using Liberty.Reservation.Application.Models.Responses;
using Microsoft.Extensions.DependencyInjection;

namespace Liberty.Reservation.Application.Domains.Services;

public sealed class BookingCalendarPriceService(
    [FromKeyedServices("plan")] IBookingPriceService bookingPriceOfPlanService,
    [FromKeyedServices("room")] IBookingPriceService bookingPriceOfRoomService,
    IBookingReservationService bookingReservationService
) : IBookingCalendarPriceService
{
    public IEnumerable<BookingSearchByPlanResponse> GetAllRoomDatePricesInPlans(
        BookingSearchModel bookingSearchModel,
        List<BookingPlanModel> listPlan,
        long siteId,
        long facilityId,
        bool isLoadingPriceAppDate = true
    )
    {
        var response = new List<BookingSearchByPlanResponse>();

        var facilityTimeZone = bookingReservationService.GetFacilityTimeZoneById(facilityId);
        var bookingSearchDate = DateTime.UtcNow.Add(
            facilityTimeZone
        );
        var bookingSearchDateId = AppDate.GetId(bookingSearchDate);

        foreach (var bookingPlanData in listPlan)
        {
            var planOfBookingResponse = new BookingSearchByPlanResponse(
                bookingPlanData.Id,
                bookingPlanData.Name?.GetValueByHeader(DefaultValues.LanguageCode),
                bookingPlanData.Tag?.GetValueByCode(DefaultValues.LanguageCode),
                bookingPlanData.IsOnLinePayment,
                bookingPlanData.IsOnSidePayment,
                bookingPlanData.Summary?.GetValueByHeader(DefaultValues.LanguageCode),
                bookingPlanData.UseDisplayDate,
                bookingPlanData.DisplayDateStart,
                bookingPlanData.DisplayDateEnd,
                bookingPlanData.UseAcceptDate,
                bookingPlanData.AcceptDateStart,
                bookingPlanData.AcceptDateEnd,
                bookingPlanData.PlanType,
                bookingPlanData.DisplayOrder,
                bookingPlanData.Description?.GetValueByHeader(DefaultValues.LanguageCode),
                bookingPlanData.DayUse
            )
            {
                Code = bookingPlanData.Code,
                FacilityId = facilityId,
                FacilityCode = bookingPlanData.FacilityCode,
                CheckInStart = bookingPlanData.CheckInStart,
                CheckInEnd = bookingPlanData.CheckInEnd,
                CheckOut = bookingPlanData.CheckOut,
                Categories =
                [
                    .. bookingPlanData.Categories.Select(
                        x => new CategoryOfPlanResponse(
                            x.Id,
                            x.Name?.GetValueByHeader(DefaultValues.LanguageCode)
                        ) { Code = x.Code }
                    )
                ],
                Files =
                [
                    .. bookingPlanData.Media.Select(
                        x => new FileOfPlanResponse(
                            x.Code,
                            x.ContentType,
                            x.Index,
                            x.IsEnabled
                        )
                    )
                ],
                Meals =
                [
                    .. bookingPlanData.MealTypes.Select(
                        x => new MealOfPlanResponse(
                            x.Id,
                            x.MealTypeEatType,
                            x.Name
                        ) { Code = x.Code }
                    )
                ],
                Rooms =
                [
                    .. bookingPlanData.RoomGroups.Select(
                        x => new RoomOfPlanResponse(
                            x.Id,
                            x.Name?.GetValueByHeader(DefaultValues.LanguageCode),
                            x.Tag,
                            x.IsEnabledSmoking,
                            x.IsOverviewVisible ? x.Overview?.GetValueByHeader(DefaultValues.LanguageCode) : null,
                            x.DisplayOrder
                        )
                        {
                            Code = x.Code,
                            Files =
                            [
                                .. x.Media.Select(
                                    y => new FileOfPlanResponse(
                                        y.Code,
                                        y.ContentType,
                                        y.Index,
                                        y.IsEnabled
                                    )
                                )
                            ],
                            AppDatePrices = []
                        }
                    )
                ]
            };

            foreach (var room in planOfBookingResponse.Rooms)
            {
                var roomId = room.Id;

                var roomGroupOfBookingPlanData = bookingPlanData.RoomGroups.First(
                    x => x.Id == roomId
                );

                var metaDataOfRoom = new BookingMetaDataModel(
                    roomGroupOfBookingPlanData.AppDates,
                    roomGroupOfBookingPlanData.PlanAppDates,
                    roomGroupOfBookingPlanData.PersonTypes,
                    [],
                    roomGroupOfBookingPlanData.PriceData,
                    roomGroupOfBookingPlanData.DiscountData,
                    bookingPlanData.Reservations
                );

                var appDatePricesOfRoom = TryGetAppDatePricesOfRoom(
                    bookingSearchModel,
                    bookingPlanData,
                    metaDataOfRoom,
                    bookingSearchDateId,
                    facilityTimeZone
                );

                if (appDatePricesOfRoom is null)
                {
                    continue;
                }

                room.AppDatePrices.AddRange(appDatePricesOfRoom);
            }

            planOfBookingResponse.BasePrice = planOfBookingResponse.Rooms.SelectMany(
                    x => x.AppDatePrices.Select(
                        y => y.BasePrice
                    )
                )
                .Min();

            if (planOfBookingResponse.Rooms.SelectMany(x => x.AppDatePrices).ToList() is not { Count: > 0 })
            {
                continue;
            }

            planOfBookingResponse.Rooms = [.. planOfBookingResponse.Rooms.Where(x => x.AppDatePrices.Count > 0)];

            response.Add(planOfBookingResponse);
        }

        response = CombineAndOrderPlans(response);

        CalculateMinPrices(response, bookingSearchModel);

        return response;
    }

    /// <summary>
    /// Attempts to retrieve the calculated room prices and availability data for a specific room within a booking plan.
    /// </summary>
    /// <param name="bookingSearchModel">The booking criteria and search parameters, including check-in dates and guest information.</param>
    /// <param name="bookingPlanData">Data of the booking plan containing room group, pricing, and scheduling details.</param>
    /// <param name="metaDataOfRoom">Metadata of the room including availability, pricing, and applicable person types information.</param>
    /// <param name="bookingSearchDateId">The identifier of the booking search date used to validate the plan's acceptance date.</param>
    /// <returns>
    /// A list of room price responses with corresponding availability and inventory details if successful;
    /// null if no valid price data is found or conditions are not met.
    /// </returns>
    private List<AppDatePriceOfPlanResponse>? TryGetAppDatePricesOfRoom(
        BookingSearchModel bookingSearchModel,
        BookingPlanModel bookingPlanData,
        BookingMetaDataModel metaDataOfRoom,
        long bookingSearchDateId,
        TimeSpan facilityTimeZone
    )
    {
        if (!metaDataOfRoom.PersonalTypes.Any())
        {
            return null;
        }

        var appDatePricesOfRoom = GetAllAppDatePricesOfRoom(
            bookingSearchModel,
            bookingPlanData,
            metaDataOfRoom,
            bookingSearchDateId,
            facilityTimeZone
        );

        var endDate = AppDate.GetDateTime(bookingSearchModel.CheckInDate).AddDays(bookingSearchModel.RestNumber - 1);
        var checkOutDate = AppDate.GetDateTime(bookingSearchModel.CheckOutDate);

        var isNotMatching = endDate == checkOutDate && bookingSearchModel.RestNumber != appDatePricesOfRoom.Count;

        // if (isNotMatching || appDatePricesOfRoom is { Count: 0 } || appDatePricesOfRoom.TrueForAll(p => !p.Status.IsAvailable))
        // {
        //     return null;
        // }
        if (isNotMatching || appDatePricesOfRoom is { Count: 0 })
        {
            return null;
        }

        return appDatePricesOfRoom;
    }

    /// <summary>
    /// Retrieves calculated prices, inventory, and availability information for specific rooms within a plan based on booking search criteria.
    /// </summary>
    /// <param name="bookingSearchModel">Booking criteria including dates, room requirements, and guest information.</param>
    /// <param name="plan">The booking plan containing room group configurations and conditions.</param>
    /// <param name="metaDataOfRoom">Additional metadata for the rooms related to availability, pricing, and reservations.</param>
    /// <param name="bookingSearchDateId">The identifier of the booking search date used to validate the plan's acceptance date.</param>
    /// <returns>
    /// A list of room price responses that include inventory, pricing, and booking eligibility details.
    /// </returns>
    private List<AppDatePriceOfPlanResponse> GetAllAppDatePricesOfRoom(
        BookingSearchModel bookingSearchModel,
        BookingPlanModel plan,
        BookingMetaDataModel metaDataOfRoom,
        long bookingSearchDateId,
        TimeSpan facilityTimeZone
    )
    {
        // Check if the booking search date is within the plan's acceptable date range
        if (!plan.CanAcceptDate(bookingSearchDateId))
        {
            var appDatePrices = new List<AppDatePriceOfPlanResponse>();

            bookingSearchModel.GetAppDates()
                .ToList()
                .ForEach(
                    appDateId =>
                    {
                        appDatePrices.Add(
                            new AppDatePriceOfPlanResponse(appDateId, 0)
                            {
                                Status = new AppDatePriceStatusSearchModel
                                {
                                    IsRoomAvailable = false,
                                    IsRoomUnderRequested = false,
                                    IsAcceptDate = false,
                                    IsDayBookable = false,
                                    IsNight = false
                                }
                            }
                        );
                    }
                );

            return appDatePrices;
        }

        // Inventory
        var inventoryAppDates = GetAllInventoryAppDates(
            bookingSearchModel,
            plan,
            metaDataOfRoom,
            facilityTimeZone
        );

        // Price
        var appDatePriceOfPlanResponses = GetAllPlanRoomPrices(
            bookingSearchModel,
            plan,
            metaDataOfRoom,
            inventoryAppDates,
            facilityTimeZone
        );

        var appDatePriceMax = appDatePriceOfPlanResponses is { Count: > 0 }
            ? appDatePriceOfPlanResponses.Max(x => x.AppDateId)
            : bookingSearchModel.CheckInDate;
        var bookingCheckOutDate = AppDate.GetDateTime(bookingSearchModel.CheckOutDate);
        if (appDatePriceMax >= bookingSearchModel.CheckOutDate)
        {
            return appDatePriceOfPlanResponses;
        }

        var appDatePriceNext = AppDate.GetDateTime(appDatePriceMax).AddDays(1);
        while (appDatePriceNext <= bookingCheckOutDate)
        {
            appDatePriceOfPlanResponses.Add(
                new AppDatePriceOfPlanResponse(
                    AppDate.GetId(appDatePriceNext),
                    0
                )
                {
                    Status = new AppDatePriceStatusSearchModel
                    {
                        IsRoomAvailable = false,
                        IsRoomUnderRequested = false,
                        IsAcceptDate = false,
                        IsDayBookable = false,
                        IsNight = false
                    }
                }
            );

            appDatePriceNext = appDatePriceNext.AddDays(1);
        }

        return appDatePriceOfPlanResponses;
    }

    /// <summary>
    /// Calculates and sets the price details of plan rooms based on the booking search criteria, availability, and pricing data.
    /// </summary>
    /// <param name="bookingSearchModel">The booking search model containing reservation parameters such as check-in date, rest days, and guests.</param>
    /// <param name="plan">The booking plan that includes room groups, pricing policies, and reception availability configuration.</param>
    /// <param name="metaDataOfRoom">The metadata of the room containing information about pricing data, personal types, and reserved dates.</param>
    /// <param name="inventoryAppDates">A list of application dates with room inventory and availability details.</param>
    /// <returns>
    /// A list of <see cref="AppDatePriceOfPlanResponse"/> representing calculated prices and availability for the specified plan rooms and dates.
    /// </returns>
    private List<AppDatePriceOfPlanResponse> GetAllPlanRoomPrices(
        BookingSearchModel bookingSearchModel,
        BookingPlanModel plan,
        BookingMetaDataModel metaDataOfRoom,
        List<AppDatePriceOfPlanResponse> inventoryAppDates,
        TimeSpan facilityTimeZone
    )
    {
        var listAppDatePriceOfPlanResponse = new List<AppDatePriceOfPlanResponse>();

        var personAgeTypes = metaDataOfRoom.PersonalTypes.ToList();

        foreach (var roomGroupAppDate in inventoryAppDates)
        {
            var appDateId = roomGroupAppDate.AppDateId;

            if (roomGroupAppDate.Status is { IsRoomUnderRequested: false, IsDayBookable: false })
            {
                continue;
            }

            var isValidGuestNumber = CheckGuestNumberOfRoom(
                bookingSearchModel,
                personAgeTypes,
                [.. metaDataOfRoom.PriceData]
            );

            var isReceptionAvailable = plan.IsReceptionAvailable(appDateId, null, true, facilityTimeZone);
            var isBookingReceptionDate = plan.CanBookingReceptionDate(appDateId);

            if (!isValidGuestNumber || !isReceptionAvailable || !isBookingReceptionDate)
            {
                var priceOfRoomGroupAppDateData = new AppDatePriceOfPlanResponse(
                    appDateId,
                    roomGroupAppDate.RemainNumber
                )
                {
                    TotalSpaTax = null,
                    BasePrice = null,
                    Price = null,
                    Status = roomGroupAppDate.Status
                };

                listAppDatePriceOfPlanResponse.Add(priceOfRoomGroupAppDateData);
                continue;
            }

            var bookingPriceResponse = GetBookingPriceResponse(
                bookingSearchModel,
                plan,
                roomGroupAppDate
            );
            if (bookingPriceResponse is null)
            {
                roomGroupAppDate.Status.IsNight = false;
                var priceOfRoomGroupAppDateData = new AppDatePriceOfPlanResponse(
                    appDateId,
                    roomGroupAppDate.RemainNumber
                )
                {
                    TotalSpaTax = null,
                    BasePrice = null,
                    Price = null,
                    Status = roomGroupAppDate.Status
                };

                listAppDatePriceOfPlanResponse.Add(priceOfRoomGroupAppDateData);
                continue;
            }

            roomGroupAppDate.Status.IsNight = true;
            var priceOfRoomGroupAppDate = new AppDatePriceOfPlanResponse(appDateId, roomGroupAppDate.RemainNumber)
            {
                TotalSpaTax = bookingPriceResponse.TotalSpaTax,
                BasePrice = bookingPriceResponse.TotalPrice - bookingPriceResponse.TotalSpaTax,
                Price = bookingPriceResponse.TotalPrice - bookingPriceResponse.TotalSpaTax,
                Status = roomGroupAppDate.Status
            };

            var isMinPriceValid = !bookingSearchModel.MinPrice.HasValue
                || priceOfRoomGroupAppDate.Price >= bookingSearchModel.MinPrice;
            var isMaxPriceValid = !bookingSearchModel.MaxPrice.HasValue
                || priceOfRoomGroupAppDate.Price <= bookingSearchModel.MaxPrice;

            if (
                priceOfRoomGroupAppDate.Price > 0
                && isMinPriceValid
                && isMaxPriceValid
            )
            {
                listAppDatePriceOfPlanResponse.Add(priceOfRoomGroupAppDate);
            }
        }

        return listAppDatePriceOfPlanResponse;
    }

    /// <summary>
    /// Retrieves the booking price response for a specific room group and application date
    /// based on the provided booking search model and plan details.
    /// </summary>
    /// <param name="bookingSearchModel">
    /// The model containing booking search parameters such as check-in date, rest number,
    /// room number, and guest details.
    /// </param>
    /// <param name="plan">
    /// The booking plan model that includes details about pricing, room groups, and plan type.
    /// </param>
    /// <param name="roomGroupAppDate">
    /// The response object containing details about the room group and application date
    /// for which the booking price is being calculated.
    /// </param>
    /// <returns>
    /// A <see cref="BookingPriceResponse"/> object containing the calculated booking price
    /// details, or <c>null</c> if an exception occurs during the calculation.
    /// </returns>
    private BookingPriceResponse? GetBookingPriceResponse(
        BookingSearchModel bookingSearchModel,
        BookingPlanModel plan,
        AppDatePriceOfPlanResponse roomGroupAppDate
    )
    {
        var appDateId = roomGroupAppDate.AppDateId;

        var bookingPriceRequest = new BookingPriceRequest
        {
            CheckInDate = appDateId,
            RestNumber = bookingSearchModel.RestNumber,
            RoomNumber = bookingSearchModel.RoomNumber,
            GuestsPerRoom = ToPriceRequests(bookingSearchModel.GuestsPerRoom, appDateId),
            ExecThrowException = false
        };

        try
        {
            var bookingPriceResponse = plan.PlanType is PlanTypes.Combo
                ? bookingPriceOfPlanService.GetBookingPrice(
                    bookingPriceRequest,
                    plan,
                    roomGroupAppDate.RoomGroupId
                )
                : bookingPriceOfRoomService.GetBookingPrice(
                    bookingPriceRequest,
                    plan,
                    roomGroupAppDate.RoomGroupId
                );

            return bookingPriceResponse;
        }
        catch (Exception)
        {
            return null;
        }
    }

    /// <summary>
    /// Validates whether the number of guests for each room matches the pricing and capacity constraints of the booking plan.
    /// </summary>
    /// <param name="bookingSearchModel">Model containing details for the booking, including guest distribution per room.</param>
    /// <param name="personAgeTypes">List of person age types and associated metadata applicable to the plan.</param>
    /// <param name="priceDataList">Collection of pricing data, including capacity and pricing ranges for comparison.</param>
    /// <returns>
    /// Returns true if the number of guests in each room stays within the defined pricing and capacity limits.
    /// Returns false otherwise.
    /// </returns>
    private static bool CheckGuestNumberOfRoom(
        BookingSearchModel bookingSearchModel,
        List<BookingMetaPersonTypeModel> personAgeTypes,
        List<BookingMetaPriceDataModel> priceDataList
    )
    {
        var personGroups = bookingSearchModel.GuestsPerRoom!
            .GroupBy(
                g => new
                {
                    g.RoomGroupIndex,
                    g.AppDateId
                }
            );

        var personAgeTypeIds = personAgeTypes
            .Where(x => (x.PersonAgeTypeIsMain ?? false) || x.PersonAgeTypeFoodBed >= FoodBeds.Bed)
            .Select(x => x.PersonAgeTypeId)
            .ToList();

        foreach (var group in personGroups)
        {
            var totalPersons = group.Sum(p => (personAgeTypeIds.Contains(p.PersonAgeTypeId) ? p.Persons : 0) ?? 0);

            var priceOption = priceDataList
                .OrderBy(p => p.PersonMax)
                .FirstOrDefault(p => p.InRange(totalPersons));

            if (priceOption == null)
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// Retrieves the inventory and availability of plan rooms for specific dates based on booking search parameters, plan details,
    /// and metadata associated with the rooms.
    /// </summary>
    /// <param name="bookingSearchModel">Contains search criteria including the check-in date, number of rooms, and guests per room.</param>
    /// <param name="plan">The booking plan containing details such as room group limits, sale limits, and booking constraints.</param>
    /// <param name="metaDataOfRoom">Metadata for the rooms including availability, price data, reservations, and applicable person types.</param>
    /// <returns>
    /// A list of <see cref="AppDatePriceOfPlanResponse" /> objects, representing the inventory and room availability
    /// along with their booking statuses for the specified dates.
    /// </returns>
    private static List<AppDatePriceOfPlanResponse> GetAllInventoryAppDates(
        BookingSearchModel bookingSearchModel,
        BookingPlanModel plan,
        BookingMetaDataModel metaDataOfRoom,
        TimeSpan facilityTimeZone
    )
    {
        var roomGroupAppDates = metaDataOfRoom.RoomAppDates.ToList();
        var bookingReservations = metaDataOfRoom.Reservations.ToList();
        var personAgeTypes = metaDataOfRoom.PersonalTypes.ToList();

        int? roomNumberDaySaleLimit = null;
        if (plan is { UseDaySaleLimit: true, PlanDaySaleLimitType: PlanDaySaleLimitTypes.RoomGroup })
        {
            roomNumberDaySaleLimit = plan.RoomNumberDaySaleLimit;
        }

        var appDatesOfReservations = bookingReservations
            .Where(x => x.PlanId == plan.Id)
            .ToList();

        var planRoomGroupSiteAppDatePrices = new List<AppDatePriceOfPlanResponse>();

        long appDateId;

        foreach (var roomGroupAppDate in roomGroupAppDates.OrderBy(a => a.AppDateId))
        {
            appDateId = roomGroupAppDate.AppDateId;

            var reservationPairs = appDatesOfReservations
                .Count(
                    x =>
                        x.BookingDateId == appDateId
                );

            var reservedPlanNumber = bookingReservations
                .Count(
                    x =>
                        x.BookingDateId == appDateId
                        && x.RoomGroupId == roomGroupAppDate.RoomGroupId
                        && x.BookingIsReserved
                        && x.PlanId == plan.Id
                );

            var reservedNumber = bookingReservations.Count(
                x => x.BookingDateId == appDateId
                    && x.RoomGroupId == roomGroupAppDate.RoomGroupId
                    && x.PlanId != plan.Id
            );

            var remainNumber = roomGroupAppDate.SellNumber - reservedNumber;

            if (roomNumberDaySaleLimit != null && roomNumberDaySaleLimit < remainNumber)
            {
                remainNumber = roomNumberDaySaleLimit.Value;
            }

            remainNumber -= reservedPlanNumber;

            var appDatePriceOfPlanResponse = new AppDatePriceOfPlanResponse(
                appDateId,
                remainNumber
            ) { RoomGroupId = roomGroupAppDate.RoomGroupId };

            var appDatePriceStatusModel = IsCheckDayBooking(
                bookingSearchModel,
                plan,
                roomGroupAppDate,
                appDatePriceOfPlanResponse,
                personAgeTypes,
                [.. metaDataOfRoom.PriceData],
                reservationPairs
            );

            var bookingTime = DateTime.UtcNow.Add(facilityTimeZone).TimeOfDay;
            var checkDay = AppDate.GetId(DateTime.UtcNow.Add(facilityTimeZone)) < appDatePriceOfPlanResponse.AppDateId;
            var isBookingTimeAvailable = checkDay || plan.CheckInEnd is null || bookingTime <= plan.CheckInEnd;

            if (!isBookingTimeAvailable)
            {
                continue;
            }

            appDatePriceOfPlanResponse.Status = appDatePriceStatusModel;
            planRoomGroupSiteAppDatePrices.Add(appDatePriceOfPlanResponse);
        }

        return planRoomGroupSiteAppDatePrices;
    }

    /// <summary>
    /// Calculates the total number of guests across all rooms for a specified application date or reservation date.
    /// </summary>
    /// <param name="appDateId">The identifier of the application date for the room.</param>
    /// <param name="reservationDate">The date when the reservation is being made.</param>
    /// <param name="reservationPersonRequest">The detailed list of persons grouped by room and application date used for reservations.</param>
    /// <returns>The total number of guests for the specified application date or reservation date.</returns>
    private static int NumberOfGuestsAllRoom(
        long appDateId,
        long reservationDate,
        List<PersonOfBookingSearchModel> reservationPersonRequest
    )
    {
        var numberPerson = reservationPersonRequest
            .Where(a => a.Persons is > 0)
            .Where(a => a.AppDateId == appDateId)
            .Sum(a => a.Persons!.Value);

        if (numberPerson is 0)
        {
            return reservationPersonRequest
                .Where(a => a.Persons is > 0)
                .Where(a => a.AppDateId == reservationDate)
                .Sum(a => a.Persons!.Value);
        }

        return numberPerson;
    }

    /// <summary>
    /// Calculates the maximum number of guests allowed per room based on the application date, reservation date, booking search data,
    /// and age type constraints.
    /// </summary>
    /// <param name="appDateId">The application date identifier for the room.</param>
    /// <param name="reservationDate">The date the reservation is being made.</param>
    /// <param name="reservationPersonRequest">The booking search model containing a list of person data grouped by room.</param>
    /// <param name="personAgeTypeIdMaxGuest">The list of person age type IDs used to determine guest eligibility for maximum capacity.</param>
    /// <returns>
    /// An integer indicating the maximum number of guests allowed in a room for the specific application or reservation date.
    /// Returns 0 if no valid guests are found.
    /// </returns>
    private static int MaxNumberOfGuestsPerRoom(
        long appDateId,
        long reservationDate,
        List<PersonOfBookingSearchModel> reservationPersonRequest,
        List<long> personAgeTypeIdMaxGuest
    )
    {
        var maxPersons = reservationPersonRequest
            .Where(a => a.AppDateId == appDateId)
            .Where(a => a.Persons is > 0)
            .Where(a => personAgeTypeIdMaxGuest.Contains(a.PersonAgeTypeId))
            .GroupBy(a => a.RoomGroupIndex)
            .Select(g => g.Sum(b => b.Persons!.Value))
            .DefaultIfEmpty(0)
            .Max();

        if (maxPersons is 0)
        {
            return reservationPersonRequest
                .Where(a => a.AppDateId == reservationDate)
                .Where(a => a.Persons is > 0)
                .Where(a => personAgeTypeIdMaxGuest.Contains(a.PersonAgeTypeId))
                .GroupBy(a => a.RoomGroupIndex)
                .Select(g => g.Sum(b => b.Persons!.Value))
                .DefaultIfEmpty(0)
                .Max();
        }

        return maxPersons;
    }

    /// <summary>
    /// Determines whether the number of reserved rooms for a given plan is within the room group’s day sale limit.
    /// </summary>
    /// <param name="reservedNumber">The number of rooms already reserved for the plan.</param>
    /// <param name="plan">The booking plan containing the day sale limit configuration.</param>
    /// <returns>
    /// Returns true if the plan does not enforce a day sale limit, the limit is not configured by room group, or the day sale limit is null;
    /// otherwise, returns true if the reserved number of rooms is less than the configured day sale limit, and false otherwise.
    /// </returns>
    private static bool IsDaySaleLimitReservedNumber(
        int reservedNumber,
        BookingPlanModel plan
    )
    {
        if (!plan.UseDaySaleLimit || plan.PlanDaySaleLimitType != PlanDaySaleLimitTypes.RoomGroup || plan.RoomNumberDaySaleLimit == null)
        {
            return true;
        }

        return plan.RoomNumberDaySaleLimit.Value > reservedNumber;
    }

    /// <summary>
    /// Determines if the number of reservation pairs for a plan is within the allowed group day sale limit.
    /// </summary>
    /// <param name="reservationPairs">The number of reservation pairs for the specified date.</param>
    /// <param name="plan">The booking plan containing group day sale limit configuration.</param>
    /// <returns>
    /// <c>true</c> if the plan does not use day sale limit, is not of type Pair, or the group number day sale limit is not set;
    /// otherwise, <c>true</c> if the number of reservation pairs is less than the group number day sale limit; <c>false</c> otherwise.
    /// </returns>
    /// <example>
    /// <code>
    /// var isWithinLimit = IsDaySaleLimitReservedPairs(3, plan);
    /// </code>
    /// </example>
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

    /// <summary>
    /// Validates whether the total number of persons for a plan falls within the acceptable limits.
    /// </summary>
    /// <param name="allPersons">Total number of persons to be validated for the plan's constraints.</param>
    /// <param name="plan">The booking plan which includes limits for the number of acceptable persons.</param>
    /// <returns>
    /// Returns true if the plan does not enforce person limits or the total number of persons is within the defined range; otherwise, false.
    /// </returns>
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

    /// <summary>
    /// Determines if a plan room is available for booking on a specific day considering multiple conditions.
    /// </summary>
    /// <param name="bookingSearchModel">Booking search parameters such as room and guest details.</param>
    /// <param name="plan">The booking plan containing pricing, availability, and constraints.</param>
    /// <param name="roomGroupAppDate">Room group details including capacity and availability for the specific date.</param>
    /// <param name="appDatePriceOfPlanResponse">Price and availability details for the given date of the plan.</param>
    /// <param name="personAgeTypesOfSitesInPlanRooms">List of person age types specific to the site and plan rooms.</param>
    /// <param name="priceDataList">List of price data models representing room pricing information for different dates.</param>
    /// <param name="reservationPairs">Number of reservations made for the given date in relation to pairing limits.</param>
    /// <returns>Returns the status of the day including whether it is bookable, room availability, and constraints validation.</returns>
    private static AppDatePriceStatusSearchModel IsCheckDayBooking(
        BookingSearchModel bookingSearchModel,
        BookingPlanModel plan,
        BookingMetaRoomAppDateModel roomGroupAppDate,
        AppDatePriceOfPlanResponse appDatePriceOfPlanResponse,
        List<BookingMetaPersonTypeModel> personAgeTypesOfSitesInPlanRooms,
        List<BookingMetaPriceDataModel> priceDataList,
        int reservationPairs
    )
    {
        var appDateId = appDatePriceOfPlanResponse.AppDateId;
        var remainNumber = appDatePriceOfPlanResponse.RemainNumber;
        var roomNumber = bookingSearchModel.RoomNumber;
        var reservationDate = bookingSearchModel.CheckInDate;

        var reservationPersonRequest = bookingSearchModel.GuestsPerRoom is not null
            ? bookingSearchModel.GuestsPerRoom!.ToList()
            : [];

        var personAgeTypeIds = personAgeTypesOfSitesInPlanRooms
            .Select(x => x.PersonAgeTypeId)
            .ToList();

        var personAgeTypeIdsToCheck = bookingSearchModel.GuestsPerRoom!
            .Where(x => x.Persons > 0)
            .Select(x => x.PersonAgeTypeId)
            .ToList();

        var allPersonAgeTypeExist = personAgeTypeIdsToCheck.TrueForAll(
            id => personAgeTypeIds.Contains(id)
        );

        var personAgeTypeIdMaxGuest = personAgeTypesOfSitesInPlanRooms
            .Where(
                x => (x.PersonAgeTypeIsMain ?? false)
                    || (x.PersonAgeTypeFoodBed ?? 0) >= FoodBeds.Bed
            )
            .Select(x => x.PersonAgeTypeId)
            .ToList();

        var numberGuestAll = NumberOfGuestsAllRoom(
            appDateId,
            reservationDate,
            reservationPersonRequest
        );
        var numberOfMaxGuest = MaxNumberOfGuestsPerRoom(
            appDateId,
            reservationDate,
            reservationPersonRequest,
            personAgeTypeIdMaxGuest
        );

        if (priceDataList is not { Count: > 0 })
        {
            return new AppDatePriceStatusSearchModel();
        }

        var existingPrice = priceDataList.ToList()
            .Exists(
                x => x.Price!.HasValue
                    && x.Price!.Value > 0
                    && x.AppDateId == appDateId
            );

        var isAcceptDate = plan.CanAcceptDate(appDateId);
        var isBookingReceptionDate = plan.CanBookingReceptionDate(appDateId);
        var isRoomAvailable = remainNumber is > 0 && remainNumber >= roomNumber;
        var isDayBookable = existingPrice
            && allPersonAgeTypeExist
            && isRoomAvailable
            && numberOfMaxGuest <= roomGroupAppDate.RoomCapacityMax
            && !roomGroupAppDate.IsNotSelled
            && IsDaySaleLimitReservedNumber(roomGroupAppDate.ReservedNumber, plan)
            && IsDaySaleLimitReservedPairs(reservationPairs, plan)
            && IsDaySaleLimitAllPersons(numberGuestAll, plan)
            && plan.CanDisplayDate(appDateId);

        var isRoomUnderRequested = remainNumber is > 0 && remainNumber < roomNumber && existingPrice;

        var appDatePriceStatus = new AppDatePriceStatusSearchModel
        {
            IsAcceptDate = isAcceptDate,
            IsRoomAvailable = isRoomAvailable && isBookingReceptionDate,
            IsDayBookable = isDayBookable,
            IsRoomUnderRequested = isRoomUnderRequested
        };

        return appDatePriceStatus;
    }

    /// <summary>
    /// Converts a collection of person data from booking search models into a list of booking price request models.
    /// </summary>
    /// <param name="models">Collection of person data as booking search models.</param>
    /// <param name="appDateId">The base application date identifier for calculating individual record dates.</param>
    /// <returns>A list of person data tailored for booking price requests, or an empty list if input is null.</returns>
    private static List<PersonOfBookingPriceRequest> ToPriceRequests(
        IEnumerable<PersonOfBookingSearchModel>? models,
        long appDateId
    )
    {
        return models?.Select(
                    model => new PersonOfBookingPriceRequest
                    {
                        AppDateId = appDateId + model.RestIndex,
                        RestIndex = model.RestIndex,
                        RoomGroupIndex = model.RoomGroupIndex,
                        PersonAgeTypeId = model.PersonAgeTypeId,
                        Persons = model.Persons,
                        MalePersons = model.MalePersons,
                        FemalePersons = model.FemalePersons
                    }
                )
                .ToList()
            ?? [];
    }

    /// <summary>
    /// Combines and orders booking plans by their display order in descending order.
    /// </summary>
    /// <param name="plans">The collection of <see cref="BookingSearchByPlanResponse"/> to be ordered.</param>
    /// <returns>
    /// A list of <see cref="BookingSearchByPlanResponse"/> ordered by <c>DisplayOrder</c> descending.
    /// </returns>
    /// <example>
    /// <code>
    /// var orderedPlans = CombineAndOrderPlans(plans);
    /// </code>
    /// </example>
    private static List<BookingSearchByPlanResponse> CombineAndOrderPlans(
        IEnumerable<BookingSearchByPlanResponse> plans
    )
    {
        return
        [
            .. plans
                .OrderByDescending(x => x.DisplayOrder)
        ];
    }

    /// <summary>
    /// Calculates and sets the minimum total price for each plan in the provided list, based on the booking search payload.
    /// </summary>
    /// <param name="plans">The list of <see cref="BookingSearchByPlanResponse"/> to update with minimum prices.</param>
    /// <param name="payload">The <see cref="BookingSearchModel"/> containing the date range for price calculation.</param>
    /// <remarks>
    /// This method updates the <c>MinTotalPrice</c> property of each plan with the minimum total price found within the specified date range.
    /// </remarks>
    /// <example>
    /// <code>
    /// CalculateMinPrices(plans, bookingSearchModel);
    /// </code>
    /// </example>
    private static void CalculateMinPrices(
        List<BookingSearchByPlanResponse> plans,
        BookingSearchModel payload
    )
    {
        plans.ForEach(
            item =>
            {
                var displayCheckInDate = payload.DisplayCheckInDate ?? payload.CheckInDate;
                var displayCheckOutDate = payload.DisplayCheckOutDate ?? payload.CheckOutDate;

                var minTotalPrice = item.Rooms
                    .SelectMany(x => x.AppDatePrices)
                    .Where(
                        x => x.AppDateId >= displayCheckInDate
                            && x.AppDateId <= displayCheckOutDate
                    )
                    .Select(x => x.TotalPrice)
                    .DefaultIfEmpty(0)
                    .Min();

                item.MinTotalPrice = minTotalPrice;
            }
        );
    }
}
