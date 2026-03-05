namespace Liberty.Reservation.Manager.WebAPI.Application.UserCases.Commands.RoomGroup;

public class RoomGroupDeleteCommandHandler(
    ILogger<RoomGroupDeleteCommandHandler> logger,
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IRoomGroupService roomGroupService,
    IFacilityRoomGroupService facilityRoomGroupService,
    IRoomGroupBedTypeService roomGroupBedTypeService,
    IRoomGroupCategoryService roomGroupCategoryService,
    IRoomGroupSiteService roomGroupSiteService,
    IFileRoomGroupService fileRoomGroupService
) : DeleteCommandHandlerBase<RoomGroupDeleteCommand, long>(unitOfWork, mapper)
{
    protected override async Task<long> HandleAsync(
        RoomGroupDeleteCommand request,
        CancellationToken cancellationToken
    )
    {
        var paylaod = request.Payload;

        var existingFacilityRoomGroup = await facilityRoomGroupService.FindAllByRoomGroupIdAsync(
            paylaod.Id,
            cancellationToken
        );

        var existingBedTypesOfRoom = await roomGroupBedTypeService.FindAllByRoomGroupIdAsync(
            paylaod.Id,
            cancellationToken
        );

        var existingCategoriesOfRoom = await roomGroupCategoryService.FindAllByRoomGroupIdAsync(
            paylaod.Id,
            cancellationToken
        );

        var existingSitesOfRoom = await roomGroupSiteService.FindAllByRoomGroupIdAsync(
            paylaod.Id,
            cancellationToken
        );

        var existingFilesOfRoom = await fileRoomGroupService.FindAllByRoomGroupIdAsync(
            paylaod.Id,
            cancellationToken
        );

        try
        {
            await UnitOfWork.BeginTransactionAsync(cancellationToken: cancellationToken);

            var removeRoomGroup = await roomGroupService.DeleteAsync(
                paylaod.Id,
                false,
                cancellationToken
            );

            _ = await facilityRoomGroupService.DeleteRangeAsync(
                existingFacilityRoomGroup,
                false,
                cancellationToken
            );

            _ = await roomGroupBedTypeService.DeleteRangeAsync(
                existingBedTypesOfRoom,
                false,
                cancellationToken
            );

            _ = await roomGroupCategoryService.DeleteRangeAsync(
                existingCategoriesOfRoom,
                false,
                cancellationToken
            );

            _ = await roomGroupSiteService.DeleteRangeAsync(
                existingSitesOfRoom,
                false,
                cancellationToken
            );

            _ = await fileRoomGroupService.DeleteRangeAsync(
                existingFilesOfRoom,
                false,
                cancellationToken
            );

            await UnitOfWork.CommitAsync(cancellationToken);

            return removeRoomGroup.Id;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Delete room group failed: {Message}", ex.Message);
            await UnitOfWork.RollbackAsync(cancellationToken);
            throw new AppLibertyException(ex.Message, ex);
        }
    }
}
