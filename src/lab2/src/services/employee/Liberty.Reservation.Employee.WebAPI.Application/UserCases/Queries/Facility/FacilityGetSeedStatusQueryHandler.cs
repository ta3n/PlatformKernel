using Liberty.Reservation.Application.Cqrs.BaseQueries;
using Liberty.Reservation.Employee.Application.Domains.Services.Interfaces;

namespace Liberty.Reservation.Employee.WebAPI.Application.UserCases.Queries.Facility;

public class FacilityGetSeedStatusQueryHandler(
    IMapper mapper,
    IFacilityInitStatusService facilityInitStatusService
) : QuerySingleBaseHandler<FacilityGetSeedStatusQuery, FacilitySeedStatusResponse>(mapper)
{
    protected override async Task<(IHeaderDictionary, FacilitySeedStatusResponse)> HandleAsync(
        FacilityGetSeedStatusQuery request,
        CancellationToken cancellationToken
    )
    {
        var facilityInitStatus = await facilityInitStatusService.GetFacilitySeedDataStatusAsync(
            request.Id,
            cancellationToken
        );

        return (
            new HeaderDictionary(),
            new FacilitySeedStatusResponse(
                facilityInitStatus.Id,
                facilityInitStatus.Code,
                facilityInitStatus.Name?.GetValueByHeader(),
                facilityInitStatus.HasSeededPersonAgeTypeData
            )
        );
    }
}
