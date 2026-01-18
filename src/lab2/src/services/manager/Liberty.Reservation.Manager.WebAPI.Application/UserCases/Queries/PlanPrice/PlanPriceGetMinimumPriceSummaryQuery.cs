using Liberty.ApplicationShared.Cqrs.BaseQuery;

namespace Liberty.Reservation.Manager.WebAPI.Application.UserCases.Queries.PlanPrice;

public record PlanPriceGetMinimumPriceSummaryQuery(
    long PlanId,
    long RoomId,
    long SiteId
) : IQuerySingleBase<PlanRoomSiteMinimumPriceSummaryResponse>;
