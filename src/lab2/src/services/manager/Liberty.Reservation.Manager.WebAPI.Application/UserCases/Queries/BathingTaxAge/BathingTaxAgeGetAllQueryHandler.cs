using Liberty.Reservation.Application.Cqrs.BaseQueries;
using Liberty.Reservation.Manager.Application.Auth;

namespace Liberty.Reservation.Manager.WebAPI.Application.UserCases.Queries.BathingTaxAge;

public class BathingTaxAgeGetAllQueryHandler(
    IMapper mapper,
    ISecurityContextAccessor securityContextAccessor,
    IFacilityRepository facilityRepository,
    IPersonAgeTypeService personAgeTypeService,
    IPlanService planService
) : QuerySingleBaseHandler<BathingTaxAgeGetAllQuery, BathingTaxAgeResponse>(mapper)
{
    protected override async Task<(IHeaderDictionary, BathingTaxAgeResponse)> HandleAsync(
        BathingTaxAgeGetAllQuery request,
        CancellationToken cancellationToken
    )
    {
        var facilityId = securityContextAccessor.FacilityKey;

        var queryable = facilityRepository
            .GetQueryableWithAsNoTracking()
            .Where(x => x.Id == facilityId)
            .OrderByDescending(x => x.DisplayOrder)
            .ProjectTo<BathingTaxAgeResponse>(Mapper.ConfigurationProvider);

        var data = await queryable.SingleOrDefaultAsync(
                cancellationToken
            )
            ?? throw new FacilityNotfoundException();

        data.PlanHasData = await planService.AnyAsync(cancellationToken);

        var personAgeTypes = await personAgeTypeService.GetAllPersonAgeTypes(
            request.Pageable,
            cancellationToken: cancellationToken
        );
        data.BathingTaxAges = Mapper.Map<List<BathingTaxAgeDetailsResponse>>(
            personAgeTypes.Content
                .OrderBy(x => x.DisplayOrder)
                .ThenBy(x => x.Id)
                .ToList()
        );

        var header = personAgeTypes.GeneratePaginationHttpHeaders();

        return (header, data);
    }
}
