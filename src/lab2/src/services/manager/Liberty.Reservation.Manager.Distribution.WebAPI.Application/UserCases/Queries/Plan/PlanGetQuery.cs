using Liberty.ApplicationShared.Cqrs.BaseQuery;
using Liberty.Reservation.Manager.Distribution.WebAPI.Application.Models.Requests;
using Liberty.Reservation.Manager.Distribution.WebAPI.Application.Models.Responses;

namespace Liberty.Reservation.Manager.Distribution.WebAPI.Application.UserCases.Queries.Plan;

public record PlanGetQuery(
    GetPlanRequest Request
) : IQuerySingleBase<BaseDataResponse<GetPlanResponse>>;
