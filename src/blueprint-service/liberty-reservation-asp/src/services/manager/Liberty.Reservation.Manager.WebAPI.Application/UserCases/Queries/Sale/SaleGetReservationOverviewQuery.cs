using Liberty.ApplicationShared.Cqrs.BaseQuery;

namespace Liberty.Reservation.Manager.WebAPI.Application.UserCases.Queries.Sale;

public record SaleGetReservationOverviewQuery(
    long StartAppDateId,
    long EndAppDateId
) : IQuerySingleBase<SaleOverviewResponse>;
