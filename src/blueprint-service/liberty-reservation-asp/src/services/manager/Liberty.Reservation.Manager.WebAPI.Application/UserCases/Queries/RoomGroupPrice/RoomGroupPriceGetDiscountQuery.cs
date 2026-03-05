using Liberty.ApplicationShared.Cqrs.BaseQuery;

namespace Liberty.Reservation.Manager.WebAPI.Application.UserCases.Queries.RoomGroupPrice;

public record RoomGroupPriceGetDiscountQuery(
    long RoomTypeId,
    long SiteId
) : IQuerySingleBase<PlanRoomDetailDiscountResponse>;
