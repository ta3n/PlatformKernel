namespace Liberty.Reservation.Manager.Application.Domains.Services;

public class FacilityPersonAgeTypeService(
    ILogger<FacilityPersonAgeTypeService> logger,
    IFacilityPersonAgeTypeRepository repository
) : BaseServiceRelation<FacilityPersonAgeType>(logger, repository), IFacilityPersonAgeTypeService
{
    public async Task<List<FacilityPersonAgeType>> FindAllByFacilityIdAsync(
        long facilityId,
        CancellationToken cancellationToken = default
    )
    {
        var response = await repository
            .GetQueryableWithAsNoTracking()
            .Include(x => x.PersonAgeType)
            .Where(x => x.FacilityId == facilityId)
            .ToListAsync(cancellationToken);

        return response;
    }

    public async Task<FacilityPersonAgeType?> FindByFacilityIdAsync(
        long facilityId,
        long personAgeTypeId,
        CancellationToken cancellationToken = default
    )
    {
        var response = await repository
            .GetQueryableWithAsNoTracking()
            .Include(x => x.PersonAgeType)
            .Where(x => x.FacilityId == facilityId)
            .Where(x => x.PersonAgeTypeId == personAgeTypeId)
            .SingleOrDefaultAsync(cancellationToken);

        return response;
    }
}
