using Liberty.Pagination.Extensions;
using Liberty.Reservation.Application.Cqrs.BaseQueries;
using Liberty.Reservation.Manager.Application.Auth;

namespace Liberty.Reservation.Manager.WebAPI.Application.UserCases.Queries.Plan;

public class PlanGetAllQueryHandler(
    IMapper mapper,
    ISecurityContextAccessor securityContextAccessor,
    IPlanRepository planRepository
) : QueryPageBaseHandler<PlanGetAllQuery, PlanResponse>(mapper)
{
    protected override async Task<(IHeaderDictionary, IEnumerable<PlanResponse>)> HandleAsync(
        PlanGetAllQuery request,
        CancellationToken cancellationToken
    )
    {
        var facilityId = securityContextAccessor.FacilityKey;

        var queryable = planRepository
            .GetQueryableWithAsNoTracking()
            .Where(
                x => x.FacilityPlans!.Any(
                    t => t.FacilityId == facilityId
                )
            )
            .OrderByDescending(x => x.DisplayOrder);

        var page = await queryable.UsePageableAsDtoAsync
            <Reservation.Application.Contexts.DataContexts.Entities.Data.Plan, PlanResponse>(
                request.Pageable,
                Mapper,
                cancellationToken: cancellationToken
            );

        var data = page.Content;
        var headers = page.GeneratePaginationHttpHeaders();

        return (headers, data);
    }
}
