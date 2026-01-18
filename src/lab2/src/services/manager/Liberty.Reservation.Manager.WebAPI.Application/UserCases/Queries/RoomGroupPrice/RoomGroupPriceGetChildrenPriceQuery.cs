using Liberty.ApplicationShared.Cqrs.BaseQuery;

namespace Liberty.Reservation.Manager.WebAPI.Application.UserCases.Queries.RoomGroupPrice;

public record RoomGroupPriceGetChildrenPriceQuery(
    long RoomTypeId,
    long SiteId
) : IQuerySingleBase<PlanRoomDetailChildrenPriceResponse>;
