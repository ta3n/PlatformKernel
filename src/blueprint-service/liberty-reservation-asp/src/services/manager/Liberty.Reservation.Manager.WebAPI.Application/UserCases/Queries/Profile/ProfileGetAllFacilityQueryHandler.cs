using Liberty.Reservation.Application.Cqrs.BaseQueries;
using Liberty.Reservation.Manager.Application.Auth;
using Liberty.Reservation.Manager.WebAPI.Application.ExternalServices.Membership.Facility.Services;

namespace Liberty.Reservation.Manager.WebAPI.Application.UserCases.Queries.Profile;

public class ProfileGetAllFacilityQueryHandler(
    IMapper mapper,
    IFacilityRepository facilityRepository,
    ICheckFacilityService checkFacilityService,
    ISecurityContextAccessor securityContextAccessor
) : QueryListBaseHandler<ProfileGetAllFacilityQuery, FacilityManagementResponse>(mapper)
{
    protected override async Task<(IHeaderDictionary, IEnumerable<FacilityManagementResponse>)> HandleAsync(
        ProfileGetAllFacilityQuery request,
        CancellationToken cancellationToken
    )
    {
        var managerId = securityContextAccessor.ApplicationUserKey ?? string.Empty;

        var facilityExternals = await checkFacilityService.GetAllFacilitiesFromMembershipAsync(
            managerId,
            cancellationToken
        );
        if (facilityExternals is null)
        {
            return (new HeaderDictionary(), []);
        }

        var facilityExternalCodes = facilityExternals.Facilities.Select(x => x.Code).ToList();

        var facilities = await facilityRepository
            .GetQueryableWithAsNoTracking()
            .Where(x => facilityExternalCodes.Contains(x.Code) && x.IsEnabled)
            .ToListAsync(cancellationToken);

        var data = (from facilityDto in facilityExternals.Facilities
            let isEnable = facilities.Exists(f => f.Code == facilityDto.Code)
            select new FacilityManagementResponse(
                facilityDto.Code,
                facilityDto.Meta.FirstOrDefault(x => x.Key == "code")?.Value,
                facilityDto.Name,
                isEnable && facilityDto.IsEnabled
            )).ToList();

        return (new HeaderDictionary(), data);
    }
}
