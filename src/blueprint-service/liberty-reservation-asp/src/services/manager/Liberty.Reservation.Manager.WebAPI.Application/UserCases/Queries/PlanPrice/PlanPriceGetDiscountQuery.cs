using Liberty.ApplicationShared.Cqrs.BaseQuery;

namespace Liberty.Reservation.Manager.WebAPI.Application.UserCases.Queries.PlanPrice;

public record PlanPriceGetDiscountQuery(
    long PlanId,
    long RomTypeId,
    long SiteId
) : IQuerySingleBase<PlanRoomDetailDiscountResponse>;
