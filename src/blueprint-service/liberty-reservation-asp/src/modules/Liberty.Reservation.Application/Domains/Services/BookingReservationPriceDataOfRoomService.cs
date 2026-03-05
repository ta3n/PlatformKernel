using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Application.Domains.Services.Interfaces;
using Liberty.Reservation.Application.Exceptions;
using Liberty.Reservation.Application.Models;
using Liberty.Reservation.Application.Models.Requests;

namespace Liberty.Reservation.Application.Domains.Services;

public class BookingReservationPriceDataOfRoomService : BookingReservationPriceDataOfPlanService, IBookingReservationPriceDataService
{
    public new IEnumerable<BookingReservationPriceData> GetAllReservationPriceData(
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

        var mainPersonTypeId = bookingRoomAvailable.PersonTypes
                .FirstOrDefault(x => x.PersonAgeTypeIsMain ?? false)
                ?.PersonAgeTypeId
            ?? 0;

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
                var listOfPeoples = peoples.ToList();

                var peoplesGroupByAgeType = listOfPeoples.GroupBy(
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
                    const int defaultNumberOfPersons = 1;
                    var numberOfPersons = peoplesByType.Sum(x => x.NumberOfPeoples);

                    var personAgeTypeId = peoplesByType.Key;
                    var (malePersons, femalePersons) = GetGenderCounts([.. peoplesByType]);

                    var planPersonAgeType = GetPlanPersonAgeType(
                        personAgeTypeId,
                        bookingCreateRequest,
                        bookingRoomAvailable
                    );

                    var isPersonTypeMain = planPersonAgeType.PersonAgeTypeIsMain ?? false;

                    var otherPersons = 0;

                    var price = planPersonAgeType.GetPrice(priceData);

                    var discountedPrice = GetDiscountedPrice(
                        numberOfAdultPersons ?? 0,
                        prevDay,
                        price,
                        bookingCreateRequest,
                        appDateOfSiteInPlanRoom,
                        bookingRoomAvailable.DiscountData
                    );

                    ValidateDiscountedPrice(
                        discountedPrice,
                        planPersonAgeType
                    );

                    var selectNumberOfPersons = !isPersonTypeMain
                        ? numberOfPersons
                        : defaultNumberOfPersons;

                    var subTotalPrice = discountedPrice * selectNumberOfPersons;
                    var spaTax = planPersonAgeType.GetSpaTax(discountedPrice);
                    var subTotalSpaTax = spaTax * numberOfPersons;

                    var nonePersons = numberOfPersons - malePersons - femalePersons;

                    if (isPersonTypeMain && planPersonAgeType.IsRegardAdult)
                    {
                        var childPersonsTypeAsRegardAdult = bookingRoomAvailable.PersonTypes
                            .Where(x => personAgeTypeIds.Contains(x.PersonAgeTypeId))
                            .Where(x => x.IsRegardAdult)
                            .Select(x => x.PersonAgeTypeId)
                            .ToList();
                        childPersonsTypeAsRegardAdult.Remove(mainPersonTypeId);

                        otherPersons = listOfPeoples
                            .Where(x => childPersonsTypeAsRegardAdult.Contains(x.PersonAgeTypeId))
                            .Sum(x => x.NumberOfPeoples);
                    }

                    var reservationPriceData = new BookingReservationPriceData
                    {
                        AppDateId = appDateId,
                        RoomGroupIndex = roomIndex,
                        Persons = numberOfPersons, // 人数
                        MalePersons = malePersons, // 男性人数
                        FemalePersons = femalePersons, // 女性人数
                        NonePersons = nonePersons,
                        Price = discountedPrice, // 部屋単価
                        TotalPrice = subTotalPrice, // 部屋料金小計
                        SpaTax = spaTax, // 入湯税
                        TotalSpaTax = subTotalSpaTax, // 入湯税小計
                        OtherPersons = otherPersons,
                        PersonAgeType = new()
                        {
                            Id = planPersonAgeType.PersonAgeTypeId,
                            Name = planPersonAgeType.PersonAgeTypeName?.GetValueByHeader(),
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
}
