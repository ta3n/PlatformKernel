namespace Liberty.Reservation.Manager.WebAPI.Application.UserCases.Commands.PlanPrice;

public class PlanRoomGroupSiteUpdateMinimumPriceCommandHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IPlanRoomGroupSiteService planRoomGroupSiteService
) : UpdateCommandHandlerBase<PlanRoomGroupSiteUpdateMinimumPriceCommand, long>(unitOfWork, mapper)
{
    protected override async Task<long> HandleAsync(
        PlanRoomGroupSiteUpdateMinimumPriceCommand request,
        CancellationToken cancellationToken
    )
    {
        var entityToUpdate = Mapper.Map<PlanRoomGroupSite>(request.Payload);
        var editPlanRoomGroupSite = await planRoomGroupSiteService.UpdateMinimumPriceAsync(
            request.PlanId,
            request.RoomGroupId,
            request.SiteId,
            entityToUpdate,
            cancellationToken
        );

        return editPlanRoomGroupSite.PlanId;
    }
}
