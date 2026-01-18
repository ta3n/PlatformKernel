using Liberty.Reservation.Application.Constants;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Metas;
using Liberty.Reservation.Application.Domains.Repositories.Interfaces;
using Liberty.Reservation.Application.Domains.Services.Interfaces;
using Liberty.Reservation.Application.Exceptions;
using Liberty.Reservation.Application.Models;
using Microsoft.EntityFrameworkCore;

namespace Liberty.Reservation.Application.Domains.Services;

public class BookingDataService(
    IBookingFacilityRepository facilityRepository,
    IBookingSiteRepository siteRepository,
    IBookingDataPlanRepository dataPlanRepository
) : IBookingDataService
{
    public async Task<BookingData> GetBookingDataAsync(
        Contexts.DataContexts.Entities.Data.Reservation reservation,
        BookingPlanModel availableBookingPlan,
        List<BookingReservationPriceData>? reservationPriceData,
        List<Question>? selectedQuestions,
        BookingCancellationPolicyModel? bookingCancellationPolicyModel,
        CancellationToken cancellationToken = default
    )
    {
        var availableBookingRoom = availableBookingPlan.RoomGroups.FirstOrDefault(
            x => x.Id == reservation.RoomGroupId
        )!;

        var facilityData = await GetFacilityAsync(reservation.FacilityId, cancellationToken);

        var siteData = await GetSiteAsync(reservation.SiteId, cancellationToken);

        var planData = await GetPlanDataAsync(availableBookingPlan, bookingCancellationPolicyModel, cancellationToken);

        var roomGroupData = GetRoomGroup(availableBookingRoom);

        var personAgeTypeDataList = GetAllPersonAgeTypes(availableBookingRoom);

        var appDates = GetAppDates(reservation, reservationPriceData);

        var planQuestions = reservation.ReservationQuestions?
            .Where(x => x.ReservationQuestionType == ReservationQuestionTypes.Plan)
            .Select(
                x => new QuestionData
                {
                    Id = x.QuestionId,
                    Name = selectedQuestions?.Where(y => y.Id == x.QuestionId)
                        .Select(y => y.Name?.GetValueByHeader(DefaultValues.LanguageCode))
                        .FirstOrDefault(),
                    Description =
                        selectedQuestions?.Where(y => y.Id == x.QuestionId)
                            .Select(y => y.Description?.GetValueByHeader(DefaultValues.LanguageCode))
                            .FirstOrDefault(),
                    FormData = selectedQuestions?.Where(y => y.Id == x.QuestionId)
                        .Select(y => y.FormData?.GetValueByHeader(DefaultValues.LanguageCode))
                        .FirstOrDefault(),
                    AnswerData = x.AnswerData,
                    IsRequired = reservation.ReservationQuestions?
                        .FirstOrDefault(y => y.QuestionId == x.QuestionId)
                        ?.IsRequired
                }
            )
            .ToList();

        var optionQuestions = reservation.ReservationQuestions?
            .Where(x => x.ReservationQuestionType == ReservationQuestionTypes.OptionItem)
            .Select(
                x => new QuestionData
                {
                    Id = x.QuestionId,
                    Name = selectedQuestions?.Where(y => y.Id == x.QuestionId)
                        .Select(y => y.Name?.GetValueByHeader(DefaultValues.LanguageCode))
                        .FirstOrDefault(),
                    Description =
                        selectedQuestions?.Where(y => y.Id == x.QuestionId)
                            .Select(y => y.Description?.GetValueByHeader(DefaultValues.LanguageCode))
                            .FirstOrDefault(),
                    FormData = selectedQuestions?.Where(y => y.Id == x.QuestionId)
                        .Select(y => y.FormData?.GetValueByHeader(DefaultValues.LanguageCode))
                        .FirstOrDefault(),
                    AnswerData = x.AnswerData,
                    IsRequired = reservation.ReservationQuestions?
                        .FirstOrDefault(y => y.QuestionId == x.QuestionId)
                        ?.IsRequired
                }
            )
            .ToList();

        return new BookingData
        {
            Facility = facilityData,
            Plan = planData,
            RoomGroup = roomGroupData,
            Site = siteData,
            PlanQuestions = planQuestions,
            OptionQuestions = optionQuestions,
            TotalRoomPrice = appDates?.Sum(x => x.Price) ?? 0,
            TotalSpaTax = appDates?.Sum(x => x.SpaTax) ?? 0,
            TotalOptionPrice = appDates?.Sum(x => x.OptionPrice) ?? 0,
            AppDates = appDates,
            PersonAgeTypes = personAgeTypeDataList,
            UseSpaTax = availableBookingPlan.FacilityUseSpaTax
        };
    }

    private async Task<FacilityData> GetFacilityAsync(
        long id,
        CancellationToken cancellationToken
    )
    {
        return await facilityRepository
                .GetQueryableWithAsNoTracking()
                .Where(x => x.Id == id)
                .Where(x => x.IsEnabled)
                .Select(
                    x => new FacilityData
                    {
                        Id = x.Id,
                        Name = x.Name!.GetValueByHeader(DefaultValues.LanguageCode),
                        LocalizedNames = x.Name,
                        Address1 = x.Address1 != null ? x.Address1!.GetValueByHeader(DefaultValues.LanguageCode) : string.Empty,
                        Address2 = x.Address2 != null ? x.Address2!.GetValueByHeader(DefaultValues.LanguageCode) : string.Empty,
                        Address3 = x.Address3 != null ? x.Address3!.GetValueByHeader(DefaultValues.LanguageCode) : string.Empty,
                        Address4 = x.Address4 != null ? x.Address4!.GetValueByHeader(DefaultValues.LanguageCode) : string.Empty,
                        Tel = x.Phone,
                        CanOnLinePayment = x.CanOnLinePayment,
                        IsOnSidePayment = x.IsOnSidePayment,
                        CanAddRoomOnModify = x.CanAddRoomOnModify,
                        IsExtendedStayOnModify = x.IsExtendedStayOnModify,
                        IsOnLinePayment = x.IsOnLinePayment,
                        AccessInfoComment = x.AccessInfoComment!.GetValueByHeader(DefaultValues.LanguageCode),
                        TimeZone = x.TimeZone.ToString(),
                        TimeZoneId = x.TimeZoneId
                    }
                )
                .FirstOrDefaultAsync(cancellationToken)
            ?? throw new ReservationInvalidException("Facility not found");
    }

    private async Task<SiteData> GetSiteAsync(
        long id,
        CancellationToken cancellationToken
    )
    {
        return await siteRepository
                .GetQueryableWithAsNoTracking()
                .Where(x => x.Id == id)
                .Where(x => x.IsEnabled)
                .Select(
                    x => new SiteData
                    {
                        Id = id,
                        Name = x.Name != null ? x.Name.GetValueByHeader(DefaultValues.LanguageCode) : null,
                        LocalizedNames = x.Name,
                        ShortName = x.ShortName,
                        PrefixName = x.PrefixName
                    }
                )
                .FirstOrDefaultAsync(cancellationToken)
            ?? throw new ReservationInvalidException("Site not found");
    }

    private static List<BookingAppDateData>? GetAppDates(
        Contexts.DataContexts.Entities.Data.Reservation reservation,
        List<BookingReservationPriceData>? reservationPriceData
    )
    {
        var data = reservation.ReservationPlanRoomGroupAppDates?
            .GroupBy(
                appDate => new
                {
                    appDate.BookingDateId,
                    appDate.RestIndex
                }
            )
            .Select(
                groupAppDates => new BookingAppDateData
                {
                    AppDateId = groupAppDates.Key.BookingDateId,
                    RestIndex = groupAppDates.Key.RestIndex,
                    Price = reservationPriceData!
                            .Where(
                                priceData => priceData.AppDateId == groupAppDates.Key.BookingDateId
                            )
                            .Sum(t => t.TotalPrice)
                        ?? 0,
                    SpaTax = reservationPriceData!
                            .Where(
                                priceData => priceData.AppDateId == groupAppDates.Key.BookingDateId
                            )
                            .Sum(t => t.TotalSpaTax)
                        ?? 0,
                    OptionPrice = groupAppDates.Sum(
                        x => x.ReservationRoomGroupAppDateOptionItems?.Select(
                                    y => y.TotalPrice
                                )
                                .Sum()
                            ?? 0
                    ),
                    Rooms =
                    [
                        .. groupAppDates
                            .Select(
                                planRoomGroupAppDate => new BookingRoomDataOfAppDate
                                {
                                    RoomIndex = planRoomGroupAppDate.RoomGroupIndex,
                                    RoomPrice = reservationPriceData!
                                            .Where(
                                                priceData => priceData.AppDateId == groupAppDates.Key.BookingDateId
                                                    && priceData.RoomGroupIndex == planRoomGroupAppDate.RoomGroupIndex
                                            )
                                            .Sum(t => t.TotalPrice)
                                        ?? 0,
                                    SpaTax = reservationPriceData!
                                            .Where(
                                                priceData => priceData.AppDateId == groupAppDates.Key.BookingDateId
                                                    && priceData.RoomGroupIndex == planRoomGroupAppDate.RoomGroupIndex
                                            )
                                            .Sum(t => t.TotalSpaTax)
                                        ?? 0,
                                    PricePeoples = planRoomGroupAppDate.ReservationRoomGroupAppDatePersonAgeTypes!.Select(
                                        personAgeTypeOfRoom =>
                                        {
                                            var priceData = reservationPriceData!
                                                .SingleOrDefault(
                                                    priceData => priceData.AppDateId == groupAppDates.Key.BookingDateId
                                                        && priceData.RoomGroupIndex == planRoomGroupAppDate.RoomGroupIndex
                                                        && priceData.PersonAgeType?.Id == personAgeTypeOfRoom.PersonAgeTypeId
                                                )!;

                                            return new PeoplePriceDataOfRoom
                                            {
                                                RoomPrice = priceData.Price ?? 0,
                                                TotalPrice = priceData.TotalPrice ?? 0,
                                                SpaTax = priceData.SpaTax ?? 0,
                                                TotalSpaTax = priceData.TotalSpaTax ?? 0,
                                                Persons = priceData.Persons,
                                                MalePersons = priceData.MalePersons,
                                                FemalePersons = priceData.FemalePersons,
                                                NonePersons = priceData.NonePersons,
                                                OtherPersons = priceData.OtherPersons,
                                                PersonAgeType = new PeopleDataOfRoom
                                                {
                                                    Id = priceData.PersonAgeType?.Id,
                                                    Name = priceData.PersonAgeType?.Name,
                                                    IsMain = priceData.PersonAgeType?.IsMain,
                                                    AgeMax = priceData.PersonAgeType?.AgeMax,
                                                    AgeMin = priceData.PersonAgeType?.AgeMin
                                                }
                                            };
                                        }
                                    ),
                                    CustomerInfo = planRoomGroupAppDate.CustomerInfo is null
                                        ? null
                                        : new CustomerData
                                        {
                                            Name = planRoomGroupAppDate.CustomerInfo?.Name,
                                            Kana = planRoomGroupAppDate.CustomerInfo?.Kana
                                        },
                                    OptionItems = planRoomGroupAppDate.ReservationRoomGroupAppDateOptionItems?
                                        .Select(
                                            b => new OptionItemDataOfRoom
                                            {
                                                Id = b.OptionItemId,
                                                Name = b.OptionItem?.Name?.GetValueByHeader(DefaultValues.LanguageCode),
                                                Number = b.Number,
                                                Price = b.Price
                                            }
                                        )
                                        .ToList(),
                                    TotalOptionPrice =
                                        planRoomGroupAppDate.ReservationRoomGroupAppDateOptionItems?.Sum(y => y.TotalPrice) ?? 0,
                                    Persons = planRoomGroupAppDate.ReservationRoomGroupAppDatePersonAgeTypes?.Sum(y => y.Number) ?? 0,
                                    MalePersons =
                                        planRoomGroupAppDate.ReservationRoomGroupAppDatePersonAgeTypes?.Sum(y => y.MaleNumber) ?? 0,
                                    FemalePersons = planRoomGroupAppDate.ReservationRoomGroupAppDatePersonAgeTypes?.Sum(y => y.FemaleNumber)
                                        ?? 0
                                }
                            )
                    ]
                }
            )
            .ToList();

        return data;
    }

    private async Task<PlanData> GetPlanDataAsync(
        BookingPlanModel availableBookingPlan,
        BookingCancellationPolicyModel? bookingCancellationPolicyModel,
        CancellationToken cancellationToken = default
    )
    {
        var queryable = dataPlanRepository
            .GetQueryableWithAsNoTracking()
            .Where(x => x.Id == availableBookingPlan.Id)
            .Where(x => x.IsEnabled)
            .Select(x => x.Meta);

        var planMeta = await queryable.FirstOrDefaultAsync(cancellationToken)
            ?? new PlanMeta();

        var planData = new PlanData
        {
            Id = availableBookingPlan.Id,
            Name = availableBookingPlan.Name!.GetValueByHeader(DefaultValues.LanguageCode),
            Meta = planMeta,
            IsOnSidePayment = availableBookingPlan.IsOnSidePayment,
            IsOnLinePayment = availableBookingPlan.IsOnLinePayment,
            DayUse = availableBookingPlan.DayUse,
            Meals =
            [
                .. availableBookingPlan.MealTypes
                    .Select(
                        y => new PlanMealData
                        {
                            Id = y.Id,
                            Name = y.Name!,
                            MealTypeEatType = y.MealTypeEatType
                        }
                    )
            ],
            Files =
            [
                .. availableBookingPlan.Media
                    .Select(
                        y => new FileData
                        {
                            Code = y.Code,
                            ContentType = y.ContentType
                        }
                    )
            ],
            CancelDayLimit = availableBookingPlan.CancelDayLimit,
            CancelLimit = availableBookingPlan.CancelLimit,
            IsCancelSameAccept = availableBookingPlan.IsCancelSameAccept,
            CancellationDataPolicy = bookingCancellationPolicyModel ?? availableBookingPlan.CancellationDataPolicy,
            ReceptionDayLimit = availableBookingPlan.ReceptionDayLimit
        };

        return planData;
    }

    private static RoomGroupData GetRoomGroup(
        RoomGroupOfBookingPlanModel availableBookingRoom
    )
    {
        var roomData = new RoomGroupData
        {
            Id = availableBookingRoom.Id,
            Name = availableBookingRoom.Name!.GetValueByHeader(DefaultValues.LanguageCode),
            GroupName = availableBookingRoom.GroupName,
            IsEnabledSmoking = availableBookingRoom.IsEnabledSmoking,
            CapacityMax = availableBookingRoom.CapacityMax,
            CapacityMin = availableBookingRoom.CapacityMin,
            Files =
            [
                .. availableBookingRoom.Media
                    .Select(
                        y => new FileData
                        {
                            Code = y.Code,
                            ContentType = y.ContentType
                        }
                    )
            ]
        };

        return roomData;
    }

    private static List<PersonAgeTypeData> GetAllPersonAgeTypes(
        RoomGroupOfBookingPlanModel availableBookingRoom
    )
    {
        var personAgeTypeData = availableBookingRoom.PersonTypes
            .OrderBy(x => x.DisplayOrder)
            .Select(
                bookingMetaPersonTypeModel => new PersonAgeTypeData
                {
                    Id = bookingMetaPersonTypeModel.PersonAgeTypeId,
                    Name = bookingMetaPersonTypeModel.PersonAgeTypeName!.GetValueByHeader(DefaultValues.LanguageCode),
                    IsMain = bookingMetaPersonTypeModel.PersonAgeTypeIsMain ?? false,
                    AgeMax = bookingMetaPersonTypeModel.PersonAgeTypeAgeMax,
                    AgeMin = bookingMetaPersonTypeModel.PersonAgeTypeAgeMin,
                    PersonAgeTypeSpaTaxDatas = bookingMetaPersonTypeModel.SpaTaxDataList.Select(
                        spaTaxDataOfBookingMetaPersonTypeModel =>
                            new SpaTaxOfPersonAgeTypeData
                            {
                                PriceMin = spaTaxDataOfBookingMetaPersonTypeModel.PriceMin,
                                PriceMax = spaTaxDataOfBookingMetaPersonTypeModel.PriceMax,
                                Tax = spaTaxDataOfBookingMetaPersonTypeModel.Tax
                            }
                    )
                }
            )
            .ToList();

        return personAgeTypeData;
    }
}
