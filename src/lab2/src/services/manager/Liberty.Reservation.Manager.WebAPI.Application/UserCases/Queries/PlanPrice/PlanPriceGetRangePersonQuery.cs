using Liberty.ApplicationShared.Cqrs.BaseQuery;

namespace Liberty.Reservation.Manager.WebAPI.Application.UserCases.Queries.PlanPrice;

public record PlanPriceGetRangePersonQuery(
    long PlanId,
    long RoomTypeId,
    long SiteId
) : IQuerySingleBase<PlanRoomSiteRangePersonResponse>;
