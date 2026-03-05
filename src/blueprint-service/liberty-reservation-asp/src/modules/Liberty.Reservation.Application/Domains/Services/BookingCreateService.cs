using AutoMapper;
using Liberty.ApplicationShared.Utils;
using Liberty.Entity.Utils;
using Liberty.Entity.ValueObjects;
using Liberty.Reservation.Application.Constants;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;
using Liberty.Reservation.Application.Domains.Services.Interfaces;
using Liberty.Reservation.Application.Exceptions;
using Liberty.Reservation.Application.Models;
using Liberty.Reservation.Application.Models.Requests;
using Liberty.Reservation.Application.Models.Responses;
using Microsoft.Extensions.DependencyInjection;
using ReservationEntity = Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data.Reservation;

namespace Liberty.Reservation.Application.Domains.Services;

public class BookingCreateService(
    IMapper mapper,
    IServiceProvider serviceProvider,
    [FromKeyedServices("plan")] IBookingReservationPriceDataService bookingReservationPriceDataOfPlanService,
    [FromKeyedServices("room")] IBookingReservationPriceDataService bookingReservationPriceDataOfRoomService
) : IBookingCreateService
{
    private readonly IBookingSearchService _bookingSearchService =
        serviceProvider.GetRequiredService<IBookingSearchService>();

    private readonly IBookingDataService _bookingDataService =
        serviceProvider.GetRequiredService<IBookingDataService>();

    private readonly IBookingRoomAppDateService _bookingRoomAppDateService =
        serviceProvider.GetRequiredService<IBookingRoomAppDateService>();

    private readonly IBookingOptionInventoryService _bookingOptionInventoryService =
        serviceProvider.GetRequiredService<IBookingOptionInventoryService>();

    private readonly IBookingCancellationService _bookingCancellationService =
        serviceProvider.GetRequiredService<IBookingCancellationService>();

    public async Task<ReservationEntity> CreateBookingAsync(
        BookingCreateRequest bookingCreateRequest,
        BookingExternalInfoRequest bookingExternalInfoRequest,
        ReservationEntity? existingReservation,
        CancellationToken cancellationToken = default
    )
    {
        var newReservation = new ReservationEntity
        {
            Code = bookingCreateRequest.TempCode,
            Serial = EntityUtil.CreateCode(),
            FacilityId = bookingCreateRequest.FacilityId,
            FacilityRecordCode = bookingCreateRequest.FacilityRecordCode,
            SiteId = bookingCreateRequest.SiteId,
            PlanId = bookingCreateRequest.PlanId,
            RoomGroupId = bookingCreateRequest.RoomGroupId,
            RestNumber = bookingCreateRequest.Adjust.NumberOfNights,
            RoomNumber = bookingCreateRequest.Adjust.NumberOfRooms,
            CheckInDate = bookingCreateRequest.CheckInDate,
            CheckInTime = ConvertUtil.ToNullableTimeSpan(bookingCreateRequest.CheckInTime ?? string.Empty),
            CheckOutTime = bookingCreateRequest.CheckOutTime,
            Memo = bookingCreateRequest.Adjust.FreeInput,
            ReservationPlanRoomGroupAppDates = [],
            ReservationRoomGroupAppDatePersonAgeTypes = [],
            Reserver = mapper.Map<CustomerInfo>(bookingCreateRequest.Adjust.Reserver),
            IsSameMainUser = bookingCreateRequest.Adjust.MainUser == null,
            UseRoomUser = bookingCreateRequest.Adjust.RoomRepresentatives != null,
            ReservationDateTime = bookingCreateRequest.BookingDate,
            PaymentType = bookingCreateRequest.PaymentType,
            ModifiedDateTime = bookingCreateRequest.ModifiedDateTime,
            UpdateCount = bookingCreateRequest.UpdateCount
        };

        if (bookingExternalInfoRequest.UserCode is not null)
        {
            newReservation.UserCode = bookingExternalInfoRequest.UserCode;
            newReservation.ReservationState = DetermineReservationState(
                newReservation.IsOnlinePayment,
                existingReservation
            );

            if (newReservation.IsOnSidePayment)
            {
                newReservation.ConfirmedDateTime = bookingCreateRequest.BookingDate;
            }
        }
        else
        {
            newReservation.ReservationState = existingReservation is null ? ReservationStatus.Temporary : ReservationStatus.Modified;
        }

        if (!newReservation.IsSameMainUser)
        {
            newReservation.MainUser ??= mapper.Map<CustomerInfo>(bookingCreateRequest.Adjust.MainUser);
        }

        var rootCode = bookingCreateRequest.TempCode;
        var existingReservationId = existingReservation?.Id;
        if (existingReservation is not null)
        {
            newReservation.ParentId = existingReservation.Id;
            newReservation.UserCode = existingReservation.UserCode;
            rootCode = existingReservation.BookingData?.RootCode ?? rootCode;
        }

        var selectedQuestions = bookingExternalInfoRequest.SelectedQuestions?.ToList();
        if (selectedQuestions is not { Count: > 0 } && existingReservation is not null)
        {
            selectedQuestions = existingReservation.ReservationQuestions?
                .Where(x => x is { IsEnabled: true, Question: not null })
                .Select(x => x.Question!)
                .ToList();
        }

        BookingCancellationPolicyModel? bookingCancellationPolicyModel = null;
        if (bookingCreateRequest.SelectedCancellation is not null)
        {
            bookingCancellationPolicyModel = await _bookingCancellationService.GetCancellationPolicyAsync(
                bookingCreateRequest.SelectedCancellation ?? 0,
                cancellationToken
            );
        }

        newReservation = GetQuestions(
            bookingCreateRequest,
            newReservation
        );

        newReservation = await GetReservationDataAsync(
            bookingCreateRequest,
            newReservation,
            selectedQuestions,
            bookingCancellationPolicyModel,
            rootCode,
            existingReservationId,
            cancellationToken
        );

        return newReservation;
    }

    private static ReservationEntity GetQuestions(
        BookingCreateRequest bookingCreateRequest,
        ReservationEntity newReservation
    )
    {
        newReservation.ReservationQuestions = [];

        if (bookingCreateRequest.PlanQuestions is not null)
        {
            foreach (var bookingQuestion in bookingCreateRequest.PlanQuestions)
            {
                var planQuestion = new ReservationQuestion
                {
                    Reservation = newReservation,
                    QuestionId = bookingQuestion.QuestionId,
                    AnswerData = bookingQuestion.AnswerData,
                    ReservationQuestionType = ReservationQuestionTypes.Plan,
                    IsEnabled = true
                };

                newReservation.ReservationQuestions.Add(planQuestion);
            }
        }

        if (bookingCreateRequest.OptionsQuestions is null)
        {
            return newReservation;
        }

        foreach (var bookingQuestion in bookingCreateRequest.OptionsQuestions)
        {
            var planQuestion = new ReservationQuestion
            {
                Reservation = newReservation,
                QuestionId = bookingQuestion.QuestionId,
                AnswerData = bookingQuestion.AnswerData,
                ReservationQuestionType = ReservationQuestionTypes.OptionItem,
                IsEnabled = true
            };

            newReservation.ReservationQuestions.Add(planQuestion);
        }

        return newReservation;
    }

    private async Task<ReservationEntity> GetReservationDataAsync(
        BookingCreateRequest bookingCreateRequest,
        ReservationEntity newReservation,
        List<Question>? selectedQuestions,
        BookingCancellationPolicyModel? bookingCancellationPolicyModel,
        string rootCode,
        long? existingReservationId,
        CancellationToken cancellationToken = default
    )
    {
        var bookingAdjustRequest = bookingCreateRequest.Adjust;

        var availableBookingPlan = await _bookingSearchService.GetBookingDataDetailByPlanAsync(
                new BookingPlanDetailRequest(
                    bookingCreateRequest.FacilityId,
                    bookingCreateRequest.SiteId,
                    bookingCreateRequest.PlanId,
                    bookingCreateRequest.RoomGroupId,
                    bookingAdjustRequest.CheckInDateId,
                    bookingAdjustRequest.NumberOfNights,
                    null
                ),
                new BookingSearchPlanRequest
                {
                    UseCache = false,
                    CheckInDate = bookingCreateRequest.CheckInDate,
                    CheckOutDate = bookingCreateRequest.Adjust.GetDateEndNight(),
                    RestNumber = bookingCreateRequest.Adjust.NumberOfNights,
                    RoomNumber = bookingCreateRequest.Adjust.NumberOfRooms,
                    GuestsPerRoom = bookingCreateRequest.Adjust.GetGuestsPerRoomForSearchModel(),
                    OptionItems = bookingCreateRequest.Adjust.GetOptionItemsForSearchModel()
                },
                cancellationToken
            )
            ?? throw new PlanNotfoundException();

        var bookingRoomAppDates = _bookingRoomAppDateService
            .GetAllRoomAppDates(
                bookingCreateRequest,
                availableBookingPlan
            )
            .ToList();

        var bookingReservationPriceData = bookingCreateRequest.PlanType is PlanTypes.Combo
            ? bookingReservationPriceDataOfPlanService
                .GetAllReservationPriceData(
                    bookingCreateRequest,
                    availableBookingPlan,
                    bookingRoomAppDates
                )
                .ToList()
            :
            [
                .. bookingReservationPriceDataOfRoomService
                    .GetAllReservationPriceData(
                        bookingCreateRequest,
                        availableBookingPlan,
                        bookingRoomAppDates
                    )
            ];

        var bookingReservationOptionItemData = _bookingOptionInventoryService
            .GetAllReservationOptionItemData(
                bookingCreateRequest,
                availableBookingPlan,
                bookingRoomAppDates,
                existingReservationId
            )
            .ToList();

        var optionItems = bookingReservationOptionItemData
            .Select(
                x => new OptionOfBookingSearchModel
                {
                    AppDateId = x.AppDateId,
                    RoomGroupIndex = x.RoomGroupIndex,
                    OptionItemId = x.OptionItemInfo!.Id,
                    Number = x.Number ?? 0
                }
            )
            .ToList();

        await _bookingOptionInventoryService.ValidateOptionInventoryAsync(
            bookingCreateRequest.FacilityId,
            bookingCreateRequest.SiteId,
            bookingCreateRequest.PlanId,
            bookingCreateRequest.CheckInDate,
            bookingCreateRequest.Adjust.NumberOfNights,
            optionItems,
            existingReservationId,
            cancellationToken
        );

        for (var nightIndex = 0; nightIndex < bookingAdjustRequest.NumberOfNights; nightIndex++)
        {
            var appDateId = AppDate.GetId(AppDate.GetDateTime(newReservation.CheckInDate).AddDays(nightIndex));

            for (var roomIndex = 0; roomIndex < bookingAdjustRequest.NumberOfRooms; roomIndex++)
            {
                var appDateOfPlanRoomInReservation = CreateReservationPlanRoomGroupAppDate(
                    appDateId,
                    nightIndex,
                    roomIndex,
                    newReservation,
                    bookingCreateRequest
                );

                var reservationRoomGroupAppDatePersonAgeTypes = GetReservationPriceData(
                    appDateId,
                    nightIndex,
                    roomIndex,
                    newReservation,
                    bookingReservationPriceData
                );

                appDateOfPlanRoomInReservation.ReservationRoomGroupAppDatePersonAgeTypes = reservationRoomGroupAppDatePersonAgeTypes;

                var reservationRoomGroupAppDateOptionItems = GetReservationOptionItemData(
                    appDateId,
                    nightIndex,
                    roomIndex,
                    newReservation,
                    bookingReservationOptionItemData
                );

                appDateOfPlanRoomInReservation.ReservationRoomGroupAppDateOptionItems = reservationRoomGroupAppDateOptionItems;

                newReservation.ReservationPlanRoomGroupAppDates!.Add(appDateOfPlanRoomInReservation);
            }
        }

        newReservation.BookingData = await _bookingDataService.GetBookingDataAsync(
            newReservation,
            availableBookingPlan,
            bookingReservationPriceData,
            selectedQuestions,
            bookingCancellationPolicyModel,
            cancellationToken
        );

        newReservation.BookingData.MediaCode = availableBookingPlan.GetMediaCode() ?? string.Empty;

        newReservation.BookingData.TimeZoneOffset = bookingCreateRequest.TimeZoneOffset;
        newReservation.BookingData.LanguageCode = LanguageHeaderUtil.GetLanguageCodeFromHeader();
        newReservation.BookingData.RootCode = rootCode;

        foreach (var reservationPlanRoomGroupAppDate in newReservation.ReservationPlanRoomGroupAppDates!)
        {
            foreach (var item in reservationPlanRoomGroupAppDate.ReservationRoomGroupAppDatePersonAgeTypes!)
            {
                item.PersonAgeType = null;
            }

            foreach (var item in reservationPlanRoomGroupAppDate.ReservationRoomGroupAppDateOptionItems!)
            {
                item.OptionItem = null;
            }
        }

        return newReservation;
    }

    private static ReservationPlanRoomGroupAppDate CreateReservationPlanRoomGroupAppDate(
        long appDateId,
        int nightIndex,
        int roomIndex,
        ReservationEntity newReservation,
        BookingCreateRequest bookingCreateRequest
    )
    {
        var bookingAdjustRequest = bookingCreateRequest.Adjust;

        var appDateOfPlanRoomInReservation = new ReservationPlanRoomGroupAppDate
        {
            Reservation = newReservation,
            PlanId = bookingCreateRequest.PlanId,
            RoomGroupId = bookingCreateRequest.RoomGroupId,
            BookingDateId = appDateId,
            RestIndex = nightIndex,
            RoomGroupIndex = roomIndex,
            ReservationRoomGroupAppDatePersonAgeTypes = [],
            ReservationRoomGroupAppDateOptionItems = []
        };

        var roomRepresentative = bookingAdjustRequest
            .RoomRepresentatives?
            .FirstOrDefault(x => x.RoomIndex == roomIndex);
        if (roomRepresentative != null)
        {
            appDateOfPlanRoomInReservation.CustomerInfo = new()
            {
                Name = roomRepresentative.FullName,
                Kana = roomRepresentative.Kana
            };
        }

        return appDateOfPlanRoomInReservation;
    }

    private static List<ReservationRoomGroupAppDatePersonAgeType> GetReservationPriceData(
        long appDateId,
        int nightIndex,
        int roomIndex,
        ReservationEntity newReservation,
        List<BookingReservationPriceData> bookingReservationPriceData
    )
    {
        var data = new List<ReservationRoomGroupAppDatePersonAgeType>();

        var reservationPriceData = bookingReservationPriceData
            .Where(x => x.AppDateId == appDateId)
            .Where(x => x.RoomGroupIndex == roomIndex);

        foreach (var priceData in reservationPriceData)
        {
            if (priceData.IsPersonsMatch != null && !priceData.IsPersonsMatch.Value)
            {
                throw new ReservationNoMatchPersonsException();
            }

            var reservationRoomGroupAppDatePersonAgeType = new ReservationRoomGroupAppDatePersonAgeType
            {
                Reservation = newReservation,
                RoomGroupId = newReservation.RoomGroupId,
                BookingDateId = appDateId,
                PersonAgeTypeId = priceData.PersonAgeType?.Id ?? 0,
                PersonAgeType = new PersonAgeType
                {
                    Id = priceData.PersonAgeType?.Id ?? 0,
                    Name =
                        new MultilingualText
                        {
                            { LanguageHeaderUtil.GetLanguageCodeFromHeader(), priceData.PersonAgeType?.Name ?? string.Empty }
                        },
                    IsMain = priceData.PersonAgeType?.IsMain ?? false,
                    AgeMax = priceData.PersonAgeType?.AgeMax ?? 0,
                    AgeMin = priceData.PersonAgeType?.AgeMin ?? 0
                },
                RestIndex = nightIndex,
                RoomGroupIndex = roomIndex,
                UnitPrice = priceData.Price ?? 0,
                SpaTax = priceData.SpaTax ?? 0,
                MaleNumber = priceData.MalePersons ?? 0,
                FemaleNumber = priceData.FemalePersons ?? 0,
                GenderNoneNumber = priceData.NonePersons ?? 0
            };

            data.Add(reservationRoomGroupAppDatePersonAgeType);
        }

        return data;
    }

    private static List<ReservationRoomGroupAppDateOptionItem> GetReservationOptionItemData(
        long appDateId,
        int nightIndex,
        int roomIndex,
        ReservationEntity newReservation,
        List<BookingReservationOptionItemData> bookingReservationOptionItemData
    )
    {
        var data = new List<ReservationRoomGroupAppDateOptionItem>();

        var reservationOptionItemData = bookingReservationOptionItemData
            .Where(a => a.AppDateId == appDateId)
            .Where(a => a.RoomGroupIndex == roomIndex);

        foreach (var optionItemData in reservationOptionItemData)
        {
            var reservationRoomAppDateOptionItem = new ReservationRoomGroupAppDateOptionItem
            {
                Reservation = newReservation,
                RoomGroupId = newReservation.RoomGroupId,
                BookingDateId = appDateId,
                OptionItemId = optionItemData.OptionItemInfo?.Id ?? 0,
                OptionItem =
                    new OptionItem
                    {
                        Name = optionItemData.OptionItemInfo?.Name
                    },
                RestIndex = nightIndex,
                RoomGroupIndex = roomIndex,
                Price = optionItemData.Price,
                Number = optionItemData.Number ?? 0
            };

            data.Add(reservationRoomAppDateOptionItem);
        }

        return data;
    }

    private static ReservationStatus DetermineReservationState(
        bool isOnlinePayment,
        ReservationEntity? existingReservation
    )
    {
        if (isOnlinePayment && existingReservation is null)
        {
            return ReservationStatus.Temporary;
        }

        return existingReservation is null ? ReservationStatus.Reserved : ReservationStatus.Modified;
    }
}
