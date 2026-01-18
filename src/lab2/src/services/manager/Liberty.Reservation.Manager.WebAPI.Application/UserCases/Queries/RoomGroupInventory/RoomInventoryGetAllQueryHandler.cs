using Liberty.Entity.ValueObjects;
using Liberty.Reservation.Application.Cqrs.BaseQueries;
using Liberty.Reservation.Application.Utils;
using Liberty.Reservation.Manager.Application.Auth;
using Newtonsoft.Json;
using SqlKata;
using SqlKata.Execution;
using ReservationEntity = Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data.Reservation;
using RoomGroupEntity = Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data.RoomGroup;

namespace Liberty.Reservation.Manager.WebAPI.Application.UserCases.Queries.RoomGroupInventory;

public class RoomInventoryGetAllQueryHandler(
    IMapper mapper,
    IUnitOfWork unitOfWork,
    ISecurityContextAccessor securityContextAccessor,
    QueryFactory queryFactory
) : QueryPageBaseHandler<RoomInventoryGetAllQuery, RoomGroupWithAppDatesResponse>(mapper)
{
    private readonly EntityProperty _roomGroupAppDateProp = unitOfWork.GetEntityProperty<RoomGroupAppDate>();
    private readonly EntityProperty _roomGroupProp = unitOfWork.GetEntityProperty<RoomGroupEntity>();
    private readonly EntityProperty _facilityRoomGroupProp = unitOfWork.GetEntityProperty<FacilityRoomGroup>();
    private readonly EntityProperty _reservationPlanRoomGroupAppDateProp = unitOfWork.GetEntityProperty<ReservationPlanRoomGroupAppDate>();
    private readonly EntityProperty _reservationProp = unitOfWork.GetEntityProperty<ReservationEntity>();

    protected override async Task<(IHeaderDictionary, IEnumerable<RoomGroupWithAppDatesResponse>)> HandleAsync(
        RoomInventoryGetAllQuery request,
        CancellationToken cancellationToken
    )
    {
        var facilityId = securityContextAccessor.FacilityKey;

        var totalCount = await queryFactory
            .Query(_roomGroupProp.TableName)
            .Join(
                _facilityRoomGroupProp.TableName,
                _facilityRoomGroupProp.FullColumnName(nameof(FacilityRoomGroup.RoomGroupId)),
                _roomGroupProp.FullColumnName(nameof(RoomGroupEntity.Id))
            )
            .Where(_facilityRoomGroupProp.FullColumnName(nameof(FacilityRoomGroup.FacilityId)), facilityId)
            .CountAsync<int>(cancellationToken: cancellationToken);

        var roomGroupQuery = queryFactory
            .Query(_roomGroupProp.TableName)
            .Join(
                _facilityRoomGroupProp.TableName,
                _facilityRoomGroupProp.FullColumnName(nameof(FacilityRoomGroup.RoomGroupId)),
                _roomGroupProp.FullColumnName(nameof(RoomGroupEntity.Id))
            )
            .Where(_facilityRoomGroupProp.FullColumnName(nameof(FacilityRoomGroup.FacilityId)), facilityId)
            .Select(
                $"{_roomGroupProp.FullColumnName(nameof(RoomGroupEntity.Id))} AS {nameof(RoomGroupWithAppDatesResponse.RoomGroupId)}",
                $"{_roomGroupProp.FullColumnName(nameof(RoomGroupEntity.Name))} AS {nameof(RoomGroupWithAppDatesResponse.RoomGroupName)}",
                $"{_roomGroupProp.FullColumnName(nameof(RoomGroupEntity.BaseNumber))} AS {nameof(RoomGroupWithAppDatesResponse.BaseNumber)}",
                $"{_roomGroupProp.FullColumnName(nameof(RoomGroupEntity.GroupName))} AS {nameof(RoomGroupWithAppDatesResponse.GroupName)}"
            )
            .OrderByDesc(_roomGroupProp.FullColumnName(nameof(RoomGroupEntity.DisplayOrder)))
            .OrderBy(_roomGroupProp.FullColumnName(nameof(RoomGroupEntity.Id)));

        if (request.Pageable.IsPaged)
        {
            roomGroupQuery = roomGroupQuery.ForPage(request.Pageable.PageNumber, request.Pageable.PageSize);
        }

        var roomGroups = (await roomGroupQuery
                .GetAsync<RoomGroupWithAppDatesResponse>(cancellationToken: cancellationToken))
            .ToList();

        var appDateQuery = BuildSqlQuery(request, facilityId);
        var appDateResult = await queryFactory
            .FromQuery(appDateQuery)
            .GetAsync<RoomGroupAppDateDataResponse>(cancellationToken: cancellationToken);

        var appDateData = appDateResult.ToList();
        foreach (var value in appDateData)
        {
            value.RoomGroupName = DeserializeMultilingualText(value.RoomGroupName) ?? string.Empty;
            value.RemainNumber = value.SellNumber - value.ReservedNumber;
        }

        var appDateGrouped = appDateData
            .GroupBy(r => r.RoomGroupId)
            .ToDictionary(
                g => g.Key,
                g => g.Select(
                        a => new RoomGroupAppDateDetailResponse
                        {
                            AppDateId = a.AppDateId,
                            SellNumber = a.SellNumber,
                            IsNotSold = a.IsNotSold,
                            ReservedNumber = a.ReservedNumber,
                            RemainNumber = a.RemainNumber
                        }
                    )
                    .ToList()
            );

        var response = roomGroups.Select(
            rg => new RoomGroupWithAppDatesResponse
            {
                RoomGroupId = rg.RoomGroupId,
                RoomGroupName = DeserializeMultilingualText(rg.RoomGroupName) ?? string.Empty,
                GroupName = rg.GroupName,
                BaseNumber = rg.BaseNumber,
                AppDates = appDateGrouped.TryGetValue(rg.RoomGroupId, out var list)
                    ? list
                    : []
            }
        );
        var totalPages = request.Pageable.IsPaged && request.Pageable.PageSize > 0
            ? (int)Math.Ceiling(totalCount / (double)request.Pageable.PageSize)
            : 1;

        var pageDto = new PageResponse
        {
            TotalCount = totalCount,
            TotalPages = totalPages,
            Page = request.Pageable.IsPaged ? request.Pageable.PageNumber : 1,
            Size = request.Pageable.IsPaged ? request.Pageable.PageSize : totalCount,
            HasNext = request.Pageable.IsPaged && request.Pageable.PageNumber < totalPages,
            HasPrevious = request.Pageable.IsPaged && request.Pageable.PageNumber > 1,
            IsFirst = !request.Pageable.IsPaged || request.Pageable.PageNumber <= 1,
            IsLast = !request.Pageable.IsPaged || request.Pageable.PageNumber >= totalPages
        };

        var headers = new HeaderDictionary
        {
            { "X-Total-Count", totalCount.ToString() },
            { "X-Pagination", JsonConvert.SerializeObject(pageDto) }
        };

        return (headers, response);
    }

    private Query BuildSqlQuery(
        RoomInventoryGetAllQuery request,
        long facilityId
    )
    {
        var query = new Query(_roomGroupProp.TableName);

        query = JoinQuery(query);
        query = ConditionQuery(query, request, facilityId);
        query = SelectQuery(query);
        query = SortQuery(query);

        return query;
    }

    private Query JoinQuery(
        Query query
    )
    {
        return query
            .Join(
                _facilityRoomGroupProp.TableName,
                _facilityRoomGroupProp.FullColumnName(nameof(FacilityRoomGroup.RoomGroupId)),
                _roomGroupProp.FullColumnName(nameof(RoomGroupEntity.Id))
            )
            .LeftJoin(
                _roomGroupAppDateProp.TableName,
                _roomGroupAppDateProp.FullColumnName(nameof(RoomGroupAppDate.RoomGroupId)),
                _roomGroupProp.FullColumnName(nameof(RoomGroupEntity.Id))
            )
            .LeftJoin(
                _reservationPlanRoomGroupAppDateProp.TableName,
                j => j
                    .On(
                        _reservationPlanRoomGroupAppDateProp.FullColumnName(nameof(ReservationPlanRoomGroupAppDate.BookingDateId)),
                        _roomGroupAppDateProp.FullColumnName(nameof(RoomGroupAppDate.AppDateId))
                    )
                    .On(
                        _reservationPlanRoomGroupAppDateProp.FullColumnName(nameof(ReservationPlanRoomGroupAppDate.RoomGroupId)),
                        _roomGroupAppDateProp.FullColumnName(nameof(RoomGroupAppDate.RoomGroupId))
                    )
            )
            .LeftJoin(
                _reservationProp.TableName,
                _reservationProp.FullColumnName(nameof(ReservationEntity.Id)),
                _reservationPlanRoomGroupAppDateProp.FullColumnName(nameof(ReservationPlanRoomGroupAppDate.ReservationId))
            );
    }

    private Query ConditionQuery(
        Query query,
        RoomInventoryGetAllQuery request,
        long facilityId
    )
    {
        query = query.Where(_facilityRoomGroupProp.FullColumnName(nameof(FacilityRoomGroup.FacilityId)), facilityId);

        if (request.StartAppDate > 0 && request.EndAppDate > 0)
        {
            query = query.WhereBetween(
                _roomGroupAppDateProp.FullColumnName(nameof(RoomGroupAppDate.AppDateId)),
                request.StartAppDate,
                request.EndAppDate
            );
        }

        return query;
    }

    private Query SelectQuery(
        Query query
    )
    {
        return query
            .Select(
                $"{_roomGroupProp.FullColumnName(nameof(RoomGroupEntity.Id))} AS {nameof(RoomGroupAppDateDataResponse.RoomGroupId)}",
                $"{_roomGroupProp.FullColumnName(nameof(RoomGroupEntity.Name))} AS {nameof(RoomGroupAppDateDataResponse.RoomGroupName)}",
                $"{_roomGroupProp.FullColumnName(nameof(RoomGroupEntity.BaseNumber))} AS {nameof(RoomGroupAppDateDataResponse.BaseNumber)}",
                $"{_roomGroupProp.FullColumnName(nameof(RoomGroupEntity.GroupName))} AS {nameof(RoomGroupAppDateDataResponse.GroupName)}",
                $"{_roomGroupAppDateProp.FullColumnName(nameof(RoomGroupAppDate.AppDateId))} AS {nameof(RoomGroupAppDateDataResponse.AppDateId)}",
                $"{_roomGroupAppDateProp.FullColumnName(nameof(RoomGroupAppDate.SellNumber))} AS {nameof(RoomGroupAppDateDataResponse.SellNumber)}",
                $"{_roomGroupAppDateProp.FullColumnName(nameof(RoomGroupAppDate.IsNotSelled))} AS {nameof(RoomGroupAppDateDataResponse.IsNotSold)}"
            )
            .SelectRaw(
                @$"
                COUNT(CASE
                    WHEN {_reservationProp.FullColumnName(nameof(ReservationEntity.ReservationState))} IN ({(int)ReservationStatus.Confirmed}, {(int)ReservationStatus.Reserved}, {(int)ReservationStatus.Modified})
                    THEN 1 END
                ) as {nameof(RoomGroupAppDateDataResponse.ReservedNumber)}
            "
            )
            .GroupBy(
                _roomGroupProp.FullColumnName(nameof(RoomGroupEntity.Id)),
                _roomGroupProp.FullColumnName(nameof(RoomGroupEntity.Name)),
                _roomGroupProp.FullColumnName(nameof(RoomGroupEntity.BaseNumber)),
                _roomGroupProp.FullColumnName(nameof(RoomGroupEntity.GroupName)),
                _roomGroupAppDateProp.FullColumnName(nameof(RoomGroupAppDate.AppDateId)),
                _roomGroupAppDateProp.FullColumnName(nameof(RoomGroupAppDate.SellNumber)),
                _roomGroupAppDateProp.FullColumnName(nameof(RoomGroupAppDate.IsNotSelled))
            );
    }

    private static Query SortQuery(
        Query query
    )
    {
        return query.OrderBy(nameof(RoomGroupAppDateDataResponse.RoomGroupId))
            .OrderBy(nameof(RoomGroupAppDateDataResponse.AppDateId));
    }

    private static string? DeserializeMultilingualText(
        string? json
    )
    {
        return json != null
            ? JsonConvert.DeserializeObject<MultilingualText>(json)?.GetValueByHeader()
            : string.Empty;
    }
}
