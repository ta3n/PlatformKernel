using Liberty.Reservation.Application.Constants;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;
using Liberty.Reservation.Application.Domains.Repositories.Interfaces;
using Liberty.Reservation.Application.Domains.Services.Interfaces;
using Liberty.Reservation.Application.Exceptions;
using Liberty.Reservation.Application.Models;
using Liberty.Reservation.Application.Models.Requests;
using Liberty.Reservation.Application.Models.Responses;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ReservationEntity = Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data.Reservation;

namespace Liberty.Reservation.Application.Domains.Services;

public class BookingCheckAvailableService(
    IServiceProvider serviceProvider,
    IDbContextFactory<DbContext> dbContextFactory,
    IBookingReservationRepository bookingReservationRepo,
    IBookingDataPlanRepository bookingDataPlanRepo,
    IBookingSystemConfigRepository bookingSystemConfigRepo
) : IBookingCheckAvailableService
{
    public async Task<ReservationEntity> GetReservationByUserAsync(
        long reservationId,
        string? userCode,
        CancellationToken cancellationToken = default
    )
    {
        var queryable = bookingReservationRepo
            .GetQueryableWithAsNoTracking()
            .Include(x => x.Reserver)
            .Include(x => x.MainUser)
            .Include(x => x.ReservationQuestions!)
            .ThenInclude(t => t.Question)
            .Include(x => x.Plan!)
            .ThenInclude(x => x.PlanMealTypes!)
            .ThenInclude(x => x.MealType)
            .Include(x => x.Plan!)
            .ThenInclude(p => p.Cancellation)
            .ThenInclude(c => c!.CancellationCancellationDatas!)
            .ThenInclude(ccd => ccd.CancellationData)
            .Include(x => x.Facility)
            .Where(x => x.UserCode == userCode)
            .Where(x => x.Id == reservationId);

        var existingReservation = await queryable.FirstOrDefaultAsync(cancellationToken)
            ?? throw new ReservationNotfoundException();

        return existingReservation;
    }

    public async Task<ReservationBasicModel> GetReservationBasicByUserAsync(
        long reservationId,
        string? userCode,
        CancellationToken cancellationToken = default
    )
    {
        var queryable = bookingReservationRepo
            .GetQueryableWithAsNoTracking()
            .Where(x => x.UserCode == userCode)
            .Where(x => x.Id == reservationId)
            .Select(
                reservation => new ReservationBasicModel(
                    reservation.Id,
                    reservation.Code,
                    reservation.UserCode,
                    reservation.FacilityId,
                    reservation.SiteId,
                    reservation.PlanId,
                    reservation.RoomGroupId,
                    reservation.CheckInDate,
                    reservation.RestNumber,
                    reservation.RoomNumber,
                    reservation.BookingData!.LanguageCode,
                    reservation.IsUnConfirmed,
                    reservation.ReservationState,
                    reservation.ReservationRoomGroupAppDateOptionItems!
                        .Select(
                            optionItem => new OptionOfBookingSearchModel
                            {
                                AppDateId = optionItem.BookingDateId,
                                RoomGroupIndex = optionItem.RoomGroupIndex,
                                OptionItemId = optionItem.OptionItemId,
                                Number = optionItem.Number
                            }
                        )
                        .ToList()
                )
            );

        var reservationBasic = await queryable.FirstOrDefaultAsync(cancellationToken)
            ?? throw new ReservationNotfoundException();

        return reservationBasic;
    }

    public async Task<ReservationEntity> GetReservationByFacilityAsync(
        long reservationId,
        long facilityId,
        CancellationToken cancellationToken = default
    )
    {
        var queryable = bookingReservationRepo
            .GetQueryableWithAsNoTracking()
            .Include(x => x.Reserver)
            .Include(x => x.MainUser)
            .Include(x => x.ReservationQuestions!)
            .ThenInclude(t => t.Question)
            .Include(x => x.Plan)
            .ThenInclude(p => p!.Cancellation)
            .ThenInclude(c => c!.CancellationCancellationDatas!)
            .ThenInclude(ccd => ccd.CancellationData)
            .Include(x => x.Facility)
            .Where(x => x.FacilityId == facilityId)
            .Where(x => x.Id == reservationId);

        var existingReservation = await queryable.FirstOrDefaultAsync(cancellationToken)
            ?? throw new ReservationNotfoundException(reservationId);

        return existingReservation;
    }

    public async Task<ReservationBasicModel> GetReservationBasicByFacilityAsync(
        long reservationId,
        long facilityId,
        CancellationToken cancellationToken = default
    )
    {
        var queryable = bookingReservationRepo
            .GetQueryableWithAsNoTracking()
            .Where(x => x.FacilityId == facilityId)
            .Where(x => x.Id == reservationId)
            .Select(
                reservation => new ReservationBasicModel(
                    reservation.Id,
                    reservation.Code,
                    reservation.UserCode,
                    reservation.FacilityId,
                    reservation.SiteId,
                    reservation.PlanId,
                    reservation.RoomGroupId,
                    reservation.CheckInDate,
                    reservation.RestNumber,
                    reservation.RoomNumber,
                    reservation.BookingData!.LanguageCode,
                    reservation.IsUnConfirmed,
                    reservation.ReservationState,
                    new List<OptionOfBookingSearchModel>()
                )
            );

        var reservationBasic = await queryable.FirstOrDefaultAsync(cancellationToken)
            ?? throw new ReservationNotfoundException();

        return reservationBasic;
    }

    public async Task<bool> CheckAdjustAvailableAsync(
        long facilityId,
        long planId,
        BookingAdjustRequest adjustRequest,
        CancellationToken cancellationToken = default
    )
    {
        if (!adjustRequest.IsAgree)
        {
            return false;
        }

        var existingPlan = await bookingDataPlanRepo.GetByIdAsync(
                facilityId,
                planId,
                cancellationToken
            )
            ?? throw new ReservationInvalidException("Plan not found");

        if (!existingPlan.CanNumberOfStayLimitMax(adjustRequest.NumberOfNights))
        {
            throw new ReservationOverNumberOfStayLimitException(adjustRequest.NumberOfNights);
        }

        var checks = new List<Func<long, BookingAdjustRequest, CancellationToken, Task<bool>>>
        {
            CheckAvailableReserverAsync,
            CheckAvailableMainUserAsync,
            CheckAvailableRoomPeoplesAsync,
            CheckAvailableNightOptionsAsync,
            CheckAvailableRoomRepresentativesAsync
        };

        foreach (var check in checks)
        {
            if (!await check(facilityId, adjustRequest, cancellationToken))
            {
                return false;
            }
        }

        return true;
    }

    public Task<bool> CheckAvailableReserverAsync(
        long facilityId,
        BookingAdjustRequest adjustRequest,
        CancellationToken cancellationToken = default
    )
    {
        return Task.FromResult(true);
    }

    public Task<bool> CheckAvailableMainUserAsync(
        long facilityId,
        BookingAdjustRequest adjustRequest,
        CancellationToken cancellationToken = default
    )
    {
        return Task.FromResult(true);
    }

    public async Task<bool> CheckAvailableRoomPeoplesAsync(
        long facilityId,
        BookingAdjustRequest adjustRequest,
        CancellationToken cancellationToken = default
    )
    {
        if (adjustRequest.NightPeoples is null)
        {
            return true;
        }

        var bookingPersonAgeTypeRepo = serviceProvider.GetRequiredService<IBookingPersonAgeTypeRepository>();

        var personAgeTypeIds = adjustRequest.GetPersonAgeTypeIds();

        var existingCount = await bookingPersonAgeTypeRepo.CountByIdsAsync(
            facilityId,
            personAgeTypeIds,
            cancellationToken
        );

        var isAvailable = existingCount == personAgeTypeIds.Length;
        if (!isAvailable)
        {
            throw new ReservationInvalidException("Person age types not found");
        }

        return isAvailable;
    }

    public async Task<bool> CheckAvailableNightOptionsAsync(
        long facilityId,
        BookingAdjustRequest adjustRequest,
        CancellationToken cancellationToken = default
    )
    {
        if (adjustRequest.NightOptions is null)
        {
            return true;
        }

        var bookingOptionItemRepo = serviceProvider.GetRequiredService<IBookingOptionItemRepository>();

        var optionItemIds = adjustRequest.GetOptionItemIds();

        var existingCount = await bookingOptionItemRepo.CountAvailableByIdsAsync(
            facilityId,
            optionItemIds,
            AppDate.GetDateTime(adjustRequest.CheckInDateId),
            cancellationToken
        );

        var isAvailable = existingCount == optionItemIds.Length;
        if (!isAvailable)
        {
            throw new ReservationInvalidException("Option items not found");
        }

        return isAvailable;
    }

    public Task<bool> CheckAvailableRoomRepresentativesAsync(
        long facilityId,
        BookingAdjustRequest adjustRequest,
        CancellationToken cancellationToken = default
    )
    {
        return Task.FromResult(true);
    }

    public async Task<bool> IsNightNumberAsync(
        long planId,
        long roomGroupId,
        long siteId,
        long checkInDate,
        long restNumber,
        CancellationToken cancellationToken = default
    )
    {
        var checkOutDate = AppDate.GetId(
            AppDate.GetDateTime(checkInDate).AddDays(restNumber - 1)
        );

        await using var dbContext = await dbContextFactory.CreateDbContextAsync(cancellationToken);

        var planRoomGroupSiteDatePriceRepo = serviceProvider.GetRequiredService<IPlanRoomGroupSiteAppDatePriceDataRepository>();

        var listPlanRoomGroupSiteAppDatePriceData = await planRoomGroupSiteDatePriceRepo
            .GetQueryableWithAsNoTracking(dbContext)
            .Where(x => x.SiteId == siteId)
            .Where(x => x.Site!.IsEnabled)
            .Where(x => x.PlanId == planId)
            .Where(x => x.Plan!.IsEnabled)
            .Where(x => x.RoomGroupId == roomGroupId)
            .Where(x => x.RoomGroup!.IsEnabled)
            .Where(x => x.DateCalendar >= checkInDate)
            .Where(x => x.DateCalendar <= checkOutDate)
            .Where(x => x.PriceData!.Price > 0)
            .Where(x => x.PriceData!.IsEnabled)
            .Where(x => x.IsEnabled)
            .ToListAsync(cancellationToken);

        var isNumberOfStayLimit = await bookingDataPlanRepo
            .GetQueryableWithAsNoTracking(dbContext)
            .Where(x => x.IsEnabled)
            .Where(x => x.IsOnLinePayment || x.IsOnSidePayment)
            .Where(x => x.Cancellation!.IsEnabled)
            .Where(x => !x.UseDisplayDate || x.DisplayDateStart == null || x.DisplayDateStart <= checkInDate)
            .Where(x => !x.UseDisplayDate || x.DisplayDateEnd == null || x.DisplayDateEnd >= checkOutDate)
            .Where(x => !x.UseAcceptDate || x.AcceptDateStart == null || x.AcceptDateStart <= checkInDate)
            .Where(x => !x.UseAcceptDate || x.AcceptDateEnd == null || x.AcceptDateEnd >= checkOutDate)
            .Where(x => !x.NumberOfStayLimitMax.HasValue || (x.NumberOfStayLimitMax.HasValue && x.NumberOfStayLimitMax >= restNumber))
            .Where(x => !x.NumberOfStayLimitMin.HasValue || (x.NumberOfStayLimitMin.HasValue && x.NumberOfStayLimitMin <= restNumber))
            .AnyAsync(x => x.Id == planId, cancellationToken);

        if (!isNumberOfStayLimit)
        {
            return false;
        }

        var index = 0;
        while (index < restNumber)
        {
            var nextDate = AppDate.GetId(
                AppDate.GetDateTime(checkInDate).AddDays(index)
            );

            var isPrice = listPlanRoomGroupSiteAppDatePriceData.Exists(
                x => x.DateCalendar == nextDate
            );
            if (!isPrice)
            {
                return false;
            }

            index++;
        }

        return true;
    }

    public async Task<bool> IsRoomNumberAsync(
        long planId,
        long roomGroupId,
        long checkInDate,
        long roomNumber,
        long restNumber,
        long reservationId = 0,
        CancellationToken cancellationToken = default
    )
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync(cancellationToken);

        var plan = await GetValidPlanAsync(
            dbContext,
            planId,
            roomNumber,
            reservationId,
            cancellationToken
        );

        if (plan is null)
        {
            return false;
        }

        var roomGroupAppDateRepo = serviceProvider.GetRequiredService<IBookingRoomGroupRepository>();

        var endDateId = AppDate.GetId(
            AppDate.GetDateTime(checkInDate).AddDays(restNumber - 1)
        );

        var roomGroupAppDates = await roomGroupAppDateRepo
            .GetQueryableWithAsNoTracking(dbContext)
            .Where(x => x.Id == roomGroupId)
            .Where(x => x.IsEnabled)
            .Select(
                x =>
                    new RoomGroup
                    {
                        RoomGroupAppDates = x.RoomGroupAppDates!
                            .Where(t => t.AppDateId >= checkInDate)
                            .Where(t => t.AppDateId <= endDateId)
                            .Where(t => !t.IsNotSelled)
                            .Where(t => t.SellNumber >= roomNumber)
                            .Select(
                                y =>
                                    new RoomGroupAppDate
                                    {
                                        AppDateId = y.AppDateId,
                                        SellNumber = y.SellNumber,
                                        ReservationPlanRoomGroupAppDates = x.ReservationPlanRoomGroupAppDates!
                                            .Where(t => t.BookingDateId >= checkInDate)
                                            .Where(t => t.BookingDateId <= endDateId)
                                            .Where(
                                                z => z.Reservation!.ReservationState == ReservationStatus.Confirmed
                                                    || z.Reservation!.ReservationState == ReservationStatus.Reserved
                                                    || z.Reservation!.ReservationState == ReservationStatus.Modified
                                            )
                                            .Select(
                                                t =>
                                                    new ReservationPlanRoomGroupAppDate
                                                    {
                                                        ReservationId = t.ReservationId,
                                                        BookingDateId = t.BookingDateId,
                                                        PlanId = t.PlanId,
                                                        Reservation = new ReservationEntity
                                                        {
                                                            ReservationState = t.Reservation!.ReservationState
                                                        }
                                                    }
                                            )
                                            .ToList()
                                    }
                            )
                            .ToList()
                    }
            )
            .SelectMany(x => x.RoomGroupAppDates!)
            .ToListAsync(cancellationToken);

        var index = 0;
        while (index < restNumber)
        {
            var nextDate = AppDate.GetId(
                AppDate.GetDateTime(checkInDate).AddDays(index)
            );
            var roomGroupAppDate = roomGroupAppDates.Find(
                x => x.AppDateId == nextDate
            );
            var reservedNumber = roomGroupAppDate?.ReservationPlanRoomGroupAppDates?.Count(
                    x => x.BookingDateId == nextDate
                )
                ?? 0;
            var reservedPlanNumber = roomGroupAppDate?.ReservationPlanRoomGroupAppDates?.Count(
                    x => x.BookingDateId == nextDate && x.PlanId == plan.Id
                )
                ?? 0;

            if (reservedPlanNumber >= plan.RoomNumberDaySaleLimit && plan.UseDaySaleLimit)
            {
                return false;
            }

            if (reservationId > 0)
            {
                reservedNumber -= roomGroupAppDate?.ReservationPlanRoomGroupAppDates?.Count(
                        x => x.ReservationId == reservationId && x.BookingDateId == nextDate
                    )
                    ?? 0;
            }

            var sellNumberRemain = (roomGroupAppDate?.SellNumber ?? 0) - reservedNumber;
            var remainNumber = sellNumberRemain;

            if (plan.UseDaySaleLimit)
            {
                var dayLimitRemain = (plan.RoomNumberDaySaleLimit ?? 0) - reservedPlanNumber;
                remainNumber = Math.Min(sellNumberRemain, dayLimitRemain);
            }

            var isRemainNumber = remainNumber >= roomNumber;

            if (!isRemainNumber)
            {
                return false;
            }

            index++;
        }

        return true;
    }

    public async Task<bool> IsReceptionAvailableAsync(
        long planId,
        long checkInDate,
        CancellationToken cancellationToken
    )
    {
        var checkInDateTime = AppDate.GetDateTime(checkInDate);
        var now = DateTime.UtcNow.AddHours(DefaultValues.TimeZoneOffset);

        await using var dbContext = await dbContextFactory.CreateDbContextAsync(cancellationToken);

        var existingPlanQueryable = bookingDataPlanRepo
            .GetQueryableWithAsNoTracking(dbContext)
            .Where(x => x.Id == planId)
            .Select(
                x => new
                {
                    x.Id,
                    x.ReceptionDayLimit,
                    x.ReceptionLimit,
                    x.UseBookingReception,
                    x.BookingReceptionStart,
                    x.BookingReceptionEnd
                }
            );

        var existingPlan = await existingPlanQueryable.FirstOrDefaultAsync(
            cancellationToken
        );
        if (existingPlan is null)
        {
            return false;
        }

        var cancelDayLimit = existingPlan.ReceptionDayLimit ?? 0;
        var cancelLimit = existingPlan.ReceptionLimit ?? TimeSpan.FromHours(24);

        var limit = checkInDateTime;
        limit = limit.AddDays(-cancelDayLimit);
   
        limit += cancelLimit;

        if (now > limit)
        {
            return false;
        }

        if (!existingPlan.UseBookingReception)
        {
            return true;
        }

        var isAvailable = existingPlan.BookingReceptionStart <= checkInDate
            && existingPlan.BookingReceptionEnd >= checkInDate;

        return isAvailable;
    }

    public async Task<(ReservationEntity?, bool)> GetReservationByOrderIdAsync(
        string orderId,
        CancellationToken cancellationToken
    )
    {
        var queryable = bookingReservationRepo
            .GetQueryableWithAsNoTracking()
            .Where(x => x.OrderReservations!.Any(t => t.Order!.ApiIssueCode == orderId));

        var reservation = await GetReservationsAsync(queryable, cancellationToken);
        var isAllowOnlinePayment = await CheckReservationAllowOnlinePaymentAsync(queryable, cancellationToken);

        return (reservation, isAllowOnlinePayment);
    }

    public static async Task<ReservationEntity?> GetReservationsAsync(
        IQueryable<ReservationEntity> queryable,
        CancellationToken cancellationToken = default
    )
    {
        return await queryable
            .Include(x => x.Plan!)
            .ThenInclude(x => x.PlanMealTypes!)
            .ThenInclude(x => x.MealType)
            .Include(x => x.Facility)
            .Include(x => x.Reserver)
            .Include(x => x.MainUser)
            .Include(x => x.ReservationQuestions)
            .Include(x => x.OrderReservations!)
            .ThenInclude(x => x.Order)
            .Include(x => x.Site)
            .SingleOrDefaultAsync(cancellationToken);
    }

    private async Task<bool> CheckReservationAllowOnlinePaymentAsync(
        IQueryable<ReservationEntity> queryable,
        CancellationToken cancellationToken = default
    )
    {
        var systemConfigQueryable = bookingSystemConfigRepo
            .GetQueryableWithAsNoTracking()
            .Select(x => x.CanOnlinePayment);

        var globalCanOnlinePayment = await systemConfigQueryable.SingleOrDefaultAsync(cancellationToken) ?? false;

        queryable = queryable
            .Where(x => x.Facility!.CanOnLinePayment)
            .Where(x => x.Facility!.IsOnLinePayment)
            .Where(x => x.Plan!.IsOnLinePayment);
        var existing = await queryable.AnyAsync(cancellationToken);

        return existing && globalCanOnlinePayment;
    }

    public async Task<ReservationEntity> GetReservationByCodeAsync(
        string code,
        string? userCode,
        CancellationToken cancellationToken = default
    )
    {
        var existingReservation = await bookingReservationRepo
            .GetQueryableWithAsNoTracking()
            .Where(x => x.UserCode == userCode)
            .SingleOrDefaultAsync(
                x => x.Code == code,
                cancellationToken
            );

        return existingReservation ?? throw new ReservationNotfoundException();
    }

    public async Task<bool> IsInvalidOptionItemsAsync(
        BookingPriceRequest bookingPriceRequest,
        long facilityId,
        long reservationId = 0,
        CancellationToken cancellationToken = default
    )
    {
        if (bookingPriceRequest.OptionItems is null or { Count: 0 })
        {
            return false;
        }

        var bookingDataAvailableService = serviceProvider.GetRequiredService<IBookingDataAvailableService>();

        var listSearchOptionItem = bookingPriceRequest.OptionItems;
        var dateEndNight = bookingPriceRequest.GetDateEndNight();
        var optionItemIds = bookingPriceRequest.GetOptionItems();

        var appDatesOfOptionItems = await bookingDataAvailableService.FindAllAppDatesOfOptionItemsAsync(
            facilityId,
            optionItemIds!,
            bookingPriceRequest.CheckInDate,
            dateEndNight,
            reservationId,
            cancellationToken
        );

        var datesOfOptionItems = appDatesOfOptionItems as OptionItemAppDate[] ?? [.. appDatesOfOptionItems];
        if (datesOfOptionItems.Length == 0)
        {
            return false;
        }

        if (reservationId <= 0)
        {
            return listSearchOptionItem
                .GroupBy(
                    x => new
                    {
                        x.AppDateId,
                        x.OptionItemId
                    }
                )
                .Any(
                    group =>
                    {
                        var totalNumber = group.Sum(x => x.Number);

                        var match = Array.Find(
                            datesOfOptionItems,
                            x => x.AppDateId == group.Key.AppDateId
                                && x.OptionItemId == group.Key.OptionItemId
                        );

                        return match is null || totalNumber > match.RemainNumber;
                    }
                );
        }

        var optionMap = new Dictionary<(long AppDateId, long OptionItemId), (int optionNumber, int reservedNumber)>();

        foreach (var option in listSearchOptionItem)
        {
            var key = (option.AppDateId, option.OptionItemId);

            var optionData = Array.Find(
                datesOfOptionItems,
                x => x.AppDateId == option.AppDateId
                    && x.OptionItemId == option.OptionItemId
            );

            var reservedCount = optionData?.ReservationRoomGroupAppDateOptionItems?
                    .Where(x => x.ReservationId == reservationId && x.RoomGroupIndex == option.RoomGroupIndex)
                    .Sum(x => x.Number)
                ?? 0;

            if (!optionMap.TryGetValue(key, out var current))
            {
                current = (0, 0);
                optionMap[key] = current;
            }

            optionMap[key] = (current.optionNumber + option.Number, current.reservedNumber + reservedCount);
        }

        foreach (var option in optionMap)
        {
            var (appDateId, optionItemId) = option.Key;
            var (optionNumber, reservedNumber) = option.Value;

            var matchedDate = Array.Find(
                datesOfOptionItems,
                x => x.AppDateId == appDateId
                    && x.OptionItemId == optionItemId
            );

            if (optionNumber > (matchedDate?.RemainNumber ?? 0) + reservedNumber)
            {
                return true;
            }
        }

        return false;
    }

    public async Task<bool> IsInvalidChangePersonsAsync(
        BookingPriceRequest bookingPriceRequest,
        BookingPlanModel planModel,
        long facilityId,
        long roomGroupId,
        long siteId,
        bool useDailyPerson,
        CancellationToken cancellationToken
    )
    {
        var bookingDataAvailableService = serviceProvider.GetRequiredService<IBookingDataAvailableService>();

        var checkInDate = bookingPriceRequest.CheckInDate;
        var dateEndNight = bookingPriceRequest.GetDateEndNight();

        var appDatesOfRooms = (await bookingDataAvailableService.FindAllAppDatesOfRoomGroupAsync(
            facilityId,
            roomGroupId,
            checkInDate,
            dateEndNight,
            cancellationToken
        )).ToList();

        var personAgeTypesOfSitesInPlanRooms = (await bookingDataAvailableService.FindAllPersonAgeTypesOfSiteInPlanRoomAsync(
            facilityId,
            siteId,
            planModel.Id,
            roomGroupId,
            cancellationToken
        )).ToList();

        var personAgeTypeIdMaxGuest = personAgeTypesOfSitesInPlanRooms
            .Where(x => x.PersonAgeType != null && (x.PersonAgeType.IsMain || (x.PersonAgeType.Meta?.FoodBed ?? 0) >= FoodBeds.Bed))
            .Select(x => x.PersonAgeTypeId)
            .ToList();

        var reservationDate = bookingPriceRequest.CheckInDate;
        var restIndex = 0;

        while (restIndex < bookingPriceRequest.RestNumber)
        {
            var appDateId = AppDate.GetId(
                AppDate.GetDateTime(reservationDate).AddDays(restIndex)
            );

            var capacityMax = appDatesOfRooms.Find(x => x.AppDateId == appDateId)?.RoomGroup?.CapacityMax ?? 0;
            var roomIndex = 0;

            while (roomIndex < bookingPriceRequest.RoomNumber)
            {
                var totalGuests = NumberOfGuestsAllRoom(
                    appDateId,
                    reservationDate,
                    bookingPriceRequest.GuestsPerRoom
                );

                var maxGuestsPerRoom = MaxNumberOfGuestsPerRoom(
                    appDateId,
                    reservationDate,
                    roomIndex,
                    bookingPriceRequest.GuestsPerRoom,
                    personAgeTypeIdMaxGuest
                );
                var exceedsCapacity = maxGuestsPerRoom > capacityMax;
                var violatesLimit = !IsDaySaleLimitAllPersons(totalGuests, planModel);

                if (exceedsCapacity || violatesLimit)
                {
                    return true;
                }

                roomIndex++;
            }

            if (!useDailyPerson)
            {
                return false;
            }

            restIndex++;
        }

        return false;

        static int NumberOfGuestsAllRoom(
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

        static int MaxNumberOfGuestsPerRoom(
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

        static bool IsDaySaleLimitAllPersons(
            int allPersons,
            BookingPlanModel plan
        )
        {
            return !plan.UseAcceptPersonNumber
                || (plan is { AcceptPersonNumberMax: not null, AcceptPersonNumberMin: not null }
                    && plan.AcceptPersonNumberMax.Value >= allPersons
                    && plan.AcceptPersonNumberMin.Value <= allPersons);
        }
    }

    private async Task<Plan?> GetValidPlanAsync(
        DbContext dbContext,
        long planId,
        long roomNumber,
        long reservationId,
        CancellationToken cancellationToken
    )
    {
        var planQuery = bookingDataPlanRepo
            .GetQueryableWithAsNoTracking(dbContext)
            .Where(x => x.IsEnabled)
            .Where(x => x.IsOnLinePayment || x.IsOnSidePayment)
            .Where(x => x.Cancellation!.IsEnabled)
            .Where(
                x =>
                    !x.UseDaySaleLimit
                    || x.PlanDaySaleLimitType != PlanDaySaleLimitTypes.RoomGroup
                    || (x.UseDaySaleLimit
                        && x.PlanDaySaleLimitType == PlanDaySaleLimitTypes.RoomGroup
                        && x.RoomNumberDaySaleLimit >= roomNumber)
            );

        if (reservationId > 0)
        {
            planQuery = planQuery.Where(
                x => x.FacilityPlans!.Any(y => y.Facility!.CanAddRoomOnModify)
            );
        }

        return await planQuery.FirstOrDefaultAsync(x => x.Id == planId, cancellationToken);
    }

    public static bool IsWithinOnlinePaymentLimit(
        DateTime checkInDate
    )
    {
        var now = DateTime.UtcNow.AddHours(DefaultValues.TimeZoneOffset).Date;
        return checkInDate.Date <= now.AddDays(DefaultValues.OnlinePaymentDayLimit);
    }
}
