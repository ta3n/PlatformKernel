using Liberty.ApplicationShared.Cqrs.BaseQuery;

namespace Liberty.Reservation.Manager.WebAPI.Application.UserCases.Queries.PlanPrice;

public record PlanPriceGetChildrenPriceQuery(
    long PlanId,
    long RomTypeId,
    long SiteId
) : IQuerySingleBase<PlanRoomDetailChildrenPriceResponse>;
