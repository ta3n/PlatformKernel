using Liberty.ApplicationShared.Cqrs.BaseQuery;

namespace Liberty.Reservation.Manager.WebAPI.Application.UserCases.Queries.PlanPrice;

public record PlanPriceGetSaleQuery(
    long PlanId,
    long RomTypeId,
    long SiteId
) : IQuerySingleBase<PlanRoomDetailSaleResponse>;
