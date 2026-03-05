using Liberty.Reservation.Application.Cqrs.BaseQueries;
using Liberty.Reservation.Employee.Application.Domains.Repositories.Interfaces;
using Liberty.Reservation.Employee.WebAPI.Application.ExternalServices.Membership.Facility.Repositories;
using Liberty.Reservation.Employee.WebAPI.Application.Web.Rest.Utilities;
using Microsoft.EntityFrameworkCore;

namespace Liberty.Reservation.Employee.WebAPI.Application.UserCases.Queries.Facility;

public class FacilityGetAllQueryHandler(
    IMapper mapper,
    IFacilityRepository facilityRepository,
    IFacilityExternalRepository facilityExternalRepository
) : QueryPageBaseHandler<FacilityGetAllQuery, FacilityResponse>(mapper)
{
    protected override async Task<(IHeaderDictionary, IEnumerable<FacilityResponse>)> HandleAsync(
        FacilityGetAllQuery request,
        CancellationToken cancellationToken
    )
    {
        var facilityExternals = await facilityExternalRepository.GetAllFacilitiesByPageAsync(
            request.Pageable,
            cancellationToken
        );
        var facilityExternalIds = facilityExternals!.Content.Select(x => x.Id).ToList();

        var facilities = await facilityRepository
            .GetQueryableWithAsNoTracking()
            .Where(x => facilityExternalIds.Contains(x.Id))
            .ToListAsync(cancellationToken);

        List<FacilityResponse> listFacility = [];
        foreach (var facilityExternal in facilityExternals.Content)
        {
            var facility = facilities.Find(x => x.Id == facilityExternal.Id);

            listFacility.Add(
                new FacilityResponse(
                    facilityExternal.Id,
                    facilityExternal.Code ?? string.Empty,
                    facility?.DisplayOrder ?? 0,
                    facility?.IsEnabled ?? false,
                    facilityExternal.Memo
                )
                {
                    Name = facilityExternal.Name,
                    Kana = facilityExternal.Kana,
                    PostCode = facilityExternal.Postcode,
                    RecordCode = facilityExternal.RecordCode
                }
            );
        }

        var headers = facilityExternals.GeneratePaginationHttpHeaders();

        return (headers, listFacility);
    }
}
