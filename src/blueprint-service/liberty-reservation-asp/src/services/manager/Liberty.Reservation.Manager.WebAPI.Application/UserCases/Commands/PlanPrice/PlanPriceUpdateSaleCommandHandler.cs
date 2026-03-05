namespace Liberty.Reservation.Manager.WebAPI.Application.UserCases.Commands.PlanPrice;

public class PlanPriceUpdateSaleCommandHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IPlanRoomGroupSiteService planRoomGroupSiteService
) : UpdateCommandHandlerBase<PlanPriceUpdateSaleCommand, long>(unitOfWork, mapper)
{
    protected override async Task<long> HandleAsync(
        PlanPriceUpdateSaleCommand request,
        CancellationToken cancellationToken
    )
    {
        var existingSiteOfPlanRoom = await planRoomGroupSiteService.FindByPlanIdAndRomTypeIdAsync(
            request.PlanId,
            request.RoomTypeId,
            request.SiteId,
            cancellationToken
        );

        if (existingSiteOfPlanRoom is null)
        {
            return request.PlanId;
        }

        Mapper.Map(request.Payload, existingSiteOfPlanRoom);

        var editSiteOfPlanRoom = await planRoomGroupSiteService.UpdateAsync(
            existingSiteOfPlanRoom,
            cancellationToken: cancellationToken
        );

        return editSiteOfPlanRoom.PlanId;
    }
}
