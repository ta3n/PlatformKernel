using Liberty.Reservation.Manager.Application.Auth;
using Liberty.Reservation.Manager.Application.Domains.Services;

namespace Liberty.Reservation.Manager.WebAPI.Application.UserCases.Commands.RoomGroup;

public class RoomGroupCreateCommandHandler(
    ILogger<RoomGroupCreateCommandHandler> logger,
    IUnitOfWork unitOfWork,
    IMapper mapper,
    ISecurityContextAccessor securityContextAccessor,
    IFacilityService facilityService,
    IRoomGroupService roomGroupService,
    IFacilityRoomGroupService facilityRoomGroupService
) : CreateCommandHandlerBase<RoomGroupCreateCommand, long>(unitOfWork, mapper)
{
    protected override async Task<long> HandleAsync(
        RoomGroupCreateCommand request,
        CancellationToken cancellationToken
    )
    {
        var payload = request.Payload;
        var facilityId = securityContextAccessor.FacilityKey;

        var existingCount = await facilityService.CountByIdsAsync(
            [facilityId],
            cancellationToken
        );
        if (existingCount == 0)
        {
            throw new FacilityNotfoundException();
        }

        var isGroupNameExisting = await facilityRoomGroupService.CheckGroupNameExisting(
            facilityId,
            0,
            payload.GroupName,
            cancellationToken
        );

        if (isGroupNameExisting)
        {
            throw new FacilityRoomGroupDuplicateGroupNameException(
                facilityId,
                payload.GroupName ?? string.Empty
            );
        }

        var roomGroup = Mapper.Map<Reservation.Application.Contexts.DataContexts.Entities.Data.RoomGroup>(payload);

        try
        {
            await UnitOfWork.BeginTransactionAsync(cancellationToken: cancellationToken);

            var newRoomGroup = await roomGroupService.CreateRoomGroupWithFacilityAsync(
                roomGroup,
                facilityId,
                false,
                cancellationToken
            );

            await UnitOfWork.CommitAsync(cancellationToken);

            return newRoomGroup.Id;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Create room group failed: {Message}", ex.Message);
            await UnitOfWork.RollbackAsync(cancellationToken);
            throw new AppLibertyException(ex.Message, ex);
        }
    }
}
