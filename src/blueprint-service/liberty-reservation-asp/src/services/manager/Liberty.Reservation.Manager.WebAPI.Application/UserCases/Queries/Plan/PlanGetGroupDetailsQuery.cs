using Liberty.ApplicationShared.Cqrs.BaseQuery;
using Liberty.Reservation.Manager.WebAPI.Application.Models;

namespace Liberty.Reservation.Manager.WebAPI.Application.UserCases.Queries.Plan;

public record PlanGetGroupDetailsQuery(
    long Id,
    GroupOfPlan Group
) : IQuerySingleBase<object>;
