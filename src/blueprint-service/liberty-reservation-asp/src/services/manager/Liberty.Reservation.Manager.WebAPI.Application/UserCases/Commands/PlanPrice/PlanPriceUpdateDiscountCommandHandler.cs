namespace Liberty.Reservation.Manager.WebAPI.Application.UserCases.Commands.PlanPrice;

public class PlanPriceUpdateDiscountCommandHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IPlanRoomGroupSiteDiscountDataService planRoomGroupSiteDiscountDataService,
    IPlanService planService,
    ILogger<PlanPriceUpdateDiscountCommandHandler> logger
) : UpdateCommandHandlerBase<PlanPriceUpdateDiscountCommand, long>(unitOfWork, mapper)
{
    protected override async Task<long> HandleAsync(
        PlanPriceUpdateDiscountCommand request,
        CancellationToken cancellationToken
    )
    {
        var listDiscountDataOfSiteInPlanRoom = new List<PlanRoomGroupSiteDiscountData>();

        foreach (var discount in request.Payload.DiscountDatas!)
        {
            listDiscountDataOfSiteInPlanRoom.Add(
                new PlanRoomGroupSiteDiscountData
                {
                    PlanId = request.PlanId,
                    RoomGroupId = request.RoomTypeId,
                    SiteId = request.SiteId,
                    DiscountData = new DiscountData
                    {
                        StartPrevDay = discount.StartPrevDay,
                        EndPrevDay = discount.EndPrevDay,
                        PersonMin = discount.PersonMin,
                        PersonMax = discount.PersonMax,
                        PriceSettingType = discount.PriceSettingType,
                        Value = discount.Value,
                        IsEnabled = true
                    },
                    IsEnabled = true
                }
            );
        }

        try
        {
            await UnitOfWork.BeginTransactionAsync(cancellationToken: cancellationToken);

            _ = await planRoomGroupSiteDiscountDataService.ChangeDiscountDataOfSiteInPlanRoom(
                request.PlanId,
                request.RoomTypeId,
                request.SiteId,
                listDiscountDataOfSiteInPlanRoom,
                false,
                cancellationToken
            );

            await planService.UpdateLastModifiedAsync(request.PlanId, cancellationToken);

            await UnitOfWork.CommitAsync(cancellationToken);

            return request.PlanId;
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "{Action} {Message}",
                nameof(PlanPriceUpdateStandardPriceCommandHandler),
                ex.Message
            );
            await UnitOfWork.RollbackAsync(cancellationToken);
            throw new AppLibertyException(ex.Message, ex);
        }
    }
}
