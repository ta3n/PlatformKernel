using Liberty.ApplicationShared.Cqrs.BaseQuery;

namespace Liberty.Reservation.Manager.WebAPI.Application.UserCases.Queries.PlanPrice;

public record PlanPriceGetMinimumPriceQuery(
    long PlanId,
    long RoomId,
    long SiteId
) : IQuerySingleBase<PlanRoomSiteMinimumPriceResponse>;
