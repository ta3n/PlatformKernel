using Liberty.Reservation.Application.Cqrs.BaseQueries;
using Liberty.Reservation.Employee.Application.Domains.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Liberty.Reservation.Employee.WebAPI.Application.UserCases.Queries.Facility;

public class FacilityGetFaxQueryHandler(
    IMapper mapper,
    IFacilityRepository facilityRepository
) : QuerySingleBaseHandler<FacilityGetFaxQuery, FacilityDetailFaxResponse>(mapper)
{
    protected override async Task<(IHeaderDictionary, FacilityDetailFaxResponse)> HandleAsync(
        FacilityGetFaxQuery request,
        CancellationToken cancellationToken
    )
    {
        var queryable = facilityRepository
            .GetQueryableWithAsNoTracking()
            .Where(x => x.Id == request.Id)
            .Select(
                x => new FacilityDetailFaxResponse(
                    x.Id,
                    x.Fax,
                    x.UseFax
                )
            );

        var faxInfoOfFacility = await queryable.FirstOrDefaultAsync(
                cancellationToken
            )
            ?? new FacilityDetailFaxResponse(request.Id, string.Empty, false);

        return (new HeaderDictionary(), faxInfoOfFacility);
    }
}
