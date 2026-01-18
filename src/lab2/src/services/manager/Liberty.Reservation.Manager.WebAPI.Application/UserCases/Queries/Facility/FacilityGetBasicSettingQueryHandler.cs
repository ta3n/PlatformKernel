using Liberty.Reservation.Application.Cqrs.BaseQueries;
using Liberty.Reservation.Manager.Application.Auth;

namespace Liberty.Reservation.Manager.WebAPI.Application.UserCases.Queries.Facility;

public class FacilityGetBasicSettingQueryHandler(
    IMapper mapper,
    IFacilityRepository facilityRepository,
    IFacilityFileRepository facilityFileRepository,
    ISecurityContextAccessor securityContextAccessor
) : QuerySingleBaseHandler<FacilityGetBasicSettingQuery, FacilityDetailBasicSettingResponse>(mapper)
{
    protected override async Task<(
        IHeaderDictionary,
        FacilityDetailBasicSettingResponse
        )> HandleAsync(
        FacilityGetBasicSettingQuery request,
        CancellationToken cancellationToken
    )
    {
        var facilityId = securityContextAccessor.FacilityKey;

        var facilityResponse =
            await facilityRepository
                .GetQueryableWithAsNoTracking()
                .Where(x => x.Id == facilityId)
                .Select(
                    x =>
                        new FacilityDetailBasicSettingResponse(
                            x.Code,
                            x.Name!.GetValueByHeader(),
                            x.Kana,
                            x.Description,
                            x.Postcode,
                            x.Address1 != null ? x.Address1!.GetValueByHeader() : string.Empty,
                            x.Address2 != null ? x.Address2!.GetValueByHeader() : string.Empty,
                            x.Address3 != null ? x.Address3!.GetValueByHeader() : string.Empty,
                            x.Address4 != null ? x.Address4!.GetValueByHeader() : string.Empty,
                            x.Phone,
                            x.Fax,
                            x.Url,
                            x.AreaId,
                            x.CategoryId,
                            x.Meta!.RoomNumberWesternStyle,
                            x.Meta!.RoomNumberJapaneseStyle,
                            x.Meta!.RoomNumberJapaneseWesternStyle,
                            x.Meta!.RoomNumberOtherStyle,
                            new FileOfFacilityResponse(x.Meta!.Logo),
                            x.TimeZone,
                            x.TimeZoneId
                        )
                )
                .SingleOrDefaultAsync(cancellationToken)
            ?? throw new FacilityNotfoundException();

        var facilityFileDescriptionQueryable = facilityFileRepository
            .GetQueryableWithAsNoTracking()
            .Where(x => x.FacilityId == facilityId)
            .Where(x => x.File!.Code == facilityResponse.File!.Code)
            .Select(x => x.File!.Description);

        var facilityDescription = await facilityFileDescriptionQueryable.SingleOrDefaultAsync(cancellationToken);

        var updatedFacilityResponse = facilityResponse.File is not null
            ? facilityResponse with { File = facilityResponse.File with { Description = facilityDescription } }
            : facilityResponse;

        return (new HeaderDictionary(), updatedFacilityResponse);
    }
}
