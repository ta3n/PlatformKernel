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

public class RoomGroupInventoryGetAllQueryHandler(
    IMapper mapper,
    IUnitOfWork unitOfWork,
    ISecurityContextAccessor securityContextAccessor,
    QueryFactory queryFactory
) : QueryPageBaseHandler<RoomGroupInventoryGetAllQuery, RoomGroupAppDateDataResponse>(mapper)
{
    private readonly EntityProperty _roomGroupAppDateProp = unitOfWork.GetEntityProperty<RoomGroupAppDate>();
    private readonly EntityProperty _roomGroupProp = unitOfWork.GetEntityProperty<RoomGroupEntity>();
    private readonly EntityProperty _facilityRoomGroupProp = unitOfWork.GetEntityProperty<FacilityRoomGroup>();
    private readonly EntityProperty _reservationPlanRoomGroupAppDateProp = unitOfWork.GetEntityProperty<ReservationPlanRoomGroupAppDate>();
    private readonly EntityProperty _reservationProp = unitOfWork.GetEntityProperty<ReservationEntity>();

    protected override async Task<(IHeaderDictionary, IEnumerable<RoomGroupAppDateDataResponse>)> HandleAsync(
        RoomGroupInventoryGetAllQuery request,
        CancellationToken cancellationToken
    )
    {
        var facilityId = securityContextAccessor.FacilityKey;

        var query = BuildSqlQuery(request, facilityId);

        var result = await queryFactory
            .FromQuery(query)
            .GetAsync<RoomGroupAppDateDataResponse>(cancellationToken: cancellationToken);

        var data = result.ToList();
        foreach (var value in data)
        {
            value.RoomGroupName = DeserializeMultilingualText(value.RoomGroupName) ?? string.Empty;
            value.RemainNumber = value.SellNumber - value.ReservedNumber;
        }

        return (new HeaderDictionary(), data);
    }

    private Query BuildSqlQuery(
        RoomGroupInventoryGetAllQuery request,
        long facilityId
    )
    {
        var query = new Query(_roomGroupAppDateProp.TableName);

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
                _roomGroupProp.TableName,
                _roomGroupProp.FullColumnName(nameof(RoomGroupEntity.Id)),
                _roomGroupAppDateProp.FullColumnName(nameof(RoomGroupAppDate.RoomGroupId))
            )
            .Join(
                _facilityRoomGroupProp.TableName,
                _facilityRoomGroupProp.FullColumnName(nameof(FacilityRoomGroup.RoomGroupId)),
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
        RoomGroupInventoryGetAllQuery request,
        long facilityId
    )
    {
        return query
            .Where(_facilityRoomGroupProp.FullColumnName(nameof(FacilityRoomGroup.FacilityId)), facilityId)
            .WhereBetween(
                _roomGroupAppDateProp.FullColumnName(nameof(RoomGroupAppDate.AppDateId)),
                request.StartAppDate,
                request.EndAppDate
            );
    }

    private Query SelectQuery(
        Query query
    )
    {
        return query
            .Select(
                $"{_roomGroupAppDateProp.FullColumnName(nameof(RoomGroupAppDate.AppDateId))} AS {nameof(RoomGroupAppDateDataResponse.AppDateId)}",
                $"{_roomGroupAppDateProp.FullColumnName(nameof(RoomGroupAppDate.RoomGroupId))} AS {nameof(RoomGroupAppDateDataResponse.RoomGroupId)}",
                $"{_roomGroupProp.FullColumnName(nameof(RoomGroupEntity.Name))} AS {nameof(RoomGroupAppDateDataResponse.RoomGroupName)}",
                $"{_roomGroupProp.FullColumnName(nameof(RoomGroupEntity.BaseNumber))} AS {nameof(RoomGroupAppDateDataResponse.BaseNumber)}",
                $"{_roomGroupProp.FullColumnName(nameof(RoomGroupEntity.GroupName))} AS {nameof(RoomGroupAppDateDataResponse.GroupName)}",
                $"{_roomGroupAppDateProp.FullColumnName(nameof(RoomGroupAppDate.SellNumber))} AS {nameof(RoomGroupAppDateDataResponse.SellNumber)}",
                $"{_roomGroupAppDateProp.FullColumnName(nameof(RoomGroupAppDate.IsNotSelled))} AS {nameof(RoomGroupAppDateDataResponse.IsNotSold)}"
            )
            .SelectRaw(
                @$"
                COUNT(CASE
                    WHEN {_reservationProp.FullColumnName(nameof(ReservationEntity.ReservationState))} IN ({(int)ReservationStatus.Confirmed}, {(int)ReservationStatus.Reserved}, {(int)ReservationStatus.Modified})
                    THEN 1 END
                ) as {nameof(RoomGroupAppDateResponse.ReservedNumber)}
            "
            )
            .GroupBy(
                _roomGroupAppDateProp.FullColumnName(nameof(RoomGroupAppDate.AppDateId)),
                _roomGroupAppDateProp.FullColumnName(nameof(RoomGroupAppDate.RoomGroupId)),
                _roomGroupProp.FullColumnName(nameof(RoomGroupEntity.Name)),
                _roomGroupProp.FullColumnName(nameof(RoomGroupEntity.BaseNumber)),
                _roomGroupProp.FullColumnName(nameof(RoomGroupEntity.GroupName)),
                _roomGroupAppDateProp.FullColumnName(nameof(RoomGroupAppDate.SellNumber)),
                _roomGroupAppDateProp.FullColumnName(nameof(RoomGroupAppDate.IsNotSelled))
            );
    }

    private static Query SortQuery(
        Query query
    )
    {
        return query.OrderBy(nameof(RoomGroupAppDateResponse.AppDateId));
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
