using Liberty.Cache.Services;
using Liberty.Entity.Utils;
using Liberty.Entity.ValueObjects;
using Liberty.Reservation.Manager.Application.Auth;
using Microsoft.Extensions.DependencyInjection;

namespace Liberty.Reservation.Manager.WebAPI.Application.UserCases.Commands.RoomGroup;

public class RoomGroupUpdateBasicConfigurationCommandHandler(
    ILogger<RoomGroupUpdateBasicConfigurationCommandHandler> logger,
    IServiceProvider serviceProvider,
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IRoomGroupService roomGroupService,
    IBedTypeService bedTypeService,
    IFileRoomGroupService fileRoomGroupService
) : UpdateCommandHandlerBase<RoomGroupUpdateBasicConfigurationCommand, long>(unitOfWork, mapper)
{
    private readonly ISecurityContextAccessor _securityContextAccessor = serviceProvider.GetRequiredService<ISecurityContextAccessor>();
    private readonly ICacheService _cacheService = serviceProvider.GetRequiredService<ICacheService>();

    protected override async Task<long> HandleAsync(
        RoomGroupUpdateBasicConfigurationCommand request,
        CancellationToken cancellationToken
    )
    {
        var payload = request.Payload;
        var facilityId = _securityContextAccessor.FacilityKey;

        var bedTypeIds = payload.BedTypes
            .Select(x => x.Id)
            .ToArray();
        var existingCountBedTypes = await bedTypeService.CountByIdsAsync(
            bedTypeIds,
            cancellationToken
        );
        if (existingCountBedTypes != bedTypeIds.Length)
        {
            throw new BedTypeNotfoundException();
        }

        try
        {
            await UnitOfWork.BeginTransactionAsync(cancellationToken: cancellationToken);

            var roomGroup = Mapper.Map<Reservation.Application.Contexts.DataContexts.Entities.Data.RoomGroup>(payload);
            _ = await fileRoomGroupService.ChangeFilesOfRoomGroupAsync(
                payload.Id ?? 0,
                payload.Files.Select(x => (x.Id, x.Index)),
                false,
                cancellationToken
            );

            await UpdatePlanAsync(payload, facilityId, roomGroup, true, cancellationToken);

            var editRoomGroup = await roomGroupService.UpdateBasicConfigurationOfRoomGroupAsync(
                roomGroup,
                false,
                cancellationToken
            );

            _ = await roomGroupService.ChangeBedTypesOfRoomGroupAsync(
                editRoomGroup.Id,
                payload.BedTypes.Select(x => (x.Id, x.Number)),
                false,
                cancellationToken
            );

            await UnitOfWork.CommitAsync(cancellationToken);

            await _cacheService.ResetAsync(
                string.Format(CacheKeys.ResetPatternSiteBookingSearch, _securityContextAccessor.FacilityKey),
                false,
                cancellationToken
            );

            await _cacheService.ResetAsync(
                CacheKeys.ResetPatternBookingDetails,
                false,
                cancellationToken
            );

            return editRoomGroup.Id;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Update basic configuration of room group failed: {Message}", ex.Message);
            await UnitOfWork.RollbackAsync(cancellationToken);
            throw new AppLibertyException(ex.Message, ex);
        }
    }

    private async Task UpdatePlanAsync(
        RoomGroupUpdateBasicConfigurationRequest payload,
        long facilityId,
        Reservation.Application.Contexts.DataContexts.Entities.Data.RoomGroup roomGroup,
        bool disable,
        CancellationToken cancellationToken
    )
    {
        if (disable) { return; }

        var planService = serviceProvider.GetRequiredService<IPlanService>();

        var planForRoomOnly = await planService.GetPlanWithRoomOnlyTypeAsync(
            payload.Id ?? 0,
            facilityId,
            cancellationToken
        );

        if (planForRoomOnly is null)
        {
            planForRoomOnly = await planService.CreatePlanWithRoomOnlyTypeAsync(
                roomGroup.Name!.GetValueByHeader(),
                facilityId,
                false,
                cancellationToken
            );
            planForRoomOnly.Summary =
                new MultilingualText { { LanguageHeaderUtil.GetLanguageCodeFromHeader(), payload.Overview ?? string.Empty } };
            planForRoomOnly.Description ??= [];
            planForRoomOnly.Description.UpdateLocalized(
                new MultilingualText { { _securityContextAccessor.GetLanguageCode(), payload.Overview ?? string.Empty } }
            );

            _ = await planService.CreatePlanWithRoomOnlyTypeRelationAsync(
                roomGroup.Id,
                planForRoomOnly,
                false,
                cancellationToken
            );
        }
        else
        {
            planForRoomOnly.Summary =
                new MultilingualText { { LanguageHeaderUtil.GetLanguageCodeFromHeader(), payload.Overview ?? string.Empty } };
            planForRoomOnly.Description ??= [];
            planForRoomOnly.Description.UpdateLocalized(
                new MultilingualText { { _securityContextAccessor.GetLanguageCode(), payload.Overview ?? string.Empty } }
            );
            _ = await planService.UpdateOverviewAsync(planForRoomOnly, false, cancellationToken: cancellationToken);
        }

        var listFilePlan = payload.Files.Select(
                x => new FilePlan
                {
                    Plan = planForRoomOnly,
                    FileId = x.Id,
                    Index = x.Index
                }
            )
            .ToList();

        _ = await planService.ChangeFilePlanAsync(
            planForRoomOnly,
            listFilePlan,
            false,
            cancellationToken
        );
    }
}
