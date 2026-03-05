using Liberty.Reservation.Manager.Application.Auth;

namespace Liberty.Reservation.Manager.Application.Domains.Services;

public class PersonAgeTypeService(
    ILogger<PersonAgeTypeService> logger,
    ISecurityContextAccessor securityContextAccessor,
    IPersonAgeTypeRepository personAgeTypeRepository
) : BaseService<PersonAgeType>(logger, personAgeTypeRepository, new PersonAgeTypeNotfoundException()),
    IPersonAgeTypeService
{
    protected override IQueryable<PersonAgeType> GetQueryable()
    {
        var facilityId = securityContextAccessor.FacilityKey;

        return personAgeTypeRepository
            .GetQueryableWithAsNoTracking()
            .Where(
                x => x.FacilityPersonAgeTypes!.Any(
                    t => t.FacilityId == facilityId
                )
            );
    }

    public async Task<IPage<PersonAgeType>> GetAllPersonAgeTypes(
        IPageable pageable,
        bool isVisible = false,
        CancellationToken cancellationToken = default
    )
    {
        var facilityId = securityContextAccessor.FacilityKey;

        var queryable = personAgeTypeRepository
            .GetQueryableWithAsNoTracking()
            .Include(x => x.PersonAgeTypeSpaTaxDatas!)
            .ThenInclude(x => x.SpaTaxData)
            .Where(
                x => x.FacilityPersonAgeTypes!.Any(
                    t => t.FacilityId == facilityId
                )
            );

        if (isVisible)
        {
            queryable = queryable.Where(x => x.IsEnabled && x.IsVisible);
        }

        var page = await queryable
            .OrderByDescending(x => x.DisplayOrder)
            .UsePageableAsync(
                pageable,
                cancellationToken: cancellationToken
            );

        return page;
    }

    public override Task<PersonAgeType> UpdateAsync(
        PersonAgeType entityToUpdate,
        bool autoSave = true,
        Func<PersonAgeType, PersonAgeType, PersonAgeType>? updateAction = null,
        CancellationToken cancellationToken = default
    )
    {
        var updateHandle = updateAction
            ?? ((
                existingEntity,
                updateEntity
            ) =>
            {
                existingEntity.IsVisible = updateEntity.IsVisible;
                existingEntity.IsEnabled = updateEntity.IsVisible;

                return existingEntity;
            });

        return base.UpdateAsync(
            entityToUpdate,
            autoSave,
            updateHandle,
            cancellationToken
        );
    }
}
