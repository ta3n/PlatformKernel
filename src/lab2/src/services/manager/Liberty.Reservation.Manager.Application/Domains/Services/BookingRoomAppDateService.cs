using Liberty.Reservation.Application.Exceptions;
using Liberty.Reservation.Application.Models.Requests;
using Liberty.Reservation.Manager.Application.Models;

namespace Liberty.Reservation.Manager.Application.Domains.Services;

public class BookingRoomAppDateService(
    IPlanService planService
) : IBookingRoomAppDateService
{
    public async Task<IEnumerable<BookingRoomAppDateModel>> GetAllRoomAppDates(
        BookingCreateRequest bookingCreateRequest,
        BookingDataAvailableModel bookingDataAvailable,
        CancellationToken cancellationToken = default
    )
    {
        var bookingAdjustRequest = bookingCreateRequest.Adjust;

        var existingPlan = await planService.FindByIdAsync(
            bookingCreateRequest.PlanId,
            cancellationToken
        );

        // An exception is made if the number of nights exceeds the number of nights that can be accepted in the first place.
        if (!existingPlan.CanNumberOfStayLimitMax(bookingAdjustRequest.NumberOfNights))
        {
            throw new ReservationOverNumberOfStayLimitException(bookingAdjustRequest.NumberOfNights);
        }

        // Setting the maximum number of rooms for sale in a plan
        var roomNumberDaySaleLimit = GetRoomNumberDaySaleLimit(existingPlan);

        // Group the number of groups currently booked for this plan by day
        var reservationPairsGroup = bookingDataAvailable
            .AppDatesOfReservations
            .GroupBy(x => x.AppDateId)
            .ToList();

        // Add price information to vacancy information
        // I also want to make changes, so I use the inner class
        var bookingRoomAppDates = CreateBookingRoomAppDates(
            roomNumberDaySaleLimit,
            reservationPairsGroup,
            bookingCreateRequest,
            bookingDataAvailable
        );

        SetRepresentativeAmount(
            bookingRoomAppDates,
            bookingCreateRequest,
            bookingDataAvailable
        );

        CheckConsecutiveNights(
            bookingRoomAppDates,
            existingPlan,
            bookingCreateRequest,
            bookingDataAvailable
        );

        return bookingRoomAppDates;
    }

    private static int? GetRoomNumberDaySaleLimit(
        Plan existingPlan
    )
    {
        return existingPlan is { UseDaySaleLimit: true, PlanDaySaleLimitType: PlanDaySaleLimitTypes.RoomGroup }
            ? existingPlan.RoomNumberDaySaleLimit
            : null;
    }

    private static List<BookingRoomAppDateModel> CreateBookingRoomAppDates(
        int? roomNumberDaySaleLimit,
        List<IGrouping<long, ReservationPlanRoomGroupAppDate>> reservationPairsGroup,
        BookingCreateRequest bookingCreateRequest,
        BookingDataAvailableModel bookingDataAvailable
    )
    {
        var bookingRoomAppDates = new List<BookingRoomAppDateModel>();

        var appDatesOfRooms = bookingDataAvailable.AppDatesOfRoom
            .Where(x => x.RoomGroupId == bookingCreateRequest.RoomGroupId)
            .OrderBy(x => x.AppDateId);

        foreach (var roomGroupAppDate in appDatesOfRooms)
        {
            var appDateId = roomGroupAppDate.AppDateId;

            // Current number of reservations for this day's plan
            var reservationPairs = reservationPairsGroup.Count(
                x => x.Key == appDateId
            );

            // If the plan has a maximum number of rooms for sale, make sure it does not exceed the number of remaining rooms
            var remainNumber = roomGroupAppDate.RemainNumber;

            if (roomNumberDaySaleLimit != null)
            {
                remainNumber = remainNumber > roomNumberDaySaleLimit ? roomNumberDaySaleLimit : remainNumber;
            }

            var bookingRoomAppDate = new BookingRoomAppDateModel
            {
                AppDateId = appDateId,
                SiteId = bookingCreateRequest.SiteId,
                PlanId = bookingCreateRequest.PlanId,
                RemainNumber = remainNumber,
                IsNotSelled = roomGroupAppDate.IsNotSelled,
                ReservedNumber = roomGroupAppDate.ReservedNumber,
                ReservationPairs = reservationPairs,
                Price = null,
                RoomGroupInfo = new()
                {
                    Id = roomGroupAppDate.RoomGroupId,
                    Name = roomGroupAppDate.RoomGroup?.Name,
                    CapacityMax = roomGroupAppDate.RoomGroup?.CapacityMax,
                    CapacityMin = roomGroupAppDate.RoomGroup?.CapacityMin
                }
            };

            bookingRoomAppDates.Add(bookingRoomAppDate);
        }

        return bookingRoomAppDates;
    }

    private static void SetRepresentativeAmount(
        List<BookingRoomAppDateModel> bookingRoomAppDates,
        BookingCreateRequest bookingCreateRequest,
        BookingDataAvailableModel bookingDataAvailable
    )
    {
        var bookingAdjustRequest = bookingCreateRequest.Adjust;

        foreach (var bookingRoomAppDate in bookingRoomAppDates)
        {
            var appDateId = bookingRoomAppDate.AppDateId;
            var numberOfAdults = bookingAdjustRequest.GetNumberOfAdultsByDateId(appDateId) ?? 0;
            var prevDay = (int)(appDateId - AppDate.GetId(bookingCreateRequest.BookingDate));

            var planRoomGroupSiteAppDate = bookingDataAvailable.AppDatesOfSiteInPlanRoom
                .Where(x => x.SiteId == bookingCreateRequest.SiteId)
                .Where(x => x.PlanId == bookingCreateRequest.PlanId)
                .Where(x => x.RoomGroupId == bookingCreateRequest.RoomGroupId)
                .SingleOrDefault(x => x.DateCalendar == appDateId);

            var price = GetPrice(
                appDateId,
                numberOfAdults,
                bookingDataAvailable.AppDatePriceDataOfSiteInPlanRoom
            );

            var discountedPrice = price;

            if (planRoomGroupSiteAppDate != null)
            {
                discountedPrice = GetDiscountedPrice(
                    numberOfAdults,
                    prevDay,
                    price,
                    bookingCreateRequest,
                    planRoomGroupSiteAppDate,
                    bookingDataAvailable.DiscountDataOfSiteInPlanRoom
                );

                if (discountedPrice is < 0)
                {
                    throw new ReservationPriceMinusException();
                }
            }

            bookingRoomAppDate.Price = discountedPrice;
        }
    }

    private static void CheckConsecutiveNights(
        List<BookingRoomAppDateModel> bookingRoomAppDates,
        Plan existingPlan,
        BookingCreateRequest bookingCreateRequest,
        BookingDataAvailableModel bookingDataAvailable
    )
    {
        var bookingAdjustRequest = bookingCreateRequest.Adjust;

        foreach (var bookingRoomAppDate in bookingRoomAppDates)
        {
            var appDateId = bookingRoomAppDate.AppDateId;
            var bookingRoomAppDateByDateId = bookingRoomAppDates.SingleOrDefault(
                x => x.AppDateId == appDateId
            );

            for (var nightIndex = 0; nightIndex < bookingAdjustRequest.NumberOfNights; nightIndex++)
            {
                var appDateIdOfNight = appDateId + nightIndex;

                var reservationServiceException = CheckCanNight(
                    appDateIdOfNight,
                    existingPlan,
                    bookingRoomAppDateByDateId,
                    bookingCreateRequest,
                    bookingDataAvailable
                );

                if (reservationServiceException == null)
                {
                    continue;
                }

                reservationServiceException.AppDateId = appDateId;
                reservationServiceException.RestIndex = nightIndex;
                bookingRoomAppDate.Exceptions.Add(reservationServiceException);
            }
        }
    }

    private static int? GetPrice(
        long appDateId,
        int? adultPersonAgePersons,
        IEnumerable<PlanRoomGroupSiteAppDatePriceData> appDatePriceData
    )
    {
        var price = appDatePriceData
            .Where(x => x.DateCalendar == appDateId)
            .FirstOrDefault(x => x.PriceData!.InRange(adultPersonAgePersons))
            ?.PriceData
            ?.Price;

        return price;
    }

    private static int? GetDiscountedPrice(
        int numberOfAdultPersons,
        int prevDay,
        int? price,
        BookingCreateRequest bookingCheckData,
        PlanRoomGroupSiteAppDate planRoomGroupSiteAppDate,
        IEnumerable<PlanRoomGroupSiteDiscountData> discountData
    )
    {
        var discountedPrice = price;

        if (!planRoomGroupSiteAppDate.UseAutoDiscount)
        {
            return discountedPrice;
        }

        var planRoomGroupSiteDiscountData = discountData
            .Where(a => a.PlanId == bookingCheckData.PlanId)
            .Where(a => a.RoomGroupId == bookingCheckData.RoomGroupId)
            .Where(a => a.SiteId == bookingCheckData.SiteId)
            .Where(a => a.DiscountData!.InRange(numberOfAdultPersons))
            .FirstOrDefault(a => a.DiscountData!.InPrevDay(prevDay));

        var discount = planRoomGroupSiteDiscountData?.DiscountData?.GetDiscount(price);
        if (discount != null)
        {
            discountedPrice -= discount.Value;
        }

        return discountedPrice;
    }

    private static ReservationServiceException? CheckCanNight(
        long appDateId,
        Plan existingPlan,
        BookingRoomAppDateModel? bookingRoomAppDateByDateId,
        BookingCreateRequest bookingCreateRequest,
        BookingDataAvailableModel bookingDataAvailable
    )
    {
        var bookingDateId = AppDate.GetId(bookingCreateRequest.BookingDate);

        if (IsDateOutOfRange(bookingDateId, appDateId))
        {
            return new ReservationOutOfDateException();
        }

        var bookingAdjustRequest = bookingCreateRequest.Adjust;

        var isPriceValid = IsPriceValid(
            appDateId,
            bookingCreateRequest,
            bookingDataAvailable
        );
        if (!isPriceValid)
        {
            return new ReservationServiceHasNoPaymentResultException();
        }

        if (bookingRoomAppDateByDateId == null)
        {
            return new ReservationNoDataException();
        }

        var roomDataException = CheckRoomData(
            bookingRoomAppDateByDateId,
            bookingAdjustRequest
        );
        if (roomDataException != null)
        {
            return roomDataException;
        }

        var planConstraintException = CheckPlanConstraints(
            appDateId,
            existingPlan,
            bookingRoomAppDateByDateId,
            bookingAdjustRequest
        );

        return planConstraintException;
    }

    private static bool IsDateOutOfRange(
        long bookingDateId,
        long appDateId
    )
    {
        return bookingDateId > appDateId;
    }

    private static bool IsPriceValid(
        long appDateId,
        BookingCreateRequest bookingCreateRequest,
        BookingDataAvailableModel bookingDataAvailable
    )
    {
        var bookingAdjustRequest = bookingCreateRequest.Adjust;
        var numberOfAdultPersons = bookingAdjustRequest.GetNumberOfAdultsByDateId(appDateId);

        return bookingDataAvailable.AppDatePriceDataOfSiteInPlanRoom
            .Where(x => x.SiteId == bookingCreateRequest.SiteId)
            .Where(x => x.PlanId == bookingCreateRequest.PlanId)
            .Where(x => x.RoomGroupId == bookingCreateRequest.RoomGroupId)
            .Where(x => x.DateCalendar == appDateId)
            .Where(x => x.PriceData!.InRange(numberOfAdultPersons))
            .Where(x => x.PriceData!.Price != null)
            .Any(x => x.PriceData!.Price!.Value > 0);
    }

    private static ReservationServiceException? CheckRoomData(
        BookingRoomAppDateModel bookingRoomAppDateByDateId,
        BookingAdjustRequest bookingAdjustRequest
    )
    {
        if (bookingRoomAppDateByDateId.RemainNumber is null or <= 0)
        {
            return new ReservationNoRemainRoomNumberRestException();
        }

        if (bookingAdjustRequest.NumberOfRooms > bookingRoomAppDateByDateId.RemainNumber)
        {
            return new ReservationOverRemainRoomNumberRestException();
        }

        var maxNumberOfPeoples =
            bookingAdjustRequest.GetMaxNumberOfPeoplesByDateId(bookingRoomAppDateByDateId.AppDateId);
        if (!bookingRoomAppDateByDateId.RoomGroupInfo?.InRange(maxNumberOfPeoples) ?? true)
        {
            return new ReservationOverCapacityNumberRestException();
        }

        if (bookingRoomAppDateByDateId.IsNotSelled)
        {
            return new ReservationNotSelledRestException();
        }

        return null;
    }

    private static ReservationServiceException? CheckPlanConstraints(
        long appDateId,
        Plan existingPlan,
        BookingRoomAppDateModel bookingRoomAppDateByDateId,
        BookingAdjustRequest bookingAdjustRequest
    )
    {
        if (!existingPlan.CanDisplayDate(appDateId))
        {
            return new ReservationOverDisplayDateException();
        }

        if (!existingPlan.CanAcceptDate(appDateId))
        {
            return new ReservationOverAcceptDateException();
        }

        if (!existingPlan.CanDaySaleLimitReservedNumber(bookingRoomAppDateByDateId.ReservedNumber))
        {
            return new ReservationOverPlanDaySaleLimitRoomNumberDatSaleLimitException();
        }

        if (!existingPlan.CanDaySaleLimitReservedPairs(bookingRoomAppDateByDateId.ReservationPairs))
        {
            return new ReservationOverPlanGroupNumberDaySaleLimitException();
        }

        var numberOfPersons = bookingAdjustRequest.GetNumberOfPeoplesByDateId(appDateId) ?? 0;
        if (!existingPlan.CanDaySaleLimitAllPersons(numberOfPersons))
        {
            return new ReservationOverAcceptPersonNumberException();
        }

        return null;
    }
}
