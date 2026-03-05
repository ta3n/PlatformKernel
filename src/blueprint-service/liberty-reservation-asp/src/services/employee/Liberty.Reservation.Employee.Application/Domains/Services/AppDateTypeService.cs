using Liberty.ApplicationShared.Utils;
using Liberty.Cache.Services;
using Liberty.Reservation.Application.Domains.Services;
using Liberty.Reservation.Employee.Application.Domains.Repositories.Interfaces;
using Liberty.Reservation.Employee.Application.Domains.Services.Interfaces;
using Liberty.Reservation.Employee.Application.Exceptions;
using Liberty.UnitOfWork.Abstractions;
using Microsoft.Extensions.Logging;

namespace Liberty.Reservation.Employee.Application.Domains.Services;

public class AppDateTypeService(
    ILogger<AppDateTypeService> logger,
    ICacheService cacheService,
    IAppDateTypeRepository appDateTypeRepository,
    IFacilityAppDateTypeRepository facilityAppDateTypeRepository,
    IDateTypeOfAppDateRepository dateTypeOfAppDateRepository,
    IUnitOfWork unitOfWork
) : BaseService<AppDateType>(logger, appDateTypeRepository, new AppDateTypeNotfoundException()), IAppDateTypeService
{
    protected override IQueryable<AppDateType> GetQueryable()
    {
        return appDateTypeRepository
            .GetQueryableWithAsNoTracking()
            .Where(
                x => !facilityAppDateTypeRepository
                    .GetQueryableWithAsNoTracking()
                    .Select(t => t.AppDateTypeId)
                    .Contains(x.Id)
            );
    }

    public override Task<AppDateType> CreateAsync(
        AppDateType entityToCreate,
        bool autoSave = true,
        CancellationToken cancellationToken = default
    )
    {
        entityToCreate.Code = EntityUtil.CreateCode();
        entityToCreate.RecordMemo = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();

        return base.CreateAsync(entityToCreate, autoSave, cancellationToken);
    }

    public override Task<AppDateType> UpdateAsync(
        AppDateType entityToUpdate,
        bool autoSave = true,
        Func<AppDateType, AppDateType, AppDateType>? updateAction = null,
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
                existingEntity.ShortName = updateEntity.ShortName;
                existingEntity.Description = updateEntity.Description;
                existingEntity.Color = updateEntity.Color;

                return existingEntity;
            });

        return base.UpdateAsync(
            entityToUpdate,
            autoSave,
            updateHandle,
            cancellationToken
        );
    }

    public override async Task<AppDateType> EnableAsync(
        long id,
        bool isEnabled,
        bool autoSave = true,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            await unitOfWork.BeginTransactionAsync(cancellationToken: cancellationToken);

            var appDateType = await base.EnableAsync(id, isEnabled, autoSave, cancellationToken);

            var dateTypesOfAppDate = await dateTypeOfAppDateRepository.GetQueryable()
                .IgnoreQueryFilters()
                .Where(x => x.AppDateTypeId == id)
                .ToListAsync(cancellationToken);
            foreach (var dateTypeOfAppDate in dateTypesOfAppDate)
            {
                dateTypeOfAppDate.IsDeleted = !isEnabled;

                _ = await dateTypeOfAppDateRepository.UpdateAsync(dateTypeOfAppDate, autoSave, cancellationToken);
            }

            await unitOfWork.CommitAsync(cancellationToken);
            await cacheService.ResetAsync($"{nameof(AppDate)}*", cancellationToken: cancellationToken);

            return appDateType;
        }
        catch (Exception)
        {
            await unitOfWork.RollbackAsync(cancellationToken);

            throw;
        }
    }

    public override async Task<AppDateType> DeleteAsync(
        long id,
        bool autoSave = true,
        CancellationToken cancellationToken = default
    )
    {
        var appDateType = await base.DeleteAsync(id, autoSave, cancellationToken);

        await cacheService.ResetAsync($"{nameof(AppDate)}*", cancellationToken: cancellationToken);

        return appDateType;
    }
}
