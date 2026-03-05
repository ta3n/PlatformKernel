using Liberty.ApplicationShared.Domains.Repositories;
using Liberty.Reservation.Site.WebAPI.Application.ExternalServices.Membership.Facility.Dtos;

namespace Liberty.Reservation.Site.WebAPI.Application.ExternalServices.Membership.Facility.Repositories.Implements;

public class FacilityExternalRepository(
    MembershipFacilityExternalDbContext dataContext
) : GenericRepository<Models.Facility>(dataContext), IFacilityExternalRepository
{
    private static readonly Func<MembershipFacilityExternalDbContext, string, CancellationToken, Task<FacilityInfoDto?>>
        CompiledGetFacilityQuery = EF.CompileAsyncQuery(
            (
                    MembershipFacilityExternalDbContext context,
                    string facilityCode,
                    CancellationToken ct
                ) =>
                context.Facilities
                    .AsNoTracking()
                    .Where(x => x.Code == facilityCode)
                    .Select(
                        x => new FacilityInfoDto
                        {
                            Id = x.Id,
                            Code = x.Code,
                            Name = x.Name,
                            RecordCode = context.KeyValues
                                .Where(kv => kv.Key == "code" && kv.Record == x.Code)
                                .Select(kv => kv.Value)
                                .FirstOrDefault()
                        }
                    )
                    .FirstOrDefault()
        );

    public async Task<bool> CheckFacilityAvailableAsync(
        string facilityCode,
        CancellationToken cancellationToken = default
    )
    {
        return await GetQueryableWithAsNoTracking()
            .AnyAsync(
                x =>
                    x.Code == facilityCode,
                cancellationToken
            );
    }

    public async Task<FacilityInfoDto?> GetFacilityAvailableByCodeAsync(
        string facilityCode,
        CancellationToken cancellationToken = default
    )
    {
        return await CompiledGetFacilityQuery(dataContext, facilityCode, cancellationToken);
    }
}
