using Liberty.Reservation.Application.Constants;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Application.Domains.Services.Interfaces;
using Liberty.Reservation.Application.Exceptions;
using Liberty.Reservation.Application.Models;
using Liberty.Reservation.Application.Models.Requests;

namespace Liberty.Reservation.Application.Domains.Services;

public class BookingReservationPriceDataOfPlanService : IBookingReservationPriceDataService
{
    public IEnumerable<BookingReservationPriceData> GetAllReservationPriceData(
        BookingCreateRequest bookingCreateRequest,
        BookingPlanModel bookingPlanAvailable,
        IEnumerable<BookingRoomAppDateModel> bookingRoomAppDates
    )
    {
        var bookingAdjustRequest = bookingCreateRequest.Adjust;
        var bookingReservationPriceData = new List<BookingReservationPriceData>();
        var bookingRoomAppDateModels = bookingRoomAppDates as BookingRoomAppDateModel[] ?? [];

        var bookingRoomAvailable = bookingPlanAvailable.RoomGroups.FirstOrDefault(
                x => x.Id == bookingCreateRequest.RoomGroupId
            )
            ?? throw new ReservationInvalidException("Room not found in the booking data available.");

        var personAgeTypeIds = bookingRoomAvailable.PersonTypes
            .Where(x => x.IsRegardAdult)
            .Select(x => x.PersonAgeTypeId)
            .ToList();

        foreach (var (appDateId, rooms) in bookingAdjustRequest.NightPeoples ?? [])
        {
            ValidateRestExceptions(
                appDateId,
                bookingPlanAvailable,
                bookingRoomAppDateModels,
                bookingCreateRequest.IsNotCheckValidDateLimit
            );

            var prevDay = (AppDate.GetDateTime(appDateId) - bookingCreateRequest.BookingDate).Days;

            var appDateOfSiteInPlanRoom = GetAppDateOfSiteInPlanRoom(
                appDateId,
                bookingCreateRequest,
                bookingRoomAvailable
            );

            foreach (var (roomIndex, peoples) in rooms)
            {
                var peoplesGroupByAgeType = peoples.GroupBy(
                    x => x.PersonAgeTypeId
                );

                var numberOfAdultPersons = bookingAdjustRequest.GetNumberOfAdultsByDateIdAndRoomIndex(
                    appDateId,
                    roomIndex,
                    personAgeTypeIds
                );

                var priceData = GetPriceData(
                    appDateId,
                    numberOfAdultPersons,
                    bookingCreateRequest,
                    bookingRoomAvailable
                );
                ValidatePriceData(priceData, numberOfAdultPersons);

                foreach (var peoplesByType in peoplesGroupByAgeType)
                {
                    var personAgeTypeId = peoplesByType.Key;
                    var numberOfPersons = peoplesByType.Sum(x => x.NumberOfPeoples);
                    var (malePersons, femalePersons) = GetGenderCounts([.. peoplesByType]);
                    var planPersonAgeType = GetPlanPersonAgeType(
                        personAgeTypeId,
                        bookingCreateRequest,
                        bookingRoomAvailable
                    );
                    var isPersonTypeMain = planPersonAgeType.PersonAgeTypeIsMain ?? false;

                    var price = planPersonAgeType.GetPrice(priceData) ?? 0;
                    var discountedPrice = isPersonTypeMain
                        ? GetDiscountedPrice(
                            numberOfAdultPersons ?? 0,
                            prevDay,
                            price,
                            bookingCreateRequest,
                            appDateOfSiteInPlanRoom,
                            bookingRoomAvailable.DiscountData
                        )
                        : price;

                    if (isPersonTypeMain)
                    {
                        ValidateDiscountedPrice(
                            discountedPrice,
                            planPersonAgeType
                        );
                    }

                    var subTotalPrice = discountedPrice * numberOfPersons;
                    var spaTax = planPersonAgeType.GetSpaTax(discountedPrice);
                    var subTotalSpaTax = spaTax * numberOfPersons;

                    var reservationPriceData = new BookingReservationPriceData
                    {
                        AppDateId = appDateId,
                        RoomGroupIndex = roomIndex,
                        Persons = numberOfPersons, // 人数
                        MalePersons = malePersons, // 男性人数
                        FemalePersons = femalePersons, // 女性人数
                        NonePersons = numberOfPersons - malePersons - femalePersons,
                        Price = discountedPrice, // 部屋単価
                        TotalPrice = subTotalPrice, // 部屋料金小計
                        SpaTax = spaTax, // 入湯税
                        TotalSpaTax = subTotalSpaTax, // 入湯税小計
                        PersonAgeType = new()
                        {
                            Id = planPersonAgeType.PersonAgeTypeId,
                            Name = planPersonAgeType.PersonAgeTypeName?.GetValueByHeader(DefaultValues.LanguageCode),
                            AgeMin = planPersonAgeType.PersonAgeTypeAgeMin,
                            AgeMax = planPersonAgeType.PersonAgeTypeAgeMax,
                            IsMain = planPersonAgeType.PersonAgeTypeIsMain ?? false,
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

    protected static void ValidateRestExceptions(
        long appDateId,
        BookingPlanModel bookingPlanAvailable,
        IEnumerable<BookingRoomAppDateModel> bookingRoomAppDates,
        bool isNotCheckValidDateLimit = false
    )
    {
        var bookingRoomAppDateModels = bookingRoomAppDates as BookingRoomAppDateModel[] ?? [];

        var bookingSearchDate = DateTime.UtcNow.AddHours(
            DefaultValues.TimeZoneOffset
        );
        var bookingSearchDateId = AppDate.GetId(bookingSearchDate);

        var isAvailable = bookingPlanAvailable.CanDisplayDate(appDateId)
            && bookingPlanAvailable.CanDisplayDate(bookingSearchDateId)
            && bookingPlanAvailable.CanAcceptDate(appDateId)
            && bookingPlanAvailable.CanAcceptDate(bookingSearchDateId)
            && bookingPlanAvailable.CanBookingReceptionDate(appDateId)
            && bookingPlanAvailable.IsReceptionAvailable(appDateId, isNotCheckValidDateLimit);
        if (!isAvailable)
        {
            throw new ReservationCanNotRestException(appDateId);
        }

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

    protected static BookingMetaPriceDataModel? GetPriceData(
        long appDateId,
        int? numberOfAdultPersons,
        BookingCreateRequest bookingCreateRequest,
        RoomGroupOfBookingPlanModel bookingDataAvailable
    )
    {
        return bookingDataAvailable
            .PriceData
            .Where(x => x.PlanId == bookingCreateRequest.PlanId)
            .Where(x => x.RoomGroupId == bookingCreateRequest.RoomGroupId)
            .Where(x => x.SiteId == bookingCreateRequest.SiteId)
            .Where(x => x.AppDateId == appDateId)
            .Where(x => x.InRange(numberOfAdultPersons))
            .Where(x => x.Price != null)
            .Where(x => x.Price > 0)
            .OrderByDescending(x => x.PriceDataId)
            .FirstOrDefault();
    }

    protected static void ValidatePriceData(
        BookingMetaPriceDataModel? priceData,
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

    protected static BookingMetaPlanAppDateModel GetAppDateOfSiteInPlanRoom(
        long appDateId,
        BookingCreateRequest bookingCreateRequest,
        RoomGroupOfBookingPlanModel bookingDataAvailable
    )
    {
        return bookingDataAvailable
                .PlanAppDates
                .Where(x => x.PlanId == bookingCreateRequest.PlanId)
                .Where(x => x.RoomGroupId == bookingCreateRequest.RoomGroupId)
                .Where(x => x.SiteId == bookingCreateRequest.SiteId)
                .SingleOrDefault(x => x.AppDateId == appDateId)
            ?? throw new ReservationPlanRoomGroupSiteAppDateNotFoundException();
    }

    protected static (int malePersons, int femalePersons) GetGenderCounts(
        IEnumerable<PeopleOfReservationAdjustRequest> peoples
    )
    {
        var peoplesList = peoples.ToList();

        var malePersons = peoplesList
            .Where(x => x.Gender == Genders.Male)
            .Sum(x => x.NumberOfPeoples);

        var femalePersons = peoplesList
            .Where(x => x.Gender == Genders.Female)
            .Sum(x => x.NumberOfPeoples);

        return (malePersons, femalePersons);
    }

    protected static BookingMetaPersonTypeModel GetPlanPersonAgeType(
        long personAgeTypeId,
        BookingCreateRequest bookingCreateRequest,
        RoomGroupOfBookingPlanModel bookingDataAvailable
    )
    {
        return bookingDataAvailable
                .PersonTypes
                .Where(x => x.IsEnabled)
                .Where(x => x.PlanId == bookingCreateRequest.PlanId)
                .Where(x => x.RoomGroupId == bookingCreateRequest.RoomGroupId)
                .Where(x => x.SiteId == bookingCreateRequest.SiteId)
                .SingleOrDefault(x => x.PersonAgeTypeId == personAgeTypeId)
            ?? throw new ReservationNoPersonAgeTypeException();
    }

    protected static void ValidateDiscountedPrice(
        int? discountedPrice,
        BookingMetaPersonTypeModel planPersonAgeType
    )
    {
        if (planPersonAgeType.IsRegardAdult && discountedPrice is null or <= 0)
        {
            throw new ReservationPriceMinusException();
        }

        // Uncomment this code when PlanRoomGroupSitePersonAgeType is implemented
        // if (planPersonAgeType.IsRegardAdult)
        // {
        //     throw new ReservationAdultPriceZeroException();
        // }
    }

    protected static int? GetDiscountedPrice(
        int numberOfAdultPersons,
        int prevDay,
        int? price,
        BookingCreateRequest bookingCreateRequest,
        BookingMetaPlanAppDateModel planRoomGroupSiteAppDate,
        IEnumerable<BookingMetaDiscountDataModel> discountData
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
            .Where(x => x.InRange(numberOfAdultPersons))
            .OrderByDescending(x => x.DiscountDataId)
            .FirstOrDefault(x => x.InPrevDay(prevDay));

        var discount = planRoomGroupSiteDiscountData?.GetDiscount(price);
        return discount.HasValue ? price - discount.Value : price;
    }
}
