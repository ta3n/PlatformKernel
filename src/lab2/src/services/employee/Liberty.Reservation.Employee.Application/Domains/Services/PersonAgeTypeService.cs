using Liberty.Reservation.Application.Domains.Services;
using Liberty.Reservation.Employee.Application.Domains.Repositories.Interfaces;
using Liberty.Reservation.Employee.Application.Domains.Services.Interfaces;
using Liberty.Reservation.Employee.Application.Exceptions;
using Microsoft.Extensions.Logging;

namespace Liberty.Reservation.Employee.Application.Domains.Services;

public class PersonAgeTypeService(
    ILogger<PersonAgeType> logger,
    IPersonAgeTypeRepository personAgeTypeRepository
) : BaseService<PersonAgeType>(logger, personAgeTypeRepository, new PersonAgeTypeNotfoundException()), IPersonAgeTypeService
{
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
                 existingEntity.Name = updateEntity.Name;
                 existingEntity.AgeMax = updateEntity.AgeMax;
                 existingEntity.AgeMin = updateEntity.AgeMin;
                 existingEntity.IsEnabled = updateEntity.IsEnabled;
                 existingEntity.Meta = updateEntity.Meta;

                 return existingEntity;
             });

        return base.UpdateAsync(
            entityToUpdate,
            autoSave,
            updateHandle,
            cancellationToken
        );
    }

    public async Task<bool> CheckExistingPersonAgeGroupAsync(
        string groupName,
        long personAgeTypeId = 0,
        CancellationToken cancellationToken = default
    )
    {
        return await personAgeTypeRepository
            .GetQueryableWithAsNoTracking()
            .AnyAsync(
                x => x.Meta != null
                    && x.Meta.GroupName == groupName
                    && x.IsMaster
                    && x.Id != personAgeTypeId,
                cancellationToken
            );
    }

    public async Task<PersonAgeType> GetPersonAgeTypeIsMainAsync(
        long id,
        CancellationToken cancellationToken = default
    )
    {
        return await personAgeTypeRepository
                .GetQueryableWithAsNoTracking()
                .FirstOrDefaultAsync(
                    x => x.Id == id
                        && x.IsMain,
                    cancellationToken
                )
            ?? new PersonAgeType();
    }
}
