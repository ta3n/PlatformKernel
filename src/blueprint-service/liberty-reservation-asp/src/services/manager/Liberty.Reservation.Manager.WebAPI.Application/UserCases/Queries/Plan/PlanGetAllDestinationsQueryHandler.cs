using Liberty.Pagination.Extensions;
using Liberty.Reservation.Application.Cqrs.BaseQueries;
using Liberty.Reservation.Manager.Application.Auth;

namespace Liberty.Reservation.Manager.WebAPI.Application.UserCases.Queries.Plan;

public class PlanGetAllDestinationsQueryHandler(
    IMapper mapper,
    ISecurityContextAccessor securityContextAccessor,
    ISiteRepository siteRepository
) : QueryPageBaseHandler<PlanGetAllDestinationsQuery, SiteResponse>(mapper)
{
    protected override async Task<(IHeaderDictionary, IEnumerable<SiteResponse>)> HandleAsync(
        PlanGetAllDestinationsQuery request,
        CancellationToken cancellationToken
    )
    {
        var facilityId = securityContextAccessor.FacilityKey;

        var queryable = siteRepository.GetQueryableWithAsNoTracking()
            .Include(x => x.FacilitySites)
            .Include(x => x.PlanSites!)
            .Where(
                x => x.PlanSites!.Any(t => t.PlanId == request.Id)
                    && x.FacilitySites!.Any(t => !t.IsDeleted && t.FacilityId == facilityId)
            );

        var page = await queryable.UsePageableAsDtoAsync<Site, SiteResponse>(
            request.Pageable,
            Mapper,
            false,
            cancellationToken
        );

        var headers = page.GeneratePaginationHttpHeaders();
        var data = page.Content;

        return (headers, data);
    }
}
