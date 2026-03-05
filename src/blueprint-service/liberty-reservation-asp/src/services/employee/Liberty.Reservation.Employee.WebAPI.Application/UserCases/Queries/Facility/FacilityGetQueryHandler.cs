using Liberty.Reservation.Application.Cqrs.BaseQueries;
using Liberty.Reservation.Employee.Application.Domains.Repositories.Interfaces;
using Liberty.Reservation.Employee.WebAPI.Application.ExternalServices.Membership.Facility.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Liberty.Reservation.Employee.WebAPI.Application.UserCases.Queries.Facility;

public class FacilityGetQueryHandler(
    IMapper mapper,
    IFacilityExternalRepository facilityExternalRepository,
    IFacilityRepository facilityRepository
) : QuerySingleBaseHandler<FacilityGetQuery, FacilityDetailResponse>(mapper)
{
    protected override async Task<(IHeaderDictionary, FacilityDetailResponse)> HandleAsync(
        FacilityGetQuery request,
        CancellationToken cancellationToken
    )
    {
        var facilityExternal = await facilityExternalRepository.GetFacilityByIdAsync(
                request.Id,
                cancellationToken
            )
            ?? throw new FacilityNotfoundException();

        var facility = await facilityRepository
            .GetQueryableWithAsNoTracking()
            .Where(x => x.Id == request.Id)
            .Select(
                a =>
                    new FacilityReservationResponse(
                        a.Id,
                        a.Fax,
                        a.CanOnLinePayment,
                        a.IsEnabled,
                        a.Meta!.SystemEMail,
                        a.Memo,
                        a.FacilitySites!.Select(
                            t =>
                                new SiteOfFacilityResponse(
                                    t.SiteId,
                                    t.Site!.Name!.GetValueByHeader(),
                                    t.Site.IsEnabled,
                                    t.Site.IsVisible
                                )
                        ),
                        a.FacilityFaxServices!.Select(
                            t =>
                                new FaxServiceOfFacilityResponse(
                                    t.FaxServiceId,
                                    t.FaxService!.Name,
                                    t.FaxService!.IsEnabled,
                                    t.FaxService!.IsVisible
                                )
                        )
                    )
            )
            .FirstOrDefaultAsync(cancellationToken);

        var facilityDetailResponse = new FacilityDetailResponse(
            request.Id,
            facilityExternal.Code,
            facilityExternal.Name,
            facilityExternal.Kana,
            facilityExternal.Postcode,
            facilityExternal.Address1,
            facilityExternal.Address2,
            facilityExternal.Address3,
            facilityExternal.Address4,
            facility?.Fax,
            facility?.Email ?? facilityExternal.Email,
            facility?.Memo ?? facilityExternal.Memo,
            facilityExternal.RecordCode,
            facility?.CanOnLinePayment ?? false,
            facility?.Sites,
            facility?.FaxServices
        );
        return (new HeaderDictionary(), facilityDetailResponse);
    }
}
