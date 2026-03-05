using Liberty.Reservation.Application.Cqrs.BaseQueries;
using Liberty.Reservation.Manager.Application.Auth;

namespace Liberty.Reservation.Manager.WebAPI.Application.UserCases.Queries.Facility;

public class FacilityGetMinimumPriceQueryHandler(
    IMapper mapper,
    IFacilityRepository facilityRepository,
    ISecurityContextAccessor securityContextAccessor
) : QuerySingleBaseHandler<FacilityGetMinimumPriceQuery, FacilityMinimumPriceResponse>(mapper)
{
    protected override async Task<(IHeaderDictionary, FacilityMinimumPriceResponse)> HandleAsync(
        FacilityGetMinimumPriceQuery request,
        CancellationToken cancellationToken
    )
    {
        var facilityId = securityContextAccessor.FacilityKey;

        var queryable = facilityRepository
            .GetQueryableWithAsNoTracking()
            .Where(x => x.Id == facilityId)
            .ProjectTo<FacilityMinimumPriceResponse>(Mapper.ConfigurationProvider);

        var facilityResponse = await queryable.SingleOrDefaultAsync(cancellationToken) ?? throw new FacilityNotfoundException();

        return (new HeaderDictionary(), facilityResponse);
    }
}
