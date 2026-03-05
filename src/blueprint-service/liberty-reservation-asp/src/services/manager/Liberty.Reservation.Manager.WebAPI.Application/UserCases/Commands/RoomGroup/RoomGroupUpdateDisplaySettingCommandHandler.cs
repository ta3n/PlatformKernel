using Liberty.Entity.Utils;
using Liberty.Entity.ValueObjects;
using Liberty.Reservation.Manager.Application.Auth;

namespace Liberty.Reservation.Manager.WebAPI.Application.UserCases.Commands.RoomGroup;

public class RoomGroupUpdateDisplaySettingCommandHandler(
    ILogger<RoomGroupUpdateDisplaySettingCommandHandler> logger,
    IUnitOfWork unitOfWork,
    IMapper mapper,
    ISecurityContextAccessor securityContextAccessor,
    IRoomGroupService roomGroupService,
    ICategoryService categoryService,
    IPlanService planService
) : UpdateCommandHandlerBase<RoomGroupUpdateDisplaySettingCommand, long>(unitOfWork, mapper)
{
    protected override async Task<long> HandleAsync(
        RoomGroupUpdateDisplaySettingCommand request,
        CancellationToken cancellationToken
    )
    {
        var payload = request.Payload;
        var tags = payload.Tags?.Distinct().ToArray() ?? [];

        var facilityId = securityContextAccessor.FacilityKey;

        var existingCountMasterCategory = await categoryService.CountMasterByIdsAsync(
            [
                ..payload.RoomGroupMasterCategoryIds ?? [],
                ..payload.RoomAmenityCategoryIds ?? [],
                ..payload.RoomGroupFeatureCategoryIds ?? [],
                ..payload.RoomGroupEquipmentCategoryIds ?? []
            ],
            [
                CategoryTypes.RoomGroup,
                CategoryTypes.Amenity,
                CategoryTypes.RoomGroupEquipment,
                CategoryTypes.RoomGroupFeature
            ],
            false,
            cancellationToken
        );

        var categoryIds = GetUpdateCategoryIds(payload);
        var existingCountCategoryWithFacility = await categoryService.CountByIdsAsync(
            [.. payload.RoomGroupCategoryIds ?? []],
            [
                CategoryTypes.RoomGroup
            ],
            cancellationToken
        );

        if (existingCountCategoryWithFacility + existingCountMasterCategory != categoryIds.Length)
        {
            throw new CategoryNotfoundException();
        }

        try
        {
            await UnitOfWork.BeginTransactionAsync(cancellationToken: cancellationToken);

            var roomGroup = new Reservation.Application.Contexts.DataContexts.Entities.Data.RoomGroup
            {
                Id = payload.Id ?? 0,
                Tag = string.Join(",", tags)
            };
            var editRoomGroup = await roomGroupService.UpdateDisplaySettingOfRoomGroupAsync(
                roomGroup,
                false,
                cancellationToken
            );

            await UpdatePlanAsync(payload, tags, facilityId, editRoomGroup, true, cancellationToken);

            _ = await roomGroupService.ChangeCategoriesOfRoomGroupAsync(
                editRoomGroup.Id,
                categoryIds,
                false,
                cancellationToken
            );

            await UnitOfWork.CommitAsync(cancellationToken);

            return editRoomGroup.Id;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Update publication setting of room group failed: {Message}", ex.Message);
            await UnitOfWork.RollbackAsync(cancellationToken);
            throw new AppLibertyException(ex.Message, ex);
        }
    }

    private async Task UpdatePlanAsync(
        RoomGroupUpdateDisplaySettingRequest payload,
        string[] tags,
        long facilityId,
        Reservation.Application.Contexts.DataContexts.Entities.Data.RoomGroup editRoomGroup,
        bool disable,
        CancellationToken cancellationToken
    )
    {
        if (disable) { return; }

        var planForRoomOnly = await planService.GetPlanWithRoomOnlyTypeAsync(
            payload.Id ?? 0,
            facilityId,
            cancellationToken
        );

        if (planForRoomOnly is null)
        {
            planForRoomOnly = await planService.CreatePlanWithRoomOnlyTypeAsync(
                editRoomGroup.Name!.GetValueByHeader(),
                facilityId,
                false,
                cancellationToken
            );

            _ = await planService.CreatePlanWithRoomOnlyTypeRelationAsync(
                editRoomGroup.Id,
                planForRoomOnly,
                false,
                cancellationToken
            );
        }

        planForRoomOnly.Tag = new MultilingualText { { LanguageHeaderUtil.GetLanguageCodeFromHeader(), string.Join(",", tags) } };

        _ = await planService.UpdateAsync(
            planForRoomOnly,
            false,
            cancellationToken: cancellationToken
        );
    }

    private static long[] GetUpdateCategoryIds(
        RoomGroupUpdateDisplaySettingRequest payload
    )
    {
        var categoryIds = new List<long>();

        if (payload.RoomGroupMasterCategoryIds is { Length: > 0 })
        {
            categoryIds.AddRange(payload.RoomGroupMasterCategoryIds.Distinct());
        }

        if (payload.RoomGroupCategoryIds is { Length: > 0 })
        {
            categoryIds.AddRange(payload.RoomGroupCategoryIds.Distinct());
        }

        if (payload.RoomGroupFeatureCategoryIds is { Length: > 0 })
        {
            categoryIds.AddRange(payload.RoomGroupFeatureCategoryIds.Distinct());
        }

        if (payload.RoomGroupEquipmentCategoryIds is { Length: > 0 })
        {
            categoryIds.AddRange(payload.RoomGroupEquipmentCategoryIds.Distinct());
        }

        if (payload.RoomAmenityCategoryIds is { Length: > 0 })
        {
            categoryIds.AddRange(payload.RoomAmenityCategoryIds.Distinct());
        }

        return [.. categoryIds];
    }
}
