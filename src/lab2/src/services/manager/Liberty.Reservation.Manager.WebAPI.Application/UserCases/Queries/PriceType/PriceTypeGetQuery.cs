using Liberty.ApplicationShared.Cqrs.BaseQuery;

namespace Liberty.Reservation.Manager.WebAPI.Application.UserCases.Queries.PriceType;

public record PriceTypeGetQuery(
    long Id
) : IQuerySingleBase<PriceTypeResponse>;
