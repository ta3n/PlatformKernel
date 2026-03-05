using Dapper;
using Liberty.ApplicationShared.Utils;
using Liberty.Reservation.Application.Constants;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;
using Liberty.Reservation.Application.Domains.Services.Interfaces;
using Liberty.Reservation.Application.Exceptions;
using Liberty.Reservation.Application.Models;
using Liberty.Reservation.Application.Models.Requests;
using Liberty.Reservation.Application.Models.Responses;
using Liberty.Reservation.Application.Utils;
using Liberty.UnitOfWork.Abstractions;
using SqlKata;
using SqlKata.Execution;
using static Liberty.Reservation.Application.Models.Responses.SetRoomsResponse;
using ReservationEntity = Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data.Reservation;

namespace Liberty.Reservation.Application.Domains.Services;

public class BookingRoomAppDateService(
    QueryFactory queryFactory,
    IUnitOfWork unitOfWork
) : IBookingRoomAppDateService
{
    private const int MaxRoomLimit = 999;

    private readonly EntityProperty _facilityProp = unitOfWork.GetEntityProperty<Facility>();
    private readonly EntityProperty _roomGroupProp = unitOfWork.GetEntityProperty<RoomGroup>();
    private readonly EntityProperty _roomGroupAppDateProp = unitOfWork.GetEntityProperty<RoomGroupAppDate>();
    private readonly EntityProperty _facilityRoomGroupProp = unitOfWork.GetEntityProperty<FacilityRoomGroup>();
    private readonly EntityProperty _reservationPlanRoomGroupAppDateProp = unitOfWork.GetEntityProperty<ReservationPlanRoomGroupAppDate>();
    private readonly EntityProperty _appDateProp = unitOfWork.GetEntityProperty<AppDate>();
    private readonly EntityProperty _reservationProp = unitOfWork.GetEntityProperty<ReservationEntity>();

    public async Task<List<RoomGroupAppDate>> GetAppDatesOfRoomAsync(
        SetRoomModel.Hotel hotel,
        CancellationToken cancellationToken
    )
    {
        var query = new Query(_roomGroupAppDateProp.TableName)
            .Join(
                _roomGroupProp.TableName,
                _roomGroupProp.FullColumnName(nameof(RoomGroup.Id)),
                _roomGroupAppDateProp.FullColumnName(nameof(RoomGroupAppDate.RoomGroupId))
            )
            .Join(
                _facilityRoomGroupProp.TableName,
                _facilityRoomGroupProp.FullColumnName(nameof(FacilityRoomGroup.RoomGroupId)),
                _roomGroupProp.FullColumnName(nameof(RoomGroup.Id))
            )
            .Join(
                _facilityProp.TableName,
                _facilityProp.FullColumnName(nameof(Facility.Id)),
                _facilityRoomGroupProp.FullColumnName(nameof(FacilityRoomGroup.FacilityId))
            )
            .WhereIn(_roomGroupAppDateProp.FullColumnName(nameof(RoomGroupAppDate.AppDateId)), hotel.Dates)
            .Where(_roomGroupProp.FullColumnName(nameof(RoomGroup.GroupName)), hotel.RoomId)
            .Where(_facilityProp.FullColumnName(nameof(Facility.Code)), hotel.HotelId)
            .Select(
                $"{_roomGroupAppDateProp.FullColumnName(nameof(RoomGroupAppDate.RoomGroupId))} as {nameof(RoomGroupAppDateDto.RoomGroupId)}",
                $"{_roomGroupAppDateProp.FullColumnName(nameof(RoomGroupAppDate.AppDateId))} as {nameof(RoomGroupAppDateDto.AppDateId)}",
                $"{_roomGroupAppDateProp.FullColumnName(nameof(RoomGroupAppDate.IsNotSelled))} as {nameof(RoomGroupAppDateDto.IsNotSelled)}",
                $"{_roomGroupAppDateProp.FullColumnName(nameof(RoomGroupAppDate.SellNumber))} as {nameof(RoomGroupAppDateDto.SellNumber)}",
                $"{_roomGroupProp.FullColumnName(nameof(RoomGroup.BaseNumber))} as {nameof(RoomGroupAppDateDto.BaseNumber)}"
            );

        var tempResults = await queryFactory.FromQuery(query).GetAsync<RoomGroupAppDateDto>(cancellationToken: cancellationToken);

        var resPlanQuery = new Query(_reservationPlanRoomGroupAppDateProp.TableName)
            .Join(
                _reservationProp.TableName,
                _reservationProp.FullColumnName(nameof(ReservationEntity.Id)),
                _reservationPlanRoomGroupAppDateProp.FullColumnName(nameof(ReservationPlanRoomGroupAppDate.ReservationId))
            )
            .Join(
                _roomGroupProp.TableName,
                _roomGroupProp.FullColumnName(nameof(RoomGroup.Id)),
                _reservationPlanRoomGroupAppDateProp.FullColumnName(nameof(ReservationPlanRoomGroupAppDate.RoomGroupId))
            )
            .Where(_roomGroupProp.FullColumnName(nameof(RoomGroup.GroupName)), hotel.RoomId)
            .WhereIn(
                _reservationPlanRoomGroupAppDateProp.FullColumnName(nameof(ReservationPlanRoomGroupAppDate.BookingDateId)),
                hotel.Dates
            )
            .Select(
                $"{_reservationPlanRoomGroupAppDateProp.FullColumnName(nameof(ReservationPlanRoomGroupAppDate.RoomGroupId))} as {nameof(ReservationPlanRoomGroupAppDateDto.RoomGroupId)}",
                $"{_reservationPlanRoomGroupAppDateProp.FullColumnName(nameof(ReservationPlanRoomGroupAppDate.BookingDateId))} as {nameof(ReservationPlanRoomGroupAppDateDto.BookingDateId)}",
                $"{_reservationProp.FullColumnName(nameof(ReservationEntity.ReservationState))} as {nameof(ReservationPlanRoomGroupAppDateDto.ReservationState)}"
            );

        var resRawList = await queryFactory.FromQuery(resPlanQuery)
            .GetAsync<ReservationPlanRoomGroupAppDateDto>(cancellationToken: cancellationToken);

        var reservationDict = resRawList
            .GroupBy(x => (x.RoomGroupId, x.BookingDateId))
            .ToDictionary(g => g.Key, g => g.ToList());

        var result = tempResults.Select(
                app =>
                {
                    var key = (app.RoomGroupId, app.AppDateId);
                    var reservations = reservationDict.TryGetValue(key, out var list) ? list : [];

                    return new RoomGroupAppDate
                    {
                        RoomGroupId = app.RoomGroupId,
                        AppDateId = app.AppDateId,
                        IsNotSelled = app.IsNotSelled,
                        SellNumber = app.SellNumber,
                        RoomGroup = new RoomGroup
                        {
                            BaseNumber = app.BaseNumber,
                            ReservationPlanRoomGroupAppDates =
                            [
                                .. reservations.Select(
                                    r => new ReservationPlanRoomGroupAppDate
                                    {
                                        BookingDateId = r.BookingDateId,
                                        RoomGroupId = r.RoomGroupId,
                                        Reservation = new ReservationEntity { ReservationState = r.ReservationState }
                                    }
                                )
                            ]
                        }
                    };
                }
            )
            .ToList();

        return result;
    }

    public async Task<RoomGroup?> GetRoomGroupByGroupNameAndFacility(
        string? groupName,
        string? facilityCode,
        CancellationToken cancellationToken = default
    )
    {
        var query = new Query(_roomGroupProp.TableName)
            .Join(
                _facilityRoomGroupProp.TableName,
                _facilityRoomGroupProp.FullColumnName(nameof(FacilityRoomGroup.RoomGroupId)),
                _roomGroupProp.FullColumnName(nameof(RoomGroup.Id))
            )
            .Join(
                _facilityProp.TableName,
                _facilityProp.FullColumnName(nameof(Facility.Id)),
                _facilityRoomGroupProp.FullColumnName(nameof(FacilityRoomGroup.FacilityId))
            )
            .Where(_roomGroupProp.FullColumnName(nameof(RoomGroup.GroupName)), groupName)
            .Where(_facilityProp.FullColumnName(nameof(Facility.Code)), facilityCode)
            .Select(
                $"{_roomGroupProp.FullColumnName(nameof(RoomGroup.Id))} as {nameof(RoomGroupFacilityDto.Id)}",
                $"{_roomGroupProp.FullColumnName(nameof(RoomGroup.GroupName))} as {nameof(RoomGroupFacilityDto.GroupName)}",
                $"{_roomGroupProp.FullColumnName(nameof(RoomGroup.BaseNumber))} as {nameof(RoomGroupFacilityDto.BaseNumber)}"
            );

        var roomGroup = await queryFactory.FromQuery(query).FirstOrDefaultAsync<RoomGroupFacilityDto>(cancellationToken: cancellationToken);

        return roomGroup is null
            ? null
            : new RoomGroup
            {
                Id = roomGroup.Id,
                GroupName = roomGroup.GroupName,
                BaseNumber = roomGroup.BaseNumber
            };
    }

    public async Task BulkUpsertRoomGroupAppDateAsync(
        List<RoomGroupAppDate> roomGroupAppDates
    )
    {
        if (roomGroupAppDates.Count == 0)
        {
            return;
        }

        var columns = new[]
        {
            _roomGroupAppDateProp.ColumnName(nameof(RoomGroupAppDate.RoomGroupId)),
            _roomGroupAppDateProp.ColumnName(nameof(RoomGroupAppDate.AppDateId)),
            _roomGroupAppDateProp.ColumnName(nameof(RoomGroupAppDate.IsNotSelled)),
            _roomGroupAppDateProp.ColumnName(nameof(RoomGroupAppDate.SellNumber)),
            _roomGroupAppDateProp.ColumnName(nameof(RoomGroupAppDate.RecordMemo)),
            _roomGroupAppDateProp.ColumnName(nameof(RoomGroupAppDate.IsEnabled)),
            _roomGroupAppDateProp.ColumnName(nameof(RoomGroupAppDate.IsVisible)),
            _roomGroupAppDateProp.ColumnName(nameof(RoomGroupAppDate.IsDeleted)),
            _roomGroupAppDateProp.ColumnName(nameof(RoomGroupAppDate.DisplayOrder)),
            _roomGroupAppDateProp.ColumnName(nameof(RoomGroupAppDate.CreatedAt)),
            _roomGroupAppDateProp.ColumnName(nameof(RoomGroupAppDate.UpdatedAt))
        };

        var now = long.Parse(ConvertUtil.ToString(DateTime.UtcNow, "yyyyMMddHHmmssfff"));

        var data = roomGroupAppDates.Select(
                x => new object?[]
                {
                    x.RoomGroupId, x.AppDateId, x.IsNotSelled, x.SellNumber, x.RecordMemo, true, true, false, now, now, now
                }
            )
            .ToList();

        var insertQuery = new Query(_roomGroupAppDateProp.TableName)
            .AsInsert(columns, data);

        var compiled = queryFactory.Compiler.Compile(insertQuery);

        var updateFields = new[]
        {
            _roomGroupAppDateProp.ColumnName(nameof(RoomGroupAppDate.IsNotSelled)),
            _roomGroupAppDateProp.ColumnName(nameof(RoomGroupAppDate.SellNumber)),
            _roomGroupAppDateProp.ColumnName(nameof(RoomGroupAppDate.RecordMemo)),
            _roomGroupAppDateProp.ColumnName(nameof(RoomGroupAppDate.UpdatedAt))
        };

        var sql = $@"
            {compiled.Sql}
            ON CONFLICT (
                {_roomGroupAppDateProp.ColumnName(nameof(RoomGroupAppDate.RoomGroupId))},
                {_roomGroupAppDateProp.ColumnName(nameof(RoomGroupAppDate.AppDateId))}
            )
            DO UPDATE SET
            {string.Join(",\n    ", updateFields.Select(f => $"{f} = EXCLUDED.{f}"))};";

        await queryFactory.Connection.ExecuteAsync(sql, compiled.NamedBindings);
    }

    public async Task BulkUpsertAppDateAsync(
        List<long> appDateIds
    )
    {
        if (appDateIds.Count == 0)
        {
            return;
        }

        var now = long.Parse(ConvertUtil.ToString(DateTime.UtcNow, "yyyyMMddHHmmssfff"));

        var columns = new[]
        {
            _appDateProp.ColumnName(nameof(AppDate.Id)),
            _appDateProp.ColumnName(nameof(AppDate.Code)),
            _appDateProp.ColumnName(nameof(AppDate.DateTime)),
            _appDateProp.ColumnName(nameof(AppDate.IsEnabled)),
            _appDateProp.ColumnName(nameof(AppDate.IsVisible)),
            _appDateProp.ColumnName(nameof(AppDate.IsDeleted)),
            _appDateProp.ColumnName(nameof(AppDate.DisplayOrder)),
            _appDateProp.ColumnName(nameof(AppDate.CreatedAt)),
            _appDateProp.ColumnName(nameof(AppDate.UpdatedAt))
        };

        var data = appDateIds.Select(
                id => new object?[] { id, Guid.NewGuid().ToString("N"), AppDate.GetDateTime(id), true, true, false, now, now, now }
            )
            .ToList();

        var insertQuery = new Query(_appDateProp.TableName)
            .AsInsert(columns, data);

        var compiled = queryFactory.Compiler.Compile(insertQuery);

        var updateFields = new[] { _appDateProp.ColumnName(nameof(AppDate.UpdatedAt)) };

        var sql = $@"
        {compiled.Sql}
        ON CONFLICT (
            {_appDateProp.ColumnName(nameof(AppDate.Id))}
        )
        DO UPDATE SET
        {string.Join(",\n    ", updateFields.Select(f => $"{f} = EXCLUDED.{f}"))};";

        await queryFactory.Connection.ExecuteAsync(sql, compiled.NamedBindings);
    }

    public async Task<Dictionary<(string roomId, string hotelId), (long roomGroupId, long facilityId)>> GetRoomGroupFacilityIdMapAsync(
        Dictionary<string, string> roomHotelDict,
        CancellationToken cancellationToken = default
    )
    {
        var filtered = roomHotelDict
            .Where(kvp => !string.IsNullOrWhiteSpace(kvp.Key) && !string.IsNullOrWhiteSpace(kvp.Value))
            .DistinctBy(kvp => kvp.Key)
            .ToList();

        var roomIds = filtered.Select(x => x.Key).Distinct().ToArray();
        var hotelIds = filtered.Select(x => x.Value).Distinct().ToArray();

        var query = new Query(_roomGroupProp.TableName)
            .Join(
                _facilityRoomGroupProp.TableName,
                _facilityRoomGroupProp.FullColumnName(nameof(FacilityRoomGroup.RoomGroupId)),
                _roomGroupProp.FullColumnName(nameof(RoomGroup.Id))
            )
            .Join(
                _facilityProp.TableName,
                _facilityProp.FullColumnName(nameof(Facility.Id)),
                _facilityRoomGroupProp.FullColumnName(nameof(FacilityRoomGroup.FacilityId))
            )
            .WhereIn(_roomGroupProp.FullColumnName(nameof(RoomGroup.GroupName)), roomIds)
            .WhereIn(_facilityProp.FullColumnName(nameof(Facility.Code)), hotelIds)
            .Select(
                $"{_roomGroupProp.FullColumnName(nameof(RoomGroup.GroupName))} as {nameof(RoomGroupFacilityIdDto.RoomId)}",
                $"{_facilityProp.FullColumnName(nameof(Facility.Code))} as {nameof(RoomGroupFacilityIdDto.HotelId)}",
                $"{_roomGroupProp.FullColumnName(nameof(RoomGroup.Id))} as {nameof(RoomGroupFacilityIdDto.RoomGroupId)}",
                $"{_facilityProp.FullColumnName(nameof(Facility.Id))} as {nameof(RoomGroupFacilityIdDto.FacilityId)}"
            );

        var rows = await queryFactory
            .FromQuery(query)
            .GetAsync<RoomGroupFacilityIdDto>(cancellationToken: cancellationToken);

        return rows.ToDictionary(
            dto => (dto.RoomId, dto.HotelId),
            dto => (dto.RoomGroupId, dto.FacilityId)
        );
    }

    public IEnumerable<BookingRoomAppDateModel> GetAllRoomAppDates(
        BookingCreateRequest bookingCreateRequest,
        // BookingDataAvailableModel bookingDataAvailable,
        BookingPlanModel bookingPlanAvailable
    )
    {
        var bookingAdjustRequest = bookingCreateRequest.Adjust;

        // An exception is made if the number of nights exceeds the number of nights that can be accepted in the first place.
        if (!bookingPlanAvailable.CanNumberOfStayLimitMax(bookingAdjustRequest.NumberOfNights))
        {
            throw new ReservationOverNumberOfStayLimitException(bookingAdjustRequest.NumberOfNights);
        }

        // Setting the maximum number of rooms for sale in a plan
        var roomNumberDaySaleLimit = GetRoomNumberDaySaleLimit(bookingPlanAvailable);

        // Add price information to vacancy information
        // I also want to make changes, so I use the inner class
        var bookingRoomAppDates = CreateBookingRoomAppDates(
            bookingCreateRequest,
            bookingPlanAvailable,
            roomNumberDaySaleLimit
        );

        SetRepresentativeAmount(
            bookingCreateRequest,
            bookingPlanAvailable,
            bookingRoomAppDates
        );

        CheckConsecutiveNights(
            bookingCreateRequest,
            bookingPlanAvailable,
            bookingRoomAppDates
        );

        return bookingRoomAppDates;
    }

    private static int? GetRoomNumberDaySaleLimit(
        BookingPlanModel existingPlan
    )
    {
        return existingPlan is
            { UseDaySaleLimit: true, PlanDaySaleLimitType: PlanDaySaleLimitTypes.RoomGroup }
            ? existingPlan.RoomNumberDaySaleLimit
            : null;
    }

    private static List<BookingRoomAppDateModel> CreateBookingRoomAppDates(
        BookingCreateRequest bookingCreateRequest,
        BookingPlanModel bookingDataAvailable,
        int? roomNumberDaySaleLimit
    )
    {
        var bookingRoomAppDates = new List<BookingRoomAppDateModel>();

        var reservationPairsGroup = bookingDataAvailable.Reservations.ToList();
        var bookingRoomAvailable = bookingDataAvailable.RoomGroups.FirstOrDefault(
            x => x.Id == bookingCreateRequest.RoomGroupId
        )!;

        var appDatesOfRooms = bookingRoomAvailable
            .AppDates
            .Where(x => x.RoomGroupId == bookingCreateRequest.RoomGroupId)
            .OrderBy(x => x.AppDateId);

        foreach (var roomGroupAppDate in appDatesOfRooms)
        {
            var appDateId = roomGroupAppDate.AppDateId;

            // Current number of reservations for this day's plan
            var reservationPairs = reservationPairsGroup
                .Where(x => x.PlanId == bookingCreateRequest.PlanId)
                .Count(
                    x => x.BookingDateId == appDateId
                );

            // If the plan has a maximum number of rooms for sale, make sure it does not exceed the number of remaining rooms
            var reservedNumber = reservationPairsGroup
                .Where(x => x.BookingDateId == appDateId)
                .Count(
                    x => x.RoomGroupId == bookingCreateRequest.RoomGroupId
                );

            var reservedPlanNumber = reservationPairsGroup
                .Where(x => x.BookingDateId == appDateId && x.PlanId == bookingCreateRequest.PlanId)
                .Count(
                    x => x.RoomGroupId == bookingCreateRequest.RoomGroupId
                );

            var editReservedNumber = reservationPairsGroup
                .Where(x => x.ReservationId == bookingCreateRequest.Adjust.Id)
                .Where(x => x.BookingDateId == appDateId)
                .Count(
                    x => x.RoomGroupId == bookingCreateRequest.RoomGroupId
                );

            var remainNumber = roomGroupAppDate.SellNumber + editReservedNumber - reservedNumber;

            if (roomNumberDaySaleLimit != null)
            {
                remainNumber = roomNumberDaySaleLimit.Value - reservedPlanNumber;
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
                    Name = bookingRoomAvailable.Name?.GetValueByHeader(),
                    CapacityMax = bookingRoomAvailable.CapacityMax,
                    CapacityMin = bookingRoomAvailable.CapacityMin
                }
            };

            bookingRoomAppDates.Add(bookingRoomAppDate);
        }

        return bookingRoomAppDates;
    }

    private static void SetRepresentativeAmount(
        BookingCreateRequest bookingCreateRequest,
        BookingPlanModel bookingDataAvailable,
        List<BookingRoomAppDateModel> bookingRoomAppDates
    )
    {
        var bookingAdjustRequest = bookingCreateRequest.Adjust;
        var bookingRoomAvailable = bookingDataAvailable.RoomGroups.FirstOrDefault(
            x => x.Id == bookingCreateRequest.RoomGroupId
        )!;

        foreach (var bookingRoomAppDate in bookingRoomAppDates)
        {
            var appDateId = bookingRoomAppDate.AppDateId;
            var numberOfAdults = bookingAdjustRequest.GetNumberOfAdultsByDateId(appDateId) ?? 0;
            var prevDay = (AppDate.GetDateTime(appDateId) - bookingCreateRequest.BookingDate).Days;

            var planRoomGroupSiteAppDate = bookingRoomAvailable
                .PlanAppDates
                .Where(x => x.SiteId == bookingCreateRequest.SiteId)
                .Where(x => x.PlanId == bookingCreateRequest.PlanId)
                .Where(x => x.RoomGroupId == bookingCreateRequest.RoomGroupId)
                .SingleOrDefault(x => x.AppDateId == appDateId);

            var price = GetPrice(
                appDateId,
                numberOfAdults,
                bookingRoomAvailable.PriceData
            );

            var discountedPrice = price;

            if (planRoomGroupSiteAppDate != null)
            {
                discountedPrice = GetDiscountedPrice(
                    bookingCreateRequest,
                    planRoomGroupSiteAppDate,
                    bookingRoomAvailable.DiscountData,
                    numberOfAdults,
                    prevDay,
                    price
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
        BookingCreateRequest bookingCreateRequest,
        BookingPlanModel bookingDataAvailable,
        List<BookingRoomAppDateModel> bookingRoomAppDates
    )
    {
        var nightIndex = 0;

        foreach (var bookingRoomAppDate in bookingRoomAppDates)
        {
            var appDateId = bookingRoomAppDate.AppDateId;
            var bookingRoomAppDateByDateId = bookingRoomAppDates.SingleOrDefault(
                x => x.AppDateId == appDateId
            );

            var reservationServiceException = CheckCanNight(
                bookingCreateRequest,
                bookingDataAvailable,
                bookingRoomAppDateByDateId,
                appDateId
            );

            nightIndex++;
            if (reservationServiceException == null)
            {
                continue;
            }

            reservationServiceException.AppDateId = appDateId;
            reservationServiceException.RestIndex = nightIndex;
            bookingRoomAppDate.Exceptions.Add(reservationServiceException);
        }
    }

    private static int? GetPrice(
        long appDateId,
        int? adultPersonAgePersons,
        IEnumerable<BookingMetaPriceDataModel> appDatePriceData
    )
    {
        var price = appDatePriceData
            .Where(x => x.AppDateId == appDateId)
            .FirstOrDefault(x => x.InRange(adultPersonAgePersons))
            ?.Price;

        return price;
    }

    private static int? GetDiscountedPrice(
        BookingCreateRequest bookingCheckData,
        BookingMetaPlanAppDateModel planRoomGroupSiteAppDate,
        IEnumerable<BookingMetaDiscountDataModel> discountData,
        int numberOfAdultPersons,
        int prevDay,
        int? price
    )
    {
        var discountedPrice = price;

        if (!planRoomGroupSiteAppDate.UseAutoDiscount)
        {
            return discountedPrice;
        }

        var planRoomGroupSiteDiscountData = discountData
            .Where(x => x.PlanId == bookingCheckData.PlanId)
            .Where(x => x.RoomGroupId == bookingCheckData.RoomGroupId)
            .Where(x => x.SiteId == bookingCheckData.SiteId)
            .Where(x => x.InRange(numberOfAdultPersons))
            .FirstOrDefault(x => x.InPrevDay(prevDay));

        var discount = planRoomGroupSiteDiscountData?.GetDiscount(price);
        if (discount != null)
        {
            discountedPrice -= discount.Value;
        }

        return discountedPrice;
    }

    private static ReservationServiceException? CheckCanNight(
        BookingCreateRequest bookingCreateRequest,
        BookingPlanModel bookingDataAvailable,
        BookingRoomAppDateModel? bookingRoomAppDateByDateId,
        long appDateId
    )
    {
        var bookingAdjustRequest = bookingCreateRequest.Adjust;

        var isPriceValid = IsPriceValid(
            bookingCreateRequest,
            bookingDataAvailable,
            appDateId
        );
        if (!isPriceValid)
        {
            throw new ReservationServiceHasNoPaymentResultException();
        }

        if (bookingRoomAppDateByDateId == null)
        {
            throw new ReservationNoDataException();
        }

        var roomDataException = CheckRoomData(
            bookingCreateRequest.RoomGroupId,
            bookingAdjustRequest,
            bookingDataAvailable,
            bookingRoomAppDateByDateId
        );
        if (roomDataException is not null)
        {
            return roomDataException;
        }

        var planConstraintException = CheckPlanConstraints(
            bookingAdjustRequest,
            bookingDataAvailable,
            bookingRoomAppDateByDateId,
            appDateId
        );

        return planConstraintException;
    }

    private static bool IsPriceValid(
        BookingCreateRequest bookingCreateRequest,
        BookingPlanModel bookingDataAvailable,
        long appDateId
    )
    {
        var bookingAdjustRequest = bookingCreateRequest.Adjust;
        var bookingRoomAppDates = bookingDataAvailable.RoomGroups.FirstOrDefault(
            x => x.Id == bookingCreateRequest.RoomGroupId
        )!;

        var roomIndex = 0;
        var personAgeTypeIds = bookingRoomAppDates.PersonTypes
            .Where(x => x.IsRegardAdult)
            .Select(x => x.PersonAgeTypeId)
            .ToList();

        while (roomIndex < bookingCreateRequest.Adjust.NumberOfRooms)
        {
            var numberOfAdultPersons = bookingAdjustRequest.GetNumberOfAdultsByDateIdAndRoomIndex(
                appDateId,
                roomIndex,
                personAgeTypeIds
            );

            var isPriceValid = bookingRoomAppDates.PriceData
                .Where(x => x.SiteId == bookingCreateRequest.SiteId)
                .Where(x => x.PlanId == bookingCreateRequest.PlanId)
                .Where(x => x.RoomGroupId == bookingCreateRequest.RoomGroupId)
                .Where(x => x.AppDateId == appDateId)
                .Where(x => x.InRange(numberOfAdultPersons))
                .Where(x => x.Price != null)
                .Any(x => x.Price!.Value > 0);

            if (!isPriceValid)
            {
                return false;
            }

            roomIndex++;
        }

        return true;
    }

    private static ReservationServiceException? CheckRoomData(
        long roomId,
        BookingAdjustRequest bookingAdjustRequest,
        BookingPlanModel bookingDataAvailable,
        BookingRoomAppDateModel bookingRoomAppDateByDateId
    )
    {
        if (bookingRoomAppDateByDateId.RemainNumber is null or <= 0)
        {
            throw new ReservationNoRemainRoomNumberRestException();
        }

        if (bookingAdjustRequest.NumberOfRooms > bookingRoomAppDateByDateId.RemainNumber)
        {
            throw new ReservationOverRemainRoomNumberRestException();
        }

        var bookingRoomAvailable = bookingDataAvailable.RoomGroups.FirstOrDefault(
            x => x.Id == roomId
        )!;

        var personAgeTypeIds = bookingRoomAvailable.PersonTypes
            .Where(x => (x.PersonAgeTypeIsMain ?? false) || (x.PersonAgeTypeFoodBed ?? 0) >= FoodBeds.Bed)
            .Select(x => x.PersonAgeTypeId)
            .ToList();

        var maxNumberOfPeoples = bookingAdjustRequest.GetMaxNumberOfPeoplesByDateId(
            bookingRoomAppDateByDateId.AppDateId,
            personAgeTypeIds
        );
        if (!bookingRoomAppDateByDateId.RoomGroupInfo?.InRange(maxNumberOfPeoples) ?? true)
        {
            throw new ReservationOverCapacityNumberRestException();
        }

        if (bookingRoomAppDateByDateId.IsNotSelled)
        {
            throw new ReservationNotSelledRestException();
        }

        return null;
    }

    private static ReservationOverAcceptPersonNumberException? CheckPlanConstraints(
        BookingAdjustRequest bookingAdjustRequest,
        BookingPlanModel bookingPlan,
        BookingRoomAppDateModel bookingRoomAppDateByDateId,
        long appDateId
    )
    {
        var bookingSearchDate = DateTime.UtcNow.AddHours(
            DefaultValues.TimeZoneOffset
        );
        var bookingSearchDateId = AppDate.GetId(bookingSearchDate);

        if (!bookingPlan.CanDisplayDate(appDateId) || !bookingPlan.CanDisplayDate(bookingSearchDateId))
        {
            throw new ReservationOverDisplayDateException();
        }

        if (!bookingPlan.CanAcceptDate(appDateId) || !bookingPlan.CanAcceptDate(bookingSearchDateId))
        {
            throw new ReservationOverAcceptDateException();
        }

        if (!bookingPlan.CanDaySaleLimitReservedNumber(bookingRoomAppDateByDateId.ReservedNumber))
        {
            throw new ReservationOverPlanDaySaleLimitRoomNumberDataSaleLimitException();
        }

        if (!bookingPlan.CanDaySaleLimitReservedPairs(bookingRoomAppDateByDateId.ReservationPairs))
        {
            throw new ReservationOverPlanGroupNumberDaySaleLimitException();
        }

        var numberOfPersons = bookingAdjustRequest.GetNumberOfPeoplesByDateId(appDateId) ?? 0;

        return !bookingPlan.CanDaySaleLimitAllPersons(numberOfPersons)
            ? new ReservationOverAcceptPersonNumberException()
            : null;
    }

    public async Task<(SetRoomModel, List<RoomGroupAppDate>, List<RoomGroupAppDate>)>
        UpdateSetRoomsAsync(
            SetRoomModel roomGroupModel,
            CancellationToken cancellationToken = default
        )
    {
        var updateDataList = new List<RoomGroupAppDate>();
        var addDataList = new List<RoomGroupAppDate>();

        foreach (var hotel in roomGroupModel.Hotels)
        {
            var (updatedRoomGroupModel, updateData, addData) = await UpdateSetRoomsAppDateAsync(
                roomGroupModel,
                hotel,
                cancellationToken
            );

            roomGroupModel = updatedRoomGroupModel;

            updateDataList.AddRange(updateData);
            addDataList.AddRange(addData);
        }

        return (roomGroupModel, updateDataList, addDataList);
    }

    private async Task<(SetRoomModel, List<RoomGroupAppDate> roomGroupAppDatesToUpdate, List<RoomGroupAppDate> roomGroupAppDatesToAdd)>
        UpdateSetRoomsAppDateAsync(
            SetRoomModel roomGroupModel,
            SetRoomModel.Hotel hotel,
            CancellationToken cancellationToken = default
        )
    {
        var updateList = new List<RoomGroupAppDate>();
        var addList = new List<RoomGroupAppDate>();

        var appDatesOfRoom = await GetAppDatesOfRoomAsync(hotel, cancellationToken);

        var room = await GetRoomGroupByGroupNameAndFacility(
            hotel.RoomId,
            hotel.HotelId,
            cancellationToken
        );

        if (room == null)
        {
            roomGroupModel.CanUpdate = false;
            hotel.FailureReason = FailureReason.InvalidParameterOutOfRangeRoomId;

            return (roomGroupModel, updateList, addList);
        }

        var baseRoomNumber = appDatesOfRoom.FirstOrDefault()?.RoomGroup?.BaseNumber ?? room.BaseNumber;
        var baseNumber = GetRoomLimit(baseRoomNumber, MaxRoomLimit);

        foreach (var appDate in hotel.Dates)
        {
            var appDateOfRoom = appDatesOfRoom.Find(x => x.AppDateId == appDate);

            if (appDateOfRoom == null)
            {
                var (handled, roomGroupModelCanUpdate, failureReasonHotel) =
                    TryHandleAdjustmentConstraints(roomGroupModel, hotel, baseNumber);

                if (handled)
                {
                    roomGroupModel.CanUpdate = roomGroupModelCanUpdate;
                    hotel.FailureReason = failureReasonHotel;
                    continue;
                }

                appDatesOfRoom.ForEach(x => x.RoomGroup = null);
                appDateOfRoom = CreateRoomGroupAppDate(hotel, room, appDate);
                addList.Add(appDateOfRoom);

                continue;
            }

            if (!ProcessAppDate(roomGroupModel, hotel, appDateOfRoom, baseNumber))
            {
                continue;
            }

            updateList.Add(appDateOfRoom);
        }

        return (roomGroupModel, updateList, addList);
    }

    private static (bool handled, bool roomGroupModelCanUpdate, FailureReason failureReasonHotel) TryHandleAdjustmentConstraints(
        SetRoomModel roomGroupModel,
        SetRoomModel.Hotel hotel,
        int baseNumber
    )
    {
        if (CheckHotelRelativeDown(hotel))
        {
            return (true, false, FailureReason.MinusSellNumber);
        }

        return CheckHotelOverBaseNumber(hotel, baseNumber)
            ? (true, false, FailureReason.OverBaseNumber)
            : (false, roomGroupModel.CanUpdate, hotel.FailureReason);
    }

    private static bool CheckHotelRelativeDown(
        SetRoomModel.Hotel hotel
    )
    {
        return hotel is { AdjustType: HotelModel.EnumAdjustType.RelativeDown, RoomCount: > 0 };
    }

    private static bool CheckHotelOverBaseNumber(
        SetRoomModel.Hotel hotel,
        int baseNumber
    )
    {
        return hotel.AdjustType is HotelModel.EnumAdjustType.RelativeUp or HotelModel.EnumAdjustType.Absolute
            && hotel.RoomCount > baseNumber;
    }

    private static RoomGroupAppDate CreateRoomGroupAppDate(
        SetRoomModel.Hotel hotel,
        RoomGroup room,
        long appDate
    )
    {
        return hotel.AdjustType switch
        {
            HotelModel.EnumAdjustType.RelativeUp or HotelModel.EnumAdjustType.Absolute => new RoomGroupAppDate
            {
                AppDateId = appDate,
                SellNumber = (int)hotel.RoomCount,
                IsNotSelled = false,
                RoomGroupId = room.Id
            },
            HotelModel.EnumAdjustType.RelativeDown => new RoomGroupAppDate
            {
                AppDateId = appDate,
                SellNumber = 0,
                IsNotSelled = false,
                RoomGroupId = room.Id
            },
            HotelModel.EnumAdjustType.Sale => new RoomGroupAppDate
            {
                AppDateId = appDate,
                SellNumber = 0,
                IsNotSelled = false,
                RoomGroupId = room.Id
            },
            HotelModel.EnumAdjustType.Stop => new RoomGroupAppDate
            {
                AppDateId = appDate,
                SellNumber = 0,
                IsNotSelled = true,
                RoomGroupId = room.Id
            },
            _ => throw new InvalidOperationException("Unsupported AdjustType")
        };
    }

    private static bool ProcessAppDate(
        SetRoomModel roomGroupModel,
        SetRoomModel.Hotel hotel,
        RoomGroupAppDate appDateOfRoom,
        int baseNumber
    )
    {
        var reservedNumber = appDateOfRoom.RoomGroup?.ReservationPlanRoomGroupAppDates?
                .Where(
                    x => x.Reservation!.ReservationState is ReservationStatus.Reserved
                        or ReservationStatus.Modified
                        or ReservationStatus.Confirmed
                )
                .Count(x => x.BookingDateId == appDateOfRoom.AppDateId)
            ?? 0;

        AdjustSellNumber(hotel, appDateOfRoom, reservedNumber);
        appDateOfRoom.RoomGroup = null;

        if (appDateOfRoom.SellNumber < 0 || appDateOfRoom.SellNumber < reservedNumber)
        {
            roomGroupModel.CanUpdate = false;
            hotel.FailureReason = FailureReason.MinusSellNumber;
            return false;
        }

        if (!(appDateOfRoom.SellNumber > baseNumber))
        {
            return true;
        }

        roomGroupModel.CanUpdate = false;
        hotel.FailureReason = FailureReason.OverBaseNumber;

        return false;
    }

    private static void AdjustSellNumber(
        SetRoomModel.Hotel hotel,
        RoomGroupAppDate appDateOfRoom,
        int reservedNumber
    )
    {
        switch (hotel.AdjustType)
        {
            case HotelModel.EnumAdjustType.RelativeDown:
                appDateOfRoom.SellNumber -= (int)hotel.RoomCount;
                break;
            case HotelModel.EnumAdjustType.RelativeUp:
                appDateOfRoom.SellNumber += (int)hotel.RoomCount;
                break;
            case HotelModel.EnumAdjustType.Absolute:
                appDateOfRoom.SellNumber = (int)hotel.RoomCount + reservedNumber;
                break;
            case HotelModel.EnumAdjustType.Stop:
                appDateOfRoom.IsNotSelled = true;
                break;
            case HotelModel.EnumAdjustType.Sale:
                appDateOfRoom.IsNotSelled = false;
                break;
        }
    }

    public List<Reason> GetAllReasons(
        SetRoomModel setRoomModel,
        List<HotelModel> hotels
    )
    {
        var reasons = new List<Reason>();

        foreach (var message in Enum.GetValues<FailureReason>())
        {
            // NOTE: NoneはXMLとして出力しない
            if (message == FailureReason.None)
            {
                continue;
            }

            var modelHotels = setRoomModel.Hotels
                .Where(x => x.FailureReason == message)
                .Select(x => hotels.Find(t => t.Key == x.Key)!)
                .ToList();
            if (modelHotels is not { Count: > 0 })
            {
                continue;
            }

            var reason = new Reason
            {
                Message = message,
                Hotels =
                [
                    .. modelHotels
                ]
            };

            reasons.Add(reason);
        }

        return reasons;
    }

    private static int GetRoomLimit(
        int? baseNumber,
        int? maxRoomLimit
    )
    {
        if (maxRoomLimit.HasValue && maxRoomLimit.Value > 0)
        {
            return maxRoomLimit.Value;
        }

        return baseNumber ?? 0;
    }
}
