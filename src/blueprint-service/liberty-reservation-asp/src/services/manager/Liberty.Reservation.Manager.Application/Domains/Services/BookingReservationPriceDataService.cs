using Liberty.Reservation.Application.Exceptions;
using Liberty.Reservation.Application.Models.Requests;
using Liberty.Reservation.Manager.Application.Models;

namespace Liberty.Reservation.Manager.Application.Domains.Services;

public class BookingReservationPriceDataService : IBookingReservationPriceDataService
{
    public IEnumerable<BookingReservationPriceData> GetAllReservationPriceData(
        BookingCreateRequest bookingCreateRequest,
        BookingDataAvailableModel bookingDataAvailable,
        IEnumerable<BookingRoomAppDateModel> bookingRoomAppDates
    )
    {
        var bookingAdjustRequest = bookingCreateRequest.Adjust;
        var bookingReservationPriceData = new List<BookingReservationPriceData>();
        var bookingRoomAppDateModels = bookingRoomAppDates as BookingRoomAppDateModel[] ?? [];

        foreach (var (appDateId, rooms) in bookingAdjustRequest.NightPeoples ?? [])
        {
            ValidateRestExceptions(appDateId, bookingRoomAppDateModels);

            var prevDay = (int)(appDateId - AppDate.GetId(bookingCreateRequest.BookingDate));
            var numberOfAdultPersons = bookingAdjustRequest.GetNumberOfAdultsByDateId(
                appDateId
            );

            var priceData = GetPriceData(
                appDateId,
                numberOfAdultPersons,
                bookingCreateRequest,
                bookingDataAvailable
            );
            ValidatePriceData(priceData, numberOfAdultPersons);

            var appDateOfSiteInPlanRoom = GetAppDateOfSiteInPlanRoom(
                appDateId,
                bookingCreateRequest,
                bookingDataAvailable
            );

            foreach (var (roomIndex, peoples) in rooms)
            {
                foreach (var (personAgeTypeId, numberOfPersons, genders) in peoples)
                {
                    var (malePersons, femalePersons) = GetGenderCounts(genders, numberOfPersons);
                    var planPersonAgeType = GetPlanPersonAgeType(
                        personAgeTypeId,
                        bookingCreateRequest,
                        bookingDataAvailable
                    );

                    var price = planPersonAgeType.GetPrice(priceData);
                    var discountedPrice = GetDiscountedPrice(
                        bookingAdjustRequest.GetNumberOfAdultsByDateId(appDateId) ?? 0,
                        prevDay,
                        price,
                        bookingCreateRequest,
                        appDateOfSiteInPlanRoom,
                        bookingDataAvailable.DiscountDataOfSiteInPlanRoom
                    );

                    ValidateDiscountedPrice(discountedPrice, planPersonAgeType);

                    var subTotalPrice = discountedPrice * numberOfPersons;
                    var spaTax = planPersonAgeType.PersonAgeType?.GetSpaTax(discountedPrice);
                    var subTotalSpaTax = spaTax * numberOfPersons;

                    ValidateSubTotalPrice(subTotalPrice);

                    var reservationPriceData = new BookingReservationPriceData
                    {
                        AppDateId = appDateId,
                        RoomGroupIndex = roomIndex,
                        Persons = numberOfPersons, // 人数
                        MalePersons = malePersons, // 男性人数
                        FemalePersons = femalePersons, // 女性人数
                        Price = discountedPrice, // 部屋単価
                        TotalPrice = subTotalPrice, // 部屋料金小計
                        SpaTax = spaTax, // 入湯税
                        TotalSpaTax = subTotalSpaTax, // 入湯税小計
                        PersonAgeType = new()
                        {
                            Id = planPersonAgeType.PersonAgeTypeId,
                            Name = planPersonAgeType.PersonAgeType?.Name,
                            AgeMin = planPersonAgeType.PersonAgeType?.AgeMin,
                            AgeMax = planPersonAgeType.PersonAgeType?.AgeMax,
                            IsMain = planPersonAgeType.PersonAgeType?.IsMain ?? false,
                            IsEnabled = planPersonAgeType.IsEnabled,
                            PriceSettingType = planPersonAgeType.PriceSettingType.ToString(),
                            Value = planPersonAgeType.Value
                        }
                    };

                    bookingReservationPriceData.Add(reservationPriceData);
                }
            }
        }

        return bookingReservationPriceData;
    }

    private static void ValidateRestExceptions(
        long appDateId,
        IEnumerable<BookingRoomAppDateModel> bookingRoomAppDates
    )
    {
        var bookingRoomAppDateModels = bookingRoomAppDates as BookingRoomAppDateModel[] ?? [];

        var hasExceptions = bookingRoomAppDateModels
            .Where(x => x.AppDateId == appDateId)
            .Where(x => !x.CanRest)
            .SelectMany(x => x.Exceptions)
            .Any(x => x.RestIndex == 0);

        if (hasExceptions)
        {
            throw new ReservationCanNotRestException(appDateId);
        }
    }

    private static PriceData? GetPriceData(
        long appDateId,
        int? numberOfAdultPersons,
        BookingCreateRequest bookingCreateRequest,
        BookingDataAvailableModel bookingDataAvailable
    )
    {
        return bookingDataAvailable
            .AppDatePriceDataOfSiteInPlanRoom
            .Where(x => x.IsEnabled)
            .Where(x => x.PlanId == bookingCreateRequest.PlanId)
            .Where(x => x.RoomGroupId == bookingCreateRequest.RoomGroupId)
            .Where(x => x.SiteId == bookingCreateRequest.SiteId)
            .Where(x => x.PriceDataId == appDateId)
            .Where(x => x.PriceData!.InRange(numberOfAdultPersons))
            .Where(x => x.PriceData?.Price != null)
            .Where(x => x.PriceData?.Price > 0)
            .Select(x => x.PriceData)
            .SingleOrDefault();
    }

    private static void ValidatePriceData(
        PriceData? priceData,
        int? numberOfAdultPersons
    )
    {
        if (priceData == null)
        {
            throw new ReservationNoPriceSettingException();
        }

        if (numberOfAdultPersons <= 0)
        {
            throw new ReservationNoAdultPersonsException();
        }
    }

    private static PlanRoomGroupSiteAppDate GetAppDateOfSiteInPlanRoom(
        long appDateId,
        BookingCreateRequest bookingCreateRequest,
        BookingDataAvailableModel bookingDataAvailable
    )
    {
        return bookingDataAvailable
                   .AppDatesOfSiteInPlanRoom
                   .Where(a => a.PlanId == bookingCreateRequest.PlanId)
                   .Where(a => a.RoomGroupId == bookingCreateRequest.RoomGroupId)
                   .Where(a => a.SiteId == bookingCreateRequest.SiteId)
                   .SingleOrDefault(a => a.DateCalendar == appDateId)
               ?? throw new ReservationPlanRoomGroupSiteAppDateNotFoundException();
    }

    private static (int malePersons, int femalePersons) GetGenderCounts(
        Genders genders,
        int numberOfPersons
    )
    {
        var malePersons = genders == Genders.Male ? numberOfPersons : 0;
        var femalePersons = genders == Genders.Female ? numberOfPersons : 0;
        return (malePersons, femalePersons);
    }

    private static PlanRoomGroupSitePersonAgeType GetPlanPersonAgeType(
        long personAgeTypeId,
        BookingCreateRequest bookingCreateRequest,
        BookingDataAvailableModel bookingDataAvailable
    )
    {
        return bookingDataAvailable
                   .PersonAgeTypesOfSiteInPlanRoom
                   .Where(x => x.IsEnabled)
                   .Where(x => x.PlanId == bookingCreateRequest.PlanId)
                   .Where(x => x.RoomGroupId == bookingCreateRequest.RoomGroupId)
                   .Where(x => x.SiteId == bookingCreateRequest.SiteId)
                   .SingleOrDefault(x => x.PersonAgeTypeId == personAgeTypeId)
               ?? throw new ReservationNoPersonAgeTypeException();
    }

    private static void ValidateDiscountedPrice(
        int? discountedPrice,
        PlanRoomGroupSitePersonAgeType planPersonAgeType
    )
    {
        if (discountedPrice is null or < 0)
        {
            throw new ReservationPriceMinusException();
        }

        if (planPersonAgeType.IsRegardAdult)
        {
            throw new ReservationAdultPriceZeroException();
        }
    }

    private static void ValidateSubTotalPrice(
        int? subTotalPrice
    )
    {
        if (subTotalPrice is null or < 0)
        {
            throw new ReservationPriceMinusException();
        }
    }

    private static int? GetDiscountedPrice(
        int numberOfAdultPersons,
        int prevDay,
        int? price,
        BookingCreateRequest bookingCreateRequest,
        PlanRoomGroupSiteAppDate planRoomGroupSiteAppDate,
        IEnumerable<PlanRoomGroupSiteDiscountData> discountData
    )
    {
        if (price is null || !planRoomGroupSiteAppDate.UseAutoDiscount)
        {
            return price;
        }

        var planRoomGroupSiteDiscountData = discountData
            .Where(x => x.PlanId == bookingCreateRequest.PlanId)
            .Where(x => x.RoomGroupId == bookingCreateRequest.RoomGroupId)
            .Where(x => x.SiteId == bookingCreateRequest.SiteId)
            .Where(x => x.DiscountData!.InRange(numberOfAdultPersons))
            .FirstOrDefault(x => x.DiscountData!.InPrevDay(prevDay));

        var discount = planRoomGroupSiteDiscountData?.DiscountData?.GetDiscount(price);
        return discount.HasValue ? price - discount.Value : price;
    }
}
