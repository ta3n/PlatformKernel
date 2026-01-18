using Liberty.ApplicationShared.Cqrs.BaseQuery;

namespace Liberty.Reservation.Manager.WebAPI.Application.UserCases.Queries.CancellationPolicy;

public record CancellationPolicyGetQuery(
    long Id
) : IQuerySingleBase<CancellationPolicyDetailResponse>;
