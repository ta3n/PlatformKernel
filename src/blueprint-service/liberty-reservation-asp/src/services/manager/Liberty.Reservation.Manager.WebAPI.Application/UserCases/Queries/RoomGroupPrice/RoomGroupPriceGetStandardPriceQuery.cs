using Liberty.ApplicationShared.Cqrs.BaseQuery;

namespace Liberty.Reservation.Manager.WebAPI.Application.UserCases.Queries.RoomGroupPrice;

public record RoomGroupPriceGetStandardPriceQuery(
    long RoomTypeId,
    long SiteId
) : IQuerySingleBase<PlanRoomDetailStandardPriceResponse>;
