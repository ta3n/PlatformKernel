using Liberty.Entity.ValueObjects;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Metas;
using Liberty.Reservation.Application.Cqrs.BaseQueries;
using Liberty.Reservation.Application.Domains.Services.Interfaces;
using Liberty.Reservation.Application.Exceptions;
using Liberty.Reservation.Application.Utils;
using Liberty.Reservation.Manager.Application.Auth;
using Liberty.Reservation.Manager.WebAPI.Application.Models;
using Newtonsoft.Json;
using SqlKata;
using SqlKata.Execution;
using FacilityEntity = Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data.Facility;
using PlanEntity = Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data.Plan;
using ReservationEntity = Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data.Reservation;
using RoomGroupEntity = Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data.RoomGroup;

namespace Liberty.Reservation.Manager.WebAPI.Application.UserCases.Queries.BookingReservation;

public class ReservationGetDetailsQueryHandler(
    IMapper mapper,
    ISecurityContextAccessor securityContextAccessor,
    IUnitOfWork unitOfWork,
    IBookingReservationService bookingReservationService,
    IBookingManagerModificationCheckerService bookingModificationCheckerService,
    QueryFactory queryFactory
) : QuerySingleBaseHandler<ReservationGetDetailsQuery, ReservationDetailResponse>(mapper)
{
    private readonly EntityProperty _reservationProp = unitOfWork.GetEntityProperty<ReservationEntity>();
    private readonly EntityProperty _facilityProp = unitOfWork.GetEntityProperty<FacilityEntity>();
    private readonly EntityProperty _planProp = unitOfWork.GetEntityProperty<PlanEntity>();
    private readonly EntityProperty _roomGroupProp = unitOfWork.GetEntityProperty<RoomGroupEntity>();
    private readonly EntityProperty _reserverProp = unitOfWork.GetEntityProperty<CustomerInfo>();
    private readonly EntityProperty _mainUserProp = unitOfWork.GetEntityProperty<CustomerInfo>();

    protected override async Task<(IHeaderDictionary, ReservationDetailResponse)> HandleAsync(
        ReservationGetDetailsQuery request,
        CancellationToken cancellationToken
    )
    {
        var facilityId = securityContextAccessor.FacilityKey;

        _reserverProp.Alias = "Reserver";
        _mainUserProp.Alias = "MainUser";

        var query = BuildReservationQuery(request.Id, facilityId);

        var result = await queryFactory
                .FromQuery(query)
                .FirstOrDefaultAsync<ReservationQueryResult>(cancellationToken: cancellationToken)
            ?? throw new ReservationNotfoundException(request.Id);

        var response = await MapToReservationDetailResponseAsync(result, facilityId, cancellationToken);

        return (new HeaderDictionary(), response);
    }

    private async Task<ReservationDetailResponse> MapToReservationDetailResponseAsync(
        ReservationQueryResult result,
        long facilityId,
        CancellationToken cancellationToken
    )
    {
        var personAgeTypes = PersonAgeTypes(
            result,
            out var appDates,
            out var planQuestions,
            out var optionQuestions,
            out var planData,
            out var siteData
        );

        var cancellationDatas = planData?.CancellationDataPolicy?.CancellationData ?? [];

        var roomGroupData = result.RoomGroupData is null
            ? new RoomGroupData()
            : JsonConvert.DeserializeObject<RoomGroupData>(result.RoomGroupData);
        var facilityData = result.FacilityData is null
            ? new FacilityData()
            : JsonConvert.DeserializeObject<FacilityData>(result.FacilityData);
        var (timeZoneId, timeZone) = bookingReservationService.GetFacilityTimeZoneInfoById(facilityId);

        var reservation = new ReservationEntity
        {
            ReservationState = (ReservationStatus)result.ReservationState,
            CheckInDate = result.CheckInDate,
            CheckInTime = result.CheckInTime,
            Plan = new PlanEntity
            {
                ReceptionDayLimit = result.ReceptionDayLimit,
                ReceptionLimit = result.ReceptionLimit,
                IsCancelSameAccept = result.IsCancelSameAccept,
                CancelDayLimit = result.CancelDayLimit,
                CancelLimit = result.CancelLimit
            },
            BookingData = new BookingData
            {
                Facility = new FacilityData(),
                Plan = planData ?? new PlanData(),
                RoomGroup = new RoomGroupData(),
                Site = new SiteData(),
                TotalRoomPrice = result.AccommodationFee,
                TotalSpaTax = result.TaxFee,
                TotalOptionPrice = result.OptionalFee,
                TimeZoneOffset = Convert.ToInt32(result.TimeZoneOffset)
            },
            Id = result.Id,
            FacilityId = facilityId
        };
        var checkOutDate = AppDate.GetDateTime(result.CheckInDate).AddDays(result.NumberOfNights);

        var isChange = bookingModificationCheckerService.CanBookingChangeModify(
            AppDate.GetId(checkOutDate),
            Convert.ToBoolean(result.IsNoShow),
            reservation.ReservationState,
            timeZone
        );

        var isCancel = bookingModificationCheckerService.CanCancelModify(
            AppDate.GetId(checkOutDate),
            Convert.ToBoolean(result.IsNoShow),
            reservation.ReservationState,
            timeZone
        );

        if (
            (CancellationStatus)result.CancellationStatus == CancellationStatus.CancelledLocalFinalized
            || (CancellationStatus)result.CancellationStatus == CancellationStatus.CancelledDepositRefunded
            || ((ReservationStatus)result.ReservationState == ReservationStatus.UserCanceled
                && (PaymentTypes)result.PaymentType == PaymentTypes.OnLinePayment))
        {
            isCancel = false;
        }

        var cancellationPrice = result.CancellationPrice;
        var bookingLanguageCode = $"{result.LanguageCode}";
        var barrierFreeInfoComment = DeserializeMultilingualText(result.BarrierFreeInfoComment);
        var spaTaxComment = DeserializeMultilingualText(result.SpaTaxComment);
        var spaTaxTable = DeserializeMultilingualText(result.SpaTaxTable);
        var payment = DeserializeMultilingualText(result.Payment);
        var meal = DeserializeMultilingualText(result.Meal);
        var other = DeserializeMultilingualText(result.Other);
        var cancellationName = planData?.CancellationDataPolicy?.Name?.GetValueByCode(bookingLanguageCode);
        var cancellationDescription = planData?.CancellationDataPolicy?.Description?.GetValueByCode(bookingLanguageCode);
        var cancellationTableSource = planData?.CancellationDataPolicy?.TableSource?.GetValueByCode(bookingLanguageCode);
        var cancellationRuleDetail = planData?.CancellationDataPolicy?.RuleDetail?.GetValueByCode(bookingLanguageCode);

        var roomRepresentatives = result.UseRoomUser
            ? appDates
                ?.SelectMany(x => x.Rooms)
                .Where(x => x.CustomerInfo is not null)
                .DistinctBy(y => y.RoomIndex)
                .Select(
                    x => new RoomRepresentativeResponse(x.RoomIndex, x.CustomerInfo?.Name, x.CustomerInfo?.Kana)
                )
                .ToList()
            : null;

        var reservationState = (ReservationStatus)result.ReservationState;
        var isNoShow = Convert.ToBoolean(result.IsNoShow);

        var canNoShow = bookingModificationCheckerService.CanNoShowModify(
            reservation.CheckInTime ?? TimeSpan.Zero,
            reservation.CheckInDate,
            AppDate.GetId(checkOutDate),
            isNoShow,
            reservationState,
            timeZone
        );
        var isOnlinePaymentModificationPermitted = await bookingReservationService.IsBookingHasFacilitySetOnlyOnlinePaymentMethodAsync(
            result.Id,
            cancellationToken
        );
        DateTime? updateAt = result.ModifiedDateTime != null
            ? Convert.ToDateTime(result.ModifiedDateTime).ToUniversalTime()
            : null;

        var useDailyPerson = isCancel && result.UseDailyPerson;
        var canAddRoomOnModify = isCancel && result.CanAddRoomOnModify;

        DateTime? cancellationDate = result.CancellationDate != null
            ? Convert.ToDateTime(result.CancellationDate).ToUniversalTime()
            : null;

        var response = new ReservationDetailResponse
        {
            Id = result.Id,
            Code = result.Code,
            State = (ReservationStatus)result.ReservationState,
            CancellationFeeType = (CancellationFeeType)result.CancellationFeeType,
            CancellationStatus = (CancellationStatus)result.CancellationStatus,
            FacilityName = facilityData?.Name,
            FacilityTimeZoneId = timeZoneId,
            FacilityTimeZone = timeZone,
            PlanName = planData?.Name,
            SiteName = siteData?.Name,
            CheckInStart = result.CheckInStart,
            CheckInEnd = result.CheckInEnd,
            TimeIntervalMinutes = result.TimeIntervalMinutes,
            LanguageCode = result.LanguageCode,
            RoomGroupName = roomGroupData?.Name,
            CheckInDate = result.CheckInDate,
            CheckInTime = result.CheckInTime,
            NumberOfNights = result.NumberOfNights,
            NumberOfRooms = result.NumberOfRooms,
            PaymentType = (PaymentTypes)result.PaymentType,
            AccommodationFee = result.AccommodationFee,
            OptionalFee = result.OptionalFee,
            TaxFee = result.TaxFee,
            TotalFee = result.TotalFee,
            FreeInput = result.FreeInput,
            IsSameMainUser = result.IsSameMainUser,
            ImportantNotes = new ImportantNotesOfReservationDetailResponse(
                payment,
                meal,
                other
            ),
            Cancellation = new CancellationOfReservationResponse(
                cancellationName,
                cancellationDescription,
                cancellationRuleDetail,
                cancellationTableSource,
                cancellationDatas.Select(
                    x =>
                        new CancellationDataOfReservationResponse(
                            x.DayStart,
                            x.DayEnd,
                            x.Rate
                        )
                )
            ),
            Reserver = new ReserverResponse(
                result.ReserverName,
                result.ReserverKana,
                result.ReserverEmail,
                result.ReserverAddress1,
                result.ReserverAddress2,
                result.ReserverAddress3,
                result.ReserverCountryCode,
                result.ReserverPostCode,
                result.ReserverPhone,
                (Genders)(result.ReserverGender ?? 0)
            ),
            Customer = new GuestResponse(
                result.MainUserName,
                result.MainUserKana,
                result.MainUserAddress1,
                result.MainUserAddress2,
                result.MainUserAddress3,
                result.MainUserCountryCode,
                result.MainUserPostCode,
                result.MainUserBirthDay,
                (Genders)(result.MainUserGender ?? 0),
                result.MainUserPhone
            ),
            PlanQuestions = planQuestions,
            OptionQuestions = optionQuestions,
            PersonAgeTypes = personAgeTypes,
            AppDates = appDates?.Select(
                bookingAppDateData =>
                    new BookingAppDateResponse(
                        bookingAppDateData.AppDateId,
                        bookingAppDateData.RestIndex,
                        bookingAppDateData.Price,
                        bookingAppDateData.SpaTax,
                        bookingAppDateData.OptionPrice,
                        bookingAppDateData.TotalPrice,
                        bookingAppDateData.Rooms.Select(
                            bookingRoomDataOfAppDate =>
                                new BookingRoomOfAppDate(
                                    bookingRoomDataOfAppDate.RoomIndex,
                                    bookingRoomDataOfAppDate.RoomPrice,
                                    bookingRoomDataOfAppDate.PricePeoples.Select(
                                        peoplePriceDataOfRoom =>
                                            new PeoplePriceOfRoomResponse(
                                                peoplePriceDataOfRoom.RoomPrice,
                                                peoplePriceDataOfRoom.SpaTax,
                                                peoplePriceDataOfRoom.Persons,
                                                peoplePriceDataOfRoom.PersonAgeType.Id,
                                                peoplePriceDataOfRoom.PersonAgeType.Name,
                                                peoplePriceDataOfRoom.PersonAgeType.IsMain,
                                                peoplePriceDataOfRoom.MalePersons,
                                                peoplePriceDataOfRoom.FemalePersons,
                                                peoplePriceDataOfRoom.NonePersons,
                                                peoplePriceDataOfRoom.OtherPersons,
                                                peoplePriceDataOfRoom.TotalPrice
                                            )
                                    ),
                                    bookingRoomDataOfAppDate.OptionItems?.Select(
                                        optionItemDataOfRoom =>
                                            new OptionItemOfRoomResponse(
                                                optionItemDataOfRoom.Id,
                                                optionItemDataOfRoom.Name,
                                                optionItemDataOfRoom.Price,
                                                optionItemDataOfRoom.Number
                                            )
                                    ),
                                    bookingRoomDataOfAppDate.SpaTax,
                                    bookingRoomDataOfAppDate.TotalOptionPrice,
                                    bookingRoomDataOfAppDate.Persons
                                )
                        )
                    )
            ),
            RoomRepresentatives = roomRepresentatives,
            IsChange = isChange,
            IsCancel = isCancel,
            UseDailyPerson = useDailyPerson,
            CanAddRoomOnModify = canAddRoomOnModify,
            CancellationPrice = cancellationPrice,
            IsBarrierFree = Convert.ToBoolean(result.IsBarrierFree),
            BarrierFreeInfoComment = barrierFreeInfoComment,
            UseSpaTax = Convert.ToBoolean(result.UseSpaTax),
            SpaTaxComment = spaTaxComment,
            SpaTaxTable = spaTaxTable,
            PlanType = (PlanTypes)result.PlanType,
            CapacityMax = result.CapacityMax,
            IsNoShow = Convert.ToBoolean(result.IsNoShow),
            NoShowReason = result.NoShowReason,
            NoShowDateTime = result.NoShowDateTime,
            IsManagerNoShow = canNoShow,
            IsFacilityOnlySetOnlinePaymentMethod = isOnlinePaymentModificationPermitted,
            DayUse = Convert.ToBoolean(planData?.DayUse),
            UpdatedAt = updateAt,
            UpdateCount = result.UpdateCount,
            CancellationDate = cancellationDate,
            CancellationRule = cancellationRuleDetail,
            CancellationRate = result.CancellationRateFee,
            IsExtendedStayOnModify = result.IsExtendedStayOnModify
        };

        return response;

        static string? DeserializeMultilingualText(
            string? json
        )
        {
            return json != null
                ? JsonConvert.DeserializeObject<MultilingualText>(json)?.GetValueByHeader(DefaultValues.LanguageCode)
                : string.Empty;
        }
    }

    private static List<PersonAgeTypesResponse>? PersonAgeTypes(
        ReservationQueryResult result,
        out List<BookingAppDateData>? appDates,
        out List<ReservationQuestionResponse>? planQuestions,
        out List<ReservationQuestionResponse>? optionQuestions,
        out PlanData? planData,
        out SiteData? siteData
    )
    {
        var personAgeTypes = result.PersonAgeTypes is null
            ? []
            : JsonConvert.DeserializeObject<List<PersonAgeTypesResponse>>(result.PersonAgeTypes);

        appDates = result.AppDates is null
            ? []
            : JsonConvert.DeserializeObject<List<BookingAppDateData>>(result.AppDates);

        planQuestions = result.PlanQuestions is null
            ? []
            : JsonConvert.DeserializeObject<List<ReservationQuestionResponse>>(
                result.PlanQuestions
            );

        optionQuestions = result.OptionQuestions is null
            ? []
            : JsonConvert.DeserializeObject<List<ReservationQuestionResponse>>(
                result.OptionQuestions
            );

        planData = result.PlanData is null
            ? new PlanData()
            : JsonConvert.DeserializeObject<PlanData>(result.PlanData);

        siteData = result.SiteData is null
            ? new SiteData()
            : JsonConvert.DeserializeObject<SiteData>(result.SiteData);
        return personAgeTypes;
    }

    private Query BuildReservationQuery(
        long id,
        long facilityId
    )
    {
        var query = new Query(_reservationProp.TableName);

        query = LeftJoinQuery(query);
        query = SelectQuery(query);
        query = FilterQuery(query, id, facilityId);
        query = GroupQuery(query);

        return query;
    }

    private Query LeftJoinQuery(
        Query query
    )
    {
        return query
            .LeftJoin(
                _reserverProp.TableNameAlias,
                j =>
                    j.On(
                        _reservationProp.FullColumnName(nameof(ReservationEntity.ReserverId)),
                        _reserverProp.FullColumnName(nameof(CustomerInfo.Id))
                    )
            )
            .LeftJoin(
                _mainUserProp.TableNameAlias,
                j =>
                    j.On(
                        _reservationProp.FullColumnName(nameof(ReservationEntity.MainUserId)),
                        _mainUserProp.FullColumnName(nameof(CustomerInfo.Id))
                    )
            )
            .LeftJoin(
                _facilityProp.TableName,
                j =>
                    j.On(
                        _reservationProp.FullColumnName(nameof(ReservationEntity.FacilityId)),
                        _facilityProp.FullColumnName(nameof(FacilityEntity.Id))
                    )
            )
            .LeftJoin(
                _planProp.TableName,
                j =>
                    j.On(
                        _reservationProp.FullColumnName(nameof(ReservationEntity.PlanId)),
                        _planProp.FullColumnName(nameof(PlanEntity.Id))
                    )
            )
            .LeftJoin(
                _roomGroupProp.TableName,
                j =>
                    j.On(
                        _reservationProp.FullColumnName(nameof(ReservationEntity.RoomGroupId)),
                        _roomGroupProp.FullColumnName(nameof(RoomGroupEntity.Id))
                    )
            );
    }

    private Query SelectQuery(
        Query query
    )
    {
        const string bookingDataName = nameof(ReservationEntity.BookingData);
        const string facilityMetaName = nameof(FacilityEntity.Meta);

        return query
            .Select(
                $"{_reservationProp.FullColumnName(nameof(ReservationEntity.Id))} AS {nameof(ReservationQueryResult.Id)}",
                $"{_reservationProp.FullColumnName(nameof(ReservationEntity.FacilityId))} AS {nameof(ReservationQueryResult.FacilityId)}",
                $"{_reservationProp.FullColumnName(nameof(ReservationEntity.IsDeleted))} AS {nameof(ReservationQueryResult.IsDeleted)}",
                $"{_reservationProp.FullColumnName(nameof(ReservationEntity.Code))} AS {nameof(ReservationQueryResult.Code)}",
                $"{_reservationProp.FullColumnName(nameof(ReservationEntity.ReservationState))} AS {nameof(ReservationQueryResult.ReservationState)}",
                $"{_reservationProp.FullColumnName(nameof(ReservationEntity.CheckInDate))} AS {nameof(ReservationQueryResult.CheckInDate)}",
                $"{_reservationProp.FullColumnName(nameof(ReservationEntity.CheckInTime))} AS {nameof(ReservationQueryResult.CheckInTime)}",
                $"{_reservationProp.FullColumnName(nameof(ReservationEntity.RestNumber))} AS {nameof(ReservationQueryResult.NumberOfNights)}",
                $"{_reservationProp.FullColumnName(nameof(ReservationEntity.RoomNumber))} AS {nameof(ReservationQueryResult.NumberOfRooms)}",
                $"{_reservationProp.FullColumnName(nameof(ReservationEntity.PaymentType))} AS {nameof(ReservationQueryResult.PaymentType)}",
                $"{_reservationProp.FullColumnName(nameof(ReservationEntity.Memo))} AS {nameof(ReservationQueryResult.FreeInput)}",
                $"{_reservationProp.FullColumnName(nameof(ReservationEntity.IsSameMainUser))} AS {nameof(ReservationQueryResult.IsSameMainUser)}",
                $"{_reservationProp.FullColumnName(nameof(ReservationEntity.ModifiedDateTime))} AS ModifiedDateTime",
                $"{_reservationProp.FullColumnName(nameof(ReservationEntity.UpdateCount))} AS UpdateCount",
                $"{_reservationProp.FullColumnName(nameof(ReservationEntity.UseRoomUser))} AS {nameof(ReservationQueryResult.UseRoomUser)}",
                $"{_reservationProp.FullColumnName(nameof(ReservationEntity.IsNoShow))} AS {nameof(ReservationQueryResult.IsNoShow)}",
                $"{_reservationProp.FullColumnName(nameof(ReservationEntity.NoShowReason))} AS {nameof(ReservationQueryResult.NoShowReason)}",
                $"{_reservationProp.FullColumnName(nameof(ReservationEntity.NoShowDateTime))} AS {nameof(ReservationQueryResult.NoShowDateTime)}",
                $"{_reservationProp.FullColumnName(nameof(ReservationEntity.ModifiedDateTime))} AS {nameof(ReservationQueryResult.ModifiedDateTime)}",
                $"{_reservationProp.FullColumnName(nameof(ReservationEntity.UpdateCount))} AS {nameof(ReservationQueryResult.UpdateCount)}",
                $"{_reservationProp.FullColumnName(nameof(ReservationEntity.CancellationStatus))} AS {nameof(ReservationQueryResult.CancellationStatus)}",
                $"{_reservationProp.FullColumnName(nameof(ReservationEntity.CancellationPrice))} AS {nameof(ReservationQueryResult.CancellationPrice)}",
                $"{_reservationProp.FullColumnName(nameof(ReservationEntity.CancellationFeeType))} AS {nameof(ReservationQueryResult.CancellationFeeType)}",
                $"{_reservationProp.FullColumnName(nameof(ReservationEntity.CancelledDateTime))} AS {nameof(ReservationQueryResult.CancellationDate)}",
                $"{_reservationProp.FullColumnName(nameof(ReservationEntity.CancelRateFee))} AS {nameof(ReservationQueryResult.CancellationRateFee)}",
                $"{_facilityProp.FullColumnName(nameof(FacilityEntity.UseDailyPerson))} AS {nameof(ReservationQueryResult.UseDailyPerson)}",
                $"{_facilityProp.FullColumnName(nameof(FacilityEntity.CanAddRoomOnModify))} AS {nameof(ReservationQueryResult.CanAddRoomOnModify)}",
                $"{_facilityProp.FullColumnName(nameof(FacilityEntity.BarrierFreeInfoComment))} AS {nameof(ReservationQueryResult.BarrierFreeInfoComment)}",
                $"{_facilityProp.FullColumnName(nameof(FacilityEntity.SpaTaxComment))} AS {nameof(ReservationQueryResult.SpaTaxComment)}",
                $"{_facilityProp.FullColumnName(nameof(FacilityEntity.SpaTaxTable))} AS {nameof(ReservationQueryResult.SpaTaxTable)}",
                $"{_facilityProp.FullColumnName(nameof(FacilityEntity.IsExtendedStayOnModify))} AS {nameof(ReservationQueryResult.IsExtendedStayOnModify)}",
                $"{_planProp.FullColumnName(nameof(PlanEntity.ReceptionDayLimit))} AS {nameof(ReservationQueryResult.ReceptionDayLimit)}",
                $"{_planProp.FullColumnName(nameof(PlanEntity.ReceptionLimit))} AS {nameof(ReservationQueryResult.ReceptionLimit)}",
                $"{_planProp.FullColumnName(nameof(PlanEntity.IsCancelSameAccept))} AS {nameof(ReservationQueryResult.IsCancelSameAccept)}",
                $"{_planProp.FullColumnName(nameof(PlanEntity.CancelDayLimit))} AS {nameof(ReservationQueryResult.CancelDayLimit)}",
                $"{_planProp.FullColumnName(nameof(PlanEntity.CancelLimit))} AS {nameof(ReservationQueryResult.CancelLimit)}",
                $"{_planProp.FullColumnName(nameof(PlanEntity.CheckInStart))} AS {nameof(ReservationQueryResult.CheckInStart)}",
                $"{_planProp.FullColumnName(nameof(PlanEntity.CheckInEnd))} AS {nameof(ReservationQueryResult.CheckInEnd)}",
                $"{_planProp.FullColumnName(nameof(PlanEntity.TimeIntervalMinutes))} AS {nameof(ReservationQueryResult.TimeIntervalMinutes)}",
                $"{_planProp.FullColumnName(nameof(PlanEntity.PlanType))} AS {nameof(ReservationQueryResult.PlanType)}",
                $"{_planProp.FullColumnName(nameof(PlanEntity.Payment))} AS {nameof(ReservationQueryResult.Payment)}",
                $"{_planProp.FullColumnName(nameof(PlanEntity.Meal))} AS {nameof(ReservationQueryResult.Meal)}",
                $"{_planProp.FullColumnName(nameof(PlanEntity.Other))} AS {nameof(ReservationQueryResult.Other)}",
                $"{_reserverProp.FullColumnName(nameof(CustomerInfo.Name))} AS {nameof(ReservationQueryResult.ReserverName)}",
                $"{_reserverProp.FullColumnName(nameof(CustomerInfo.Kana))} AS {nameof(ReservationQueryResult.ReserverKana)}",
                $"{_reserverProp.FullColumnName(nameof(CustomerInfo.EMail))} AS {nameof(ReservationQueryResult.ReserverEmail)}",
                $"{_reserverProp.FullColumnName(nameof(CustomerInfo.Address1))} AS {nameof(ReservationQueryResult.ReserverAddress1)}",
                $"{_reserverProp.FullColumnName(nameof(CustomerInfo.Address2))} AS {nameof(ReservationQueryResult.ReserverAddress2)}",
                $"{_reserverProp.FullColumnName(nameof(CustomerInfo.Address3))} AS {nameof(ReservationQueryResult.ReserverAddress3)}",
                $"{_reserverProp.FullColumnName(nameof(CustomerInfo.CountryCode))} AS {nameof(ReservationQueryResult.ReserverCountryCode)}",
                $"{_reserverProp.FullColumnName(nameof(CustomerInfo.PostCode))} AS {nameof(ReservationQueryResult.ReserverPostCode)}",
                $"{_reserverProp.FullColumnName(nameof(CustomerInfo.Phone))} AS {nameof(ReservationQueryResult.ReserverPhone)}",
                $"{_reserverProp.FullColumnName(nameof(CustomerInfo.Gender))} AS {nameof(ReservationQueryResult.ReserverGender)}",
                $"{_mainUserProp.FullColumnName(nameof(CustomerInfo.Name))} AS {nameof(ReservationQueryResult.MainUserName)}",
                $"{_mainUserProp.FullColumnName(nameof(CustomerInfo.Kana))} AS {nameof(ReservationQueryResult.MainUserKana)}",
                $"{_mainUserProp.FullColumnName(nameof(CustomerInfo.Address1))} AS {nameof(ReservationQueryResult.MainUserAddress1)}",
                $"{_mainUserProp.FullColumnName(nameof(CustomerInfo.Address2))} AS {nameof(ReservationQueryResult.MainUserAddress2)}",
                $"{_mainUserProp.FullColumnName(nameof(CustomerInfo.Address3))} AS {nameof(ReservationQueryResult.MainUserAddress3)}",
                $"{_mainUserProp.FullColumnName(nameof(CustomerInfo.CountryCode))} AS {nameof(ReservationQueryResult.MainUserCountryCode)}",
                $"{_mainUserProp.FullColumnName(nameof(CustomerInfo.PostCode))} AS {nameof(ReservationQueryResult.MainUserPostCode)}",
                $"{_mainUserProp.FullColumnName(nameof(CustomerInfo.BirthDay))} AS {nameof(ReservationQueryResult.MainUserBirthDay)}",
                $"{_mainUserProp.FullColumnName(nameof(CustomerInfo.Gender))} AS {nameof(ReservationQueryResult.MainUserGender)}",
                $"{_mainUserProp.FullColumnName(nameof(CustomerInfo.Phone))} AS {nameof(ReservationQueryResult.MainUserPhone)}",
                $"{_roomGroupProp.FullColumnName(nameof(RoomGroupEntity.CapacityMax))} AS {nameof(ReservationQueryResult.CapacityMax)}"
            )
            .SelectRaw(
                @$"{_reservationProp.FullColumnName(bookingDataName)}->>'{nameof(ReservationEntity.BookingData.Plan)}' AS ""{nameof(ReservationQueryResult.PlanData)}"""
            )
            .SelectRaw(
                @$"{_reservationProp.FullColumnName(bookingDataName)}->>'{nameof(ReservationEntity.BookingData.Site)}' AS ""{nameof(ReservationQueryResult.SiteData)}"""
            )
            .SelectRaw(
                @$"{_reservationProp.FullColumnName(bookingDataName)}->>'{nameof(ReservationEntity.BookingData.Facility)}' AS ""{nameof(ReservationQueryResult.FacilityData)}"""
            )
            .SelectRaw(
                @$"{_reservationProp.FullColumnName(bookingDataName)}->>'{nameof(ReservationEntity.BookingData.RoomGroup)}' AS ""{nameof(ReservationQueryResult.RoomGroupData)}"""
            )
            .SelectRaw(
                @$"CAST({_reservationProp.FullColumnName(bookingDataName)}->>'{nameof(ReservationEntity.BookingData.TotalRoomPrice)}' AS DECIMAL) AS ""{nameof(ReservationQueryResult.AccommodationFee)}"""
            )
            .SelectRaw(
                @$"CAST({_reservationProp.FullColumnName(bookingDataName)}->>'{nameof(ReservationEntity.BookingData.TotalOptionPrice)}' AS DECIMAL) AS ""{nameof(ReservationQueryResult.OptionalFee)}"""
            )
            .SelectRaw(
                @$"CAST({_reservationProp.FullColumnName(bookingDataName)}->>'{nameof(ReservationEntity.BookingData.TotalSpaTax)}' AS DECIMAL) AS ""{nameof(ReservationQueryResult.TaxFee)}"""
            )
            .SelectRaw(
                @$"CAST({_reservationProp.FullColumnName(bookingDataName)}->>'{nameof(ReservationEntity.BookingData.AllTotalPrice)}' AS DECIMAL) AS ""{nameof(ReservationQueryResult.TotalFee)}"""
            )
            .SelectRaw(
                @$"{_reservationProp.FullColumnName(bookingDataName)}->>'{nameof(ReservationEntity.BookingData.PersonAgeTypes)}' AS ""{nameof(ReservationQueryResult.PersonAgeTypes)}"""
            )
            .SelectRaw(
                @$"{_reservationProp.FullColumnName(bookingDataName)}->>'{nameof(ReservationEntity.BookingData.AppDates)}' AS ""{nameof(ReservationQueryResult.AppDates)}"""
            )
            .SelectRaw(
                @$"{_reservationProp.FullColumnName(bookingDataName)}->>'{nameof(ReservationEntity.BookingData.PlanQuestions)}' AS ""{nameof(ReservationQueryResult.PlanQuestions)}"""
            )
            .SelectRaw(
                @$"{_reservationProp.FullColumnName(bookingDataName)}->>'{nameof(ReservationEntity.BookingData.OptionQuestions)}' AS ""{nameof(ReservationQueryResult.OptionQuestions)}"""
            )
            .SelectRaw(
                @$"{_reservationProp.FullColumnName(bookingDataName)}->>'{nameof(ReservationEntity.BookingData.TimeZoneOffset)}' AS ""{nameof(ReservationQueryResult.TimeZoneOffset)}"""
            )
            .SelectRaw(
                @$"{_reservationProp.FullColumnName(bookingDataName)}->>'{nameof(ReservationEntity.BookingData.LanguageCode)}' AS ""{nameof(ReservationQueryResult.LanguageCode)}"""
            )
            .SelectRaw(
                @$"{_facilityProp.FullColumnName(facilityMetaName)}->>'{nameof(FacilityEntity.Meta.IsBarrierFree)}' AS ""{nameof(ReservationQueryResult.IsBarrierFree)}"""
            )
            .SelectRaw(
                @$"{_facilityProp.FullColumnName(facilityMetaName)}->>'{nameof(FacilityEntity.Meta.UseSpaTax)}' AS ""{nameof(ReservationQueryResult.UseSpaTax)}"""
            );
    }

    private Query FilterQuery(
        Query query,
        long id,
        long facilityId
    )
    {
        return query
            .Where(_reservationProp.FullColumnName(nameof(ReservationEntity.Id)), id)
            .Where(
                _reservationProp.FullColumnName(nameof(ReservationEntity.FacilityId)),
                facilityId
            )
            .WhereFalse(_reservationProp.FullColumnName(nameof(ReservationEntity.IsDeleted)));
    }

    private Query GroupQuery(
        Query query
    )
    {
        return query.GroupBy(
            _reservationProp.FullColumnName(nameof(ReservationEntity.Id)),
            _reservationProp.FullColumnName(nameof(ReservationEntity.Code)),
            _reservationProp.FullColumnName(nameof(ReservationEntity.ReservationState)),
            _reservationProp.FullColumnName(nameof(ReservationEntity.CheckInDate)),
            _reservationProp.FullColumnName(nameof(ReservationEntity.CheckInTime)),
            _reservationProp.FullColumnName(nameof(ReservationEntity.RestNumber)),
            _reservationProp.FullColumnName(nameof(ReservationEntity.RoomNumber)),
            _reservationProp.FullColumnName(nameof(ReservationEntity.PaymentType)),
            _reservationProp.FullColumnName(nameof(ReservationEntity.Memo)),
            _reservationProp.FullColumnName(nameof(ReservationEntity.IsSameMainUser)),
            _reservationProp.FullColumnName(nameof(ReservationEntity.UseRoomUser)),
            _facilityProp.FullColumnName(nameof(FacilityEntity.UseDailyPerson)),
            _facilityProp.FullColumnName(nameof(FacilityEntity.CanAddRoomOnModify)),
            _facilityProp.FullColumnName(nameof(FacilityEntity.BarrierFreeInfoComment)),
            _facilityProp.FullColumnName(nameof(FacilityEntity.SpaTaxComment)),
            _facilityProp.FullColumnName(nameof(FacilityEntity.SpaTaxTable)),
            _facilityProp.FullColumnName(nameof(FacilityEntity.IsExtendedStayOnModify)),
            _planProp.FullColumnName(nameof(PlanEntity.ReceptionDayLimit)),
            _planProp.FullColumnName(nameof(PlanEntity.ReceptionLimit)),
            _planProp.FullColumnName(nameof(PlanEntity.IsCancelSameAccept)),
            _planProp.FullColumnName(nameof(PlanEntity.CancelDayLimit)),
            _planProp.FullColumnName(nameof(PlanEntity.CancelLimit)),
            _planProp.FullColumnName(nameof(PlanEntity.CheckInStart)),
            _planProp.FullColumnName(nameof(PlanEntity.CheckInEnd)),
            _planProp.FullColumnName(nameof(PlanEntity.TimeIntervalMinutes)),
            _planProp.FullColumnName(nameof(PlanEntity.PlanType)),
            _planProp.FullColumnName(nameof(PlanEntity.Payment)),
            _planProp.FullColumnName(nameof(PlanEntity.Meal)),
            _planProp.FullColumnName(nameof(PlanEntity.Other)),
            _reserverProp.FullColumnName(nameof(CustomerInfo.Name)),
            _reserverProp.FullColumnName(nameof(CustomerInfo.Kana)),
            _reserverProp.FullColumnName(nameof(CustomerInfo.EMail)),
            _reserverProp.FullColumnName(nameof(CustomerInfo.Address1)),
            _reserverProp.FullColumnName(nameof(CustomerInfo.Address2)),
            _reserverProp.FullColumnName(nameof(CustomerInfo.Address3)),
            _reserverProp.FullColumnName(nameof(CustomerInfo.CountryCode)),
            _reserverProp.FullColumnName(nameof(CustomerInfo.PostCode)),
            _reserverProp.FullColumnName(nameof(CustomerInfo.Phone)),
            _reserverProp.FullColumnName(nameof(CustomerInfo.Gender)),
            _mainUserProp.FullColumnName(nameof(CustomerInfo.Name)),
            _mainUserProp.FullColumnName(nameof(CustomerInfo.Kana)),
            _mainUserProp.FullColumnName(nameof(CustomerInfo.Address1)),
            _mainUserProp.FullColumnName(nameof(CustomerInfo.Address2)),
            _mainUserProp.FullColumnName(nameof(CustomerInfo.Address3)),
            _mainUserProp.FullColumnName(nameof(CustomerInfo.CountryCode)),
            _mainUserProp.FullColumnName(nameof(CustomerInfo.PostCode)),
            _mainUserProp.FullColumnName(nameof(CustomerInfo.BirthDay)),
            _mainUserProp.FullColumnName(nameof(CustomerInfo.Gender)),
            _mainUserProp.FullColumnName(nameof(CustomerInfo.Phone)),
            _planProp.FullColumnName(nameof(PlanEntity.Meta)),
            _facilityProp.FullColumnName(nameof(FacilityEntity.Meta)),
            _roomGroupProp.FullColumnName(nameof(RoomGroupEntity.CapacityMax))
        );
    }
}
